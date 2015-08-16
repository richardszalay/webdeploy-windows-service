using Microsoft.Web.Deployment;
using RichardSzalay.Web.Deployment.WindowsService.Properties;
using System;
using System.IO;

namespace RichardSzalay.Web.Deployment.WindowsService
{
    static class ServicePathHelper
    {
        static char[] DirectorySeparatorChars = { '\\', '/' };

        public static string GetPhysicalPath(string path)
        {
            if (IsAbsolutePhysicalPath(path))
                return path;

            string serviceName;
            string contentPath;
            GetAbsoluteServicePath(path, out serviceName, out contentPath);

            string servicePath = GetServicePath(serviceName);

            return servicePath + contentPath;
        }

        static string GetServicePath(string serviceName)
        {
            var serviceImagePath = Microsoft.Win32.Registry.GetValue(
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\" + serviceName, "ImagePath", null) as string;


            serviceImagePath = NormaliseServiceImagePath(serviceImagePath);

            return Path.GetDirectoryName(serviceImagePath);
        }

        static string NormaliseServiceImagePath(string serviceImagePath)
        {
            // TODO: Validate uncommon path formats

            if (serviceImagePath.StartsWith("\"", StringComparison.Ordinal))
                serviceImagePath = serviceImagePath.Substring(1);

            if (serviceImagePath.EndsWith("\"", StringComparison.Ordinal))
                serviceImagePath = serviceImagePath.Substring(0, serviceImagePath.Length - 1);

            return serviceImagePath;
        }

        static void GetAbsoluteServicePath(string path, out string serviceName, out string contentPath)
        {
            string[] pathParts = path.Split(DirectorySeparatorChars, StringSplitOptions.RemoveEmptyEntries);

            if (pathParts.Length < 1 || string.IsNullOrEmpty(pathParts[0]))
                throw new DeploymentDetailedException(DeploymentErrorCode.ERROR_APP_DOES_NOT_EXIST, Resources.UnknownServiceName, path);

            serviceName = pathParts[0];

            contentPath = pathParts.Length <= 1
                ? "\\"
                : "\\" + string.Join("\\", pathParts, 1, pathParts.Length - 1);
        }

        public static string GetServiceName(string path)
        {
            string serviceName, contentPath;

            GetAbsoluteServicePath(path, out serviceName, out contentPath);

            return serviceName;
        }

        public static bool IsAbsolutePhysicalPath(string path)
        {
            if (string.IsNullOrEmpty(path) || path.Length < 3)
                return false;

            return (path[1] == Path.VolumeSeparatorChar && IsDirectorySeparatorChar(path[2]) || 
                IsDirectorySeparatorChar(path[0]) && IsDirectorySeparatorChar(path[1]));
        }

        static bool IsDirectorySeparatorChar(char c)
        {
            return c == Path.DirectorySeparatorChar ||
                c == Path.AltDirectorySeparatorChar;
        }
    }
}
