using Microsoft.Web.Deployment;
using RichardSzalay.Web.Deployment.WindowsService.Properties;
using System.Collections.Generic;
using System.IO;

namespace RichardSzalay.Web.Deployment.WindowsService
{
    class ServicePathLibProvider : DeploymentObjectProvider
    {
        internal const string ObjectName = "servicePathLib";
        internal const string LibFolderName = "serviceroot";
        bool _isDirectory;

        public override string Name => ObjectName;

        public string Path => ProviderContext.Path;

        internal ServicePathLibProvider(DeploymentProviderContext providerContext, DeploymentBaseContext baseContext)
          : base(providerContext, baseContext)
        {
        }

        public override DeploymentObjectAttributeData CreateKeyAttributeData()
        {
            return new DeploymentObjectAttributeData("path", Path, DeploymentObjectAttributeKind.CaseInsensitiveCompare);
        }

        public override void GetAttributes(DeploymentAddAttributeContext addContext)
        {
            EnsureNodeExists(ServicePathHelper.GetPhysicalPath(Path), out _isDirectory);
        }

        internal void EnsureNodeExists(out bool isDirectory)
        {
            isDirectory = false;
            EnsureNodeExists(ServicePathHelper.GetPhysicalPath(Path), out isDirectory);
        }

        protected virtual void EnsureNodeExists(string physicalPath, out bool isDirectory)
        {
            isDirectory = false;

            try
            {
                if ((File.GetAttributes(physicalPath) & FileAttributes.Directory) != FileAttributes.Directory)
                    return;
                isDirectory = true;
            }
            catch (IOException ex)
            {
                IOException ioException = ex;

                string serverMessage = string.Format(Resources.FileNotFound_FileName, physicalPath);
                string clientMessage = string.Format(Resources.FileNotFound_FileName, Path);

                throw new DeploymentDetailedClientServerException(ioException, DeploymentErrorCode.FileOrFolderNotFound, clientMessage, serverMessage);
            }
        }

        public override void Add(DeploymentObject source, bool whatIf)
        {
            foreach (DeploymentObject deploymentObject in source.GetChildren())
            {
                if (deploymentObject.Name == "dirPath")
                {
                    _isDirectory = true;
                    break;
                }
            }
        }

        public override IEnumerable<DeploymentObjectProvider> GetChildProviders()
        {
            if (_isDirectory)
            {
                yield return CreateDirPathProvider();
            }
            else
            {
                string physicalPath = ServicePathHelper.GetPhysicalPath(Path);
                yield return CreateProvider("filePath", physicalPath, Name);
            }
        }

        public override DeploymentObjectProvider AddChild(DeploymentObject source, int position, bool whatIf)
        {
            if (source.Name == "dirPath")
                return CreateDirPathProvider();

            return null;
        }

        DeploymentObjectProvider CreateDirPathProvider()
        {
            var providerOptions = new DeploymentProviderOptions("dirPath");
            providerOptions.Path = ServicePathHelper.GetPhysicalPath(Path);

            DeploymentObjectProvider provider = CreateProvider(providerOptions, ObjectName + LibFolderName);
            return provider;
        }

        public override string GetAddChildMessage(string childProviderName, string absolutePath)
        {
            return GetAddMessage(absolutePath);
        }

        public override string GetAddMessage(string absolutePath)
        {
            absolutePath = absolutePath ?? string.Empty;
            return string.Format(Resources.ServicePathLibProviderAddMessage, absolutePath);
        }

        public override string GetDeleteMessage(string absolutePath)
        {
            absolutePath = absolutePath ?? string.Empty;
            return string.Format(Resources.ServicePathLibProviderDeleteMessage, absolutePath);
        }

        public override string GetUpdateMessage(string absolutePath)
        {
            absolutePath = absolutePath ?? string.Empty;
            return string.Format(Resources.ServicePathLibProviderUpdateMessage, absolutePath);
        }
    }
}
