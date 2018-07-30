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
    public class SOAPTracer : SoapExtension
    {
        Stream oldStream;
        Stream newStream;
        string fileName;

        public override Stream ChainStream(Stream stream)
        {
            oldStream = stream;
            newStream = new MemoryStream();
            return newStream;
        }

        public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
        {
            return ((TraceExtensionAttribute)attribute).FileName;
        }

        public override object GetInitializer(Type serviceType)
        {
            return Directory.GetCurrentDirectory() + "\\ws-log.txt";
        }

        public override void Initialize(object initializer)
        {
            fileName = (string)initializer;
        }

        public override void ProcessMessage(SoapMessage message)
        {
            switch (message.Stage)
            {
                case SoapMessageStage.BeforeSerialize:
                    break;
                case SoapMessageStage.AfterSerialize:
                    WriteOutput(message);
                    break;
                case SoapMessageStage.BeforeDeserialize:
                    WriteInput(message);
                    break;
                case SoapMessageStage.AfterDeserialize:
                    break;
                default:
                    throw new Exception("invalid stage");
            }
        }

        public void WriteOutput(SoapMessage message)
        {
            newStream.Position = 0;
            FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write);
            StreamWriter w = new StreamWriter(fs);

            string soap = (message is SoapServerMessage) ? "SoapResponse" : "SoapRequest";
            w.WriteLine("-----" + soap + " at " + DateTime.Now);
            w.Flush();
            Copy(newStream, fs);
            w.Close();
            newStream.Position = 0;
            Copy(newStream, oldStream);
        }

        public void WriteInput(SoapMessage message)
        {
            Copy(oldStream, newStream);
            FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write);
            StreamWriter w = new StreamWriter(fs);

            string soap = (message is SoapServerMessage) ? "SoapRequest" : "SoapResponse";
            w.Write("-----" + soap + " at " + DateTime.Now);
            w.Flush();
            newStream.Position = 0;
            Copy(newStream, fs);
            w.Close();
            newStream.Position = 0;
        }

        private void Copy(Stream from, Stream to)
        {
            TextReader reader = new StreamReader(from);
            TextWriter writer = new StreamWriter(to);
            writer.WriteLine(reader.ReadToEnd());
            writer.Flush();
        }
    }
}
