using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Monexa.DET
{
    public class XmlHelper
    {
        private string _LinearizeXml;
        private XDocument _Document;
        private string _PrettyXml;

        public XmlHelper(string text)
        {
            Text = text;
        }

        #region Properties
        public enum XmlEncoding
        {
            UTF8,
            UTF16
        };

        public string Text
        {
            get;
            private set;
        }

        public string LinearizeXml
        {
            get
            {
                return _LinearizeXml ?? (_LinearizeXml = Clean(Document, LinearizedSettings));
            }
        }

        public XDocument Document
        {
            get
            {
                return _Document ?? (_Document = XDocument.Parse(GetLinearizedXml(Text)));
            }
        }

        public string PrettyXml
        {
            get
            {
                return _PrettyXml ?? (_PrettyXml = Clean(Document, PrettySettings));
            }
        }

        public XmlEncoding Encoding
        {
            get;
            set;
        }

        public Stream ToStream()
        {
            return new MemoryStream(ToByteArray());
        }

        public byte[] ToByteArray()
        {
            return GetEncoding().GetBytes(PrettyXml ?? "");
        }

        private Encoding GetEncoding()
        {
            switch (Encoding)
            {
                case XmlEncoding.UTF8:
                    return XmlUTF8Encoding.Instance;
                case XmlEncoding.UTF16:
                    return XmlUnicode.Instance;
                default:
                    return XmlUnicode.Instance;
            }
        }

        private XmlWriterSettings LinearizedSettings
        {
            get
            {
                return new XmlWriterSettings
                {
                    Encoding = GetEncoding(),
                    Indent = false,
                    NewLineOnAttributes = false
                };
            }
        }

        private XmlWriterSettings PrettySettings
        {
            get
            {
                return new XmlWriterSettings
                {
                    Encoding = GetEncoding(),
                    Indent = true,
                    IndentChars = string.IsNullOrEmpty(IndentCharacters) ? " " : IndentCharacters,
                    NewLineOnAttributes = false,
                    NewLineHandling = NewLineHandling.Replace
                };
            }
        }

        public string IndentCharacters
        {
            get;
            set;
        }

        private string Clean(XDocument doc, XmlWriterSettings settings)
        {
            StringBuilder sb = new StringBuilder();
            var stringWriter = new StringWriterWithEncoding(sb, GetEncoding());
            using(var xmlWriter = XmlWriter.Create(stringWriter, settings))
            {
                doc.Save(xmlWriter);
                xmlWriter.Flush();
                return sb.ToString();
            }
        }

        private string GetLinearizedXml(string text)
        {
            var halfclean = Regex.Replace(text, @"\s+", " ", RegexOptions.Singleline);
            var clean75 = Regex.Replace(halfclean, @">\s+", ">");
            var fullclean = Regex.Replace(clean75, @">\s+", ">");
            return fullclean;
        }
        #endregion

        #region Related Classes
        private sealed class StringWriterWithEncoding : StringWriter
        {
            private readonly Encoding _Encoding;

            public StringWriterWithEncoding(StringBuilder builder, Encoding encoding) : base(builder)
            {
                _Encoding = encoding;
            }

            public override Encoding Encoding => _Encoding;
        }

        private sealed class XmlUTF8Encoding : UTF8Encoding
        {
            private static XmlUTF8Encoding _XmlUTF8Encoding;

            public override string WebName => base.WebName.ToUpper();

            public override string HeaderName => base.HeaderName.ToUpper();

            public override string BodyName => base.BodyName.ToUpper();

            public static XmlUTF8Encoding Instance
            {
                get
                {
                    return _XmlUTF8Encoding ?? (_XmlUTF8Encoding = new XmlUTF8Encoding());
                }
            }
        }

        private sealed class XmlUnicode : UnicodeEncoding
        {
            private static XmlUnicode _XmlUnicode;

            public override string WebName => base.WebName.ToUpper();

            public override string HeaderName => base.HeaderName.ToUpper();

            public override string BodyName => base.BodyName.ToUpper();

            public static XmlUnicode Instance
            {
                get
                {
                    return _XmlUnicode ?? (_XmlUnicode = new XmlUnicode());
                }
            }
        }
        #endregion
    }
}
