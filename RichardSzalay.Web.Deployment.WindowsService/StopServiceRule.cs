using Microsoft.Web.Deployment;
using RichardSzalay.Web.Deployment.WindowsService.Properties;
using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace RichardSzalay.Web.Deployment.WindowsService
{
    [DeploymentRuleHandler]
    public class StopServiceRule : DeploymentRuleHandler
    {
        bool _hasStoppedService = false;

        public override string Description => Resources.StopServiceRuleDescription;

        public override bool EnabledByDefault => true;

        public override string FriendlyName => Resources.StopServiceRuleFriendlyName;

        public override string Name => "StopService";

        public override void Add(DeploymentSyncContext syncContext, DeploymentObject destinationObject, ref DeploymentObject sourceObject, ref bool proceed)
        {
            StopService(syncContext);
        }

        public override void AddChild(DeploymentSyncContext syncContext, DeploymentObject destinationParentObject, ref DeploymentObject sourceObject, ref bool proceed)
        {
            StopService(syncContext);
        }

        public override void Update(DeploymentSyncContext syncContext, DeploymentObject destinationObject, ref DeploymentObject sourceObject, ref bool proceed)
        {
            StopService(syncContext);
        }

        public override void Delete(DeploymentSyncContext syncContext, DeploymentObject destinationObject, DeploymentObject sourceParentObject, ref bool proceed)
        {
            StopService(syncContext);
        }

        void StopService(DeploymentSyncContext syncContext)
        {
            if (_hasStoppedService)
                return;

            _hasStoppedService = true;

            var provider = syncContext.DestinationObject;

            if (provider.Name == "MSDeploy." + ServicePathProvider.ObjectName &&
                !ServicePathHelper.IsAbsolutePhysicalPath(provider.ProviderContext.Path))
            {
                var serviceName = ServicePathHelper.GetServiceName(provider.ProviderContext.Path);

                var serviceController = new ServiceController(serviceName);

                if (serviceController.Status == ServiceControllerStatus.Running ||
                    serviceController.Status == ServiceControllerStatus.Paused)
                {
                    syncContext.SourceObject.BaseContext.RaiseEvent(new WindowsServiceTraceEvent(TraceLevel.Info, Resources.StoppingServiceEvent, serviceName));

                    if (!syncContext.WhatIf)
                        serviceController.Stop();
                }

                if (!syncContext.WhatIf)
                {
                    int serviceTimeoutSeconds = provider.ProviderContext.ProviderOptions.ProviderSettings.GetValueOrDefault("serviceTimeout", 20);
                    serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(serviceTimeoutSeconds));
                }

                // TODO: Info, service already stopped
                // TODO: Invalid service name
                // TODO: Can't stop service (perms)
                // TODO: Can't stop service (timeout)
            }

            base.PreSync(syncContext);
        }
    }
}
