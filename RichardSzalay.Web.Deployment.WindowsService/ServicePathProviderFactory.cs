using Microsoft.Web.Deployment;
using RichardSzalay.Web.Deployment.WindowsService.Properties;

namespace RichardSzalay.Web.Deployment.WindowsService
{
    [DeploymentProviderFactory]
    public class ServicePathProviderFactory : DeploymentProviderFactory
    {
        public override string ExamplePath => "myService";

        public override string FriendlyName => Resources.ProviderFriendlyNameServicePath;

        public override string Name => "servicePath";

        public override string Description => Resources.ServicePathProviderDescription;

        protected override DeploymentObjectProvider Create(DeploymentProviderContext providerContext, DeploymentBaseContext baseContext)
        {
            return new ServicePathProvider(providerContext, baseContext);
        }

        public override DeploymentProviderSettingInfo[] GetSupportedSettings()
        {
            return new[]
            {
                new ServiceTimeoutSettingInfo(Name)
            };
        }
    }
}
