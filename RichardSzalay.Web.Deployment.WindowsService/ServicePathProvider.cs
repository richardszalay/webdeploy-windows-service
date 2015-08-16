using System;
using Microsoft.Web.Deployment;
using System.IO;
using RichardSzalay.Web.Deployment.WindowsService.Properties;
using System.Collections.Generic;

namespace RichardSzalay.Web.Deployment.WindowsService
{
    class ServicePathProvider : DeploymentObjectProvider
    {
        public const string ObjectName = "servicePath";

        DeploymentBaseContext baseContext;
        DeploymentProviderContext providerContext;
        bool _isDirectory;

        public ServicePathProvider(DeploymentProviderContext providerContext, DeploymentBaseContext baseContext)
            : base(baseContext)
        {
            this.providerContext = providerContext;
            this.baseContext = baseContext;
        }

        public override string Name => ObjectName;

        public string Path { get; private set; }

        internal void EnsureNodeExists(out bool isDirectory)
        {
            isDirectory = false;
            EnsureNodeExists(ServicePathHelper.GetPhysicalPath(providerContext.Path), out isDirectory);
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
                //if (COMHelper.IsExceptionSameAsError((Exception)ioException, ErrorCode.FileNotFound))
                //ioException = (IOException)null;
                string serverMessage = String.Format(Resources.FileNotFound_FileName, physicalPath);
                string clientMessage = String.Format(Resources.FileNotFound_FileName, Path);

                throw new DeploymentDetailedClientServerException(ioException, DeploymentErrorCode.FileOrFolderNotFound, clientMessage, serverMessage);
            }
        }

        public override DeploymentObjectAttributeData CreateKeyAttributeData()
        {
            return new DeploymentObjectAttributeData("path", providerContext.Path, DeploymentObjectAttributeKind.CaseInsensitiveCompare);
        }

        public override void GetAttributes(DeploymentAddAttributeContext addContext)
        {
            EnsureNodeExists(out _isDirectory);
        }

        public override void Add(DeploymentObject source, bool whatIf)
        {
            ServicePathHelper.GetPhysicalPath(providerContext.Path);

            foreach(var deploymentObject in source.GetChildren())
            {
                if (deploymentObject.Name == "dirPath")
                {
                    _isDirectory = true;
                    break;
                }
            }
        }

        public override DeploymentObjectProvider AddChild(DeploymentObject source, int position, bool whatIf)
        {
            string physicalPath = ServicePathHelper.GetPhysicalPath(Path);

            if (source.Name == "contentPathLib")
                return CreateProvider("contentPathLib", Path, "ContentPathLib");

            if (source.Name == "dirPath" && !_isDirectory)
                throw new DeploymentException(Resources.FileAlreadyExists, physicalPath);

            throw new DeploymentException(Resources.DirectoryAlreadyExists, physicalPath);
        }

        public override IEnumerable<DeploymentObjectProvider> GetChildProviders()
        {
            string physicalPath = ServicePathHelper.GetPhysicalPath(providerContext.Path);

            string childProviderName = _isDirectory ? "dirPath" : "filePath";

            yield return CreateProvider(childProviderName, physicalPath, Name);
        }

        public override string GetAddMessage(string absolutePath)
        {
            absolutePath = absolutePath ?? string.Empty;

            return string.Format(Resources.AddingContentPathMessage, absolutePath);
        }

        public override string GetAddChildMessage(string childProviderName, string absolutePath)
        {
            return GetAddMessage(absolutePath);
        }
    }
}