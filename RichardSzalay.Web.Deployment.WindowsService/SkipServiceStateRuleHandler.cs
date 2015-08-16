using Microsoft.Web.Deployment;
using RichardSzalay.Web.Deployment.WindowsService.Properties;
using System;
using System.IO;

namespace RichardSzalay.Web.Deployment.WindowsService
{
    [DeploymentRuleHandler]
    public class SkipServiceStateRuleHandler : DeploymentRuleHandler
    {
        public override string Description => Resources.SkipServiceStateRuleDescription;

        public override bool EnabledByDefault => true;

        public override string FriendlyName => Resources.SkipServiceStateRuleFriendlyName;

        public override string Name => "SkipServiceState";

        public override void Update(DeploymentSyncContext syncContext, DeploymentObject destinationObject, ref DeploymentObject sourceObject, ref bool proceed)
        {
            if (IsServiceStateProvider(destinationObject))
            {
                syncContext.SourceObject.BaseContext.RaiseEvent(new WindowsServiceTraceEvent(
                    Resources.SkipServiceStateEvent, "Update", destinationObject.AbsolutePath));

                proceed = false;
            }
        }

        public override void Add(DeploymentSyncContext syncContext, DeploymentObject destinationObject, ref DeploymentObject sourceObject, ref bool proceed)
        {
            if (IsServiceStateProvider(destinationObject))
            {
                syncContext.SourceObject.BaseContext.RaiseEvent(new WindowsServiceTraceEvent(
                    Resources.SkipServiceStateEvent, "Add", destinationObject.AbsolutePath));

                proceed = false;
            }
        }

        public override void Delete(DeploymentSyncContext syncContext, DeploymentObject destinationObject, DeploymentObject sourceParentObject, ref bool proceed)
        {
            if (IsServiceStateProvider(destinationObject))
            {
                syncContext.SourceObject.BaseContext.RaiseEvent(new WindowsServiceTraceEvent(
                    Resources.SkipServiceStateEvent, "Delete", destinationObject.AbsolutePath));

                proceed = false;
            }
        }

        bool IsServiceStateProvider(DeploymentObject destinationObject)
        {
            if (destinationObject.ProviderName != "filePath")
                return false;

            string extension = Path.GetExtension(destinationObject.AbsolutePath);

            return string.Equals(extension, ".InstallLog", StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(extension, ".InstallState", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
