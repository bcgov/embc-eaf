using System;
using System.Collections.Generic;
using System.Xml;

namespace EMBC.ExpenseAuthorization.Api.ETeam.Requests
{
    public class CreaterReportRequest : SoapRequest
    {
        private readonly IDictionary<string, string> _items;

        public CreaterReportRequest(IDictionary<string,string> items)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
        }

        protected override void WriteBody(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("create", "urn:extds");
            xmlWriter.WriteStartElement("report");

            foreach (var item in _items)
            {
                WriteItem(xmlWriter, item);
            }

            xmlWriter.WriteEndElement(); // </report>
            xmlWriter.WriteEndElement(); // </create>
        }

        private static void WriteItem(XmlWriter xmlWriter, KeyValuePair<string, string> item)
        {
            xmlWriter.WriteStartElement("item");

            xmlWriter.WriteStartElement("key");
            xmlWriter.WriteString(item.Key);
            xmlWriter.WriteEndElement(); // </key>

            xmlWriter.WriteStartElement("value");
            xmlWriter.WriteString(item.Value);
            xmlWriter.WriteEndElement(); // </value>

            xmlWriter.WriteEndElement(); // </item>
        }

    }
}
