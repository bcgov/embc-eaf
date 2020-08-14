using System.IO;
using System.Text;
using System.Xml;

namespace EMBC.ExpenseAuthorization.Api.ETeam.Requests
{
    public abstract class SoapRequest
    {
        public string CreateSoapRequest()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            StringBuilder buffer = new StringBuilder();

            using var stream = new StringWriter(buffer);
            using var xmlWriter = XmlWriter.Create(stream, settings);

            xmlWriter.WriteStartElement( "Envelope", "http://www.w3.org/2003/05/soap-envelope");

            //xmlWriter.WriteAttributeString("xmlns", "soapenv", null, "http://schemas.xmlsoap.org/soap/envelope");
            xmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
            xmlWriter.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
            xmlWriter.WriteAttributeString("xmlns", "urn", null, "urn:extds");

            xmlWriter.WriteStartElement("Header", "http://www.w3.org/2003/05/soap-envelope");
            xmlWriter.WriteEndElement(); // </Header>

            xmlWriter.WriteStartElement("Body", "http://www.w3.org/2003/05/soap-envelope");
            WriteBody(xmlWriter);
            xmlWriter.WriteEndElement(); // </Body>

            xmlWriter.WriteEndElement(); // </Envelope>

            xmlWriter.Flush();

            return buffer.ToString();
        }

        protected abstract void WriteBody(XmlWriter xmlWriter);
    }
}
