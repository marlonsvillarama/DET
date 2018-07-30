using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace Monexa.DET
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TraceExtensionAttribute : SoapExtensionAttribute
    {
        private string fileName = Directory.GetCurrentDirectory() + "\\trace-log.txt";
        private int priority;

        public override Type ExtensionType => typeof(SOAPTracer);

        public override int Priority
        {
            get => priority;
            set => priority = value;
        }

        public string FileName
        {
            get => fileName;
            set => fileName = value;
        }
    }
}
