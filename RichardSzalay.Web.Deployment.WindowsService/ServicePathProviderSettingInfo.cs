using Microsoft.Web.Deployment;
using RichardSzalay.Web.Deployment.WindowsService.Properties;
using System;

namespace RichardSzalay.Web.Deployment.WindowsService
{
    public class ServicePathProviderSettingInfo : DeploymentProviderSettingInfo
    {
        public ServicePathProviderSettingInfo(string providerFactoryName)
            : base("serviceTimeout", providerFactoryName)
        {

        }

        public override string Description => Resources.ServiceTimeoutSettingDescription;

        public override string FriendlyName => Resources.ServiceTimeoutSettingFriendlyName;

        public override Type Type => typeof(int);
    }
}
