using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monexa.DET
{
    static class SoapTemplates
    {
        public static string SoapEnvelope
        {
            get
            {
                return new StringBuilder()
                    .Append(@"<?xml version=""1.0"" encoding=""utf-8""?>")
                    .Append(@"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" ")
                    .Append(@"xmlns:xsi=""http://www.w3.org/1999/XMLSchema-instance"" ")
                    .Append(@"xmlns:xsd=""http://www.w3.org/1999/XMLSchema"">")
                    .Append(@"<soap:Body>")
                    //.Append(@"<ip_applications_API>")
                    //.Append(@"<api_version>1.0</api_version>")
                    //.Append(@"<Request>")
                    //.Append(@"{authentication}")
                    //.Append(@"<Command>{command}</Command>")
                    //.Append(@"</Request>")
                    //.Append(@"</ip_applications_API>")
                    .Append("{command}")
                    .Append(@"</soap:Body>")
                    .Append(@"</soap:Envelope>")
                    .ToString();
            }
        }

        public static string SoapRequestNoEnvelope
        {
            get
            {
                return new StringBuilder()
                    //.Append(@"<?xml version=""1.0"" encoding=""utf-8""?>")
                    //.Append(@"<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" ")
                    //.Append(@"xmlns:xsi=""http://www.w3.org/1999/XMLSchema-instance"" ")
                    //.Append(@"xmlns:xsd=""http://www.w3.org/1999/XMLSchema"">")
                    //.Append(@"<SOAP-ENV:Body>")
                    .Append(@"<ip_applications_API>")
                    .Append(@"<api_version>1.0</api_version>")
                    .Append(@"<Request>")
                    .Append(@"{authentication}")
                    .Append(@"<Command>{command}</Command>")
                    .Append(@"</Request>")
                    .Append(@"</ip_applications_API>")
                    //.Append(@"</SOAP-ENV:Body>")
                    //.Append(@"</SOAP-ENV:Envelope>")
                    .ToString();
            }
        }

        public static string Authentication
        {
            get
            {
                return new StringBuilder()
                    .Append(@"<Authentication>")
                    .Append(@"<administrator_login_name>{administrator_login_name}</administrator_login_name>")
                    .Append(@"<password>{password}</password>")
                    .Append(@"</Authentication>")
                    .ToString();
            }
        }

        public static string PingServer
        {
            get
            {
                return new StringBuilder()
                    //.Append(@"<PING></PING>")
                    .Append("{authentication}")
                    .ToString();
            }
        }

        public static string SearchSubscribers
        {
            get
            {
                return new StringBuilder()
                    .Append(@"<SearchSubscriberRequest xmlns=""http://www.monexa.com"">")
                    .Append(@"{authentication}")
                    .Append(@"<SEARCH_SUBSCRIBER>")
                    .Append(@"<account_type>all</account_type>")
                    .Append(@"</SEARCH_SUBSCRIBER>")
                    .Append(@"</SearchSubscriberRequest>")
                    .ToString();
            }
        }
    }
}
