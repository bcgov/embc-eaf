using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace EMBC.ExpenseAuthorization.Api.ETeam.Responses
{
    public abstract class SoapResponse
    {
        /// <summary>
        /// Loads response from supplied XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <exception cref="ArgumentNullException">xml</exception>
        /// <exception cref="ArgumentException">Parameter cannot be empty string - xml</exception>
        /// <exception cref="SoapFaultException">The <paramref name="xml"/> represents a SOAP fault.</exception>
        public void LoadFromXml(string xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException(nameof(xml));
            }

            if (string.IsNullOrWhiteSpace(xml))
            {
                throw new ArgumentException("Parameter cannot be empty string", nameof(xml));
            }

            var document = XDocument.Parse(xml);

            XElement element = (XElement)document.Document
                .Elements(XName.Get(@"{http://www.w3.org/2003/05/soap-envelope}Envelope"))
                .Elements(XName.Get(@"{http://www.w3.org/2003/05/soap-envelope}Body"))
                .Single()
                .FirstNode;

            if (element.Name == XName.Get(@"{http://www.w3.org/2003/05/soap-envelope}Fault"))
            {
                // soap fault
                throw new SoapFaultException(element);
            }

            ReadElementsFromXml(element);
        }
        
        /// <summary>
        /// Reads the elements from XML.
        /// </summary>
        /// <param name="element">The reader.</param>
        protected virtual void ReadElementsFromXml(XElement element)
        {
        }
        
        protected KeyValuePair<string,string> ReadItemFromXml(XElement item)
        {
            Debug.Assert(item.NodeType == XmlNodeType.Element);
            Debug.Assert(item.Name.LocalName == "item");

            string key = item.XPathSelectElement("./key")?.Value ?? string.Empty;
            string value = item.XPathSelectElement("./value")?.Value ?? string.Empty;

            return new KeyValuePair<string, string>(key, value);
        }
    }
}
