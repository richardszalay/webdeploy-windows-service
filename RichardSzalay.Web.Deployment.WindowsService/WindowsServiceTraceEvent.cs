using Microsoft.Web.Deployment;
using System.Diagnostics;

namespace RichardSzalay.Web.Deployment.WindowsService
{
    class WindowsServiceTraceEvent : DeploymentTraceEventArgs
    {
        readonly string _message;

        public override string Message => _message;

        public WindowsServiceTraceEvent(string message, params object[] parameters)
          : this(TraceLevel.Verbose, message, parameters)
        {
        }

        public WindowsServiceTraceEvent(TraceLevel level, string message, params object[] parameters)
          : this(level, string.Format(message, parameters))
        {
        }

        public WindowsServiceTraceEvent(TraceLevel level, string message)
          : base(level)
        {
            _message = message;
        }
    }
}
