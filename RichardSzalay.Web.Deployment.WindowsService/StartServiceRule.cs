using Microsoft.Web.Deployment;
using RichardSzalay.Web.Deployment.WindowsService.Properties;
using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace RichardSzalay.Web.Deployment.WindowsService
{
    [DeploymentRuleHandler]
    public class StartServiceRule : DeploymentRuleHandler
    {
        public override string Description => Resources.StartServiceRuleDescription;

        public override bool EnabledByDefault => true;

        public override string FriendlyName => Resources.StartServiceRuleFriendlyName;

        public override string Name => "StartService";

        public override void PostSync(DeploymentSyncContext syncContext)
        {
            var provider = syncContext.DestinationObject;

            if (provider.Name == "MSDeploy." + ServicePathProvider.ObjectName &&
                !ServicePathHelper.IsAbsolutePhysicalPath(provider.ProviderContext.Path))
            {
                var serviceName = ServicePathHelper.GetServiceName(provider.ProviderContext.Path);

                var serviceController = new ServiceController(serviceName);

                if (serviceController.Status == ServiceControllerStatus.Stopped)
                {
                    syncContext.SourceObject.BaseContext.RaiseEvent(new WindowsServiceTraceEvent(TraceLevel.Info, Resources.StartingServiceEvent, serviceName));

                    if (!syncContext.WhatIf)
                        serviceController.Start();
                }

                if (!syncContext.WhatIf)
                {
                    int serviceTimeoutSeconds = provider.ProviderContext.ProviderOptions.ProviderSettings.GetValueOrDefault("serviceTimeout", 20);
                    serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(serviceTimeoutSeconds));
                }
            }

            base.PreSync(syncContext);
        }
    }
}
