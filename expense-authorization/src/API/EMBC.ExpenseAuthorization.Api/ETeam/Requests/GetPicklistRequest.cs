using System.Xml;

namespace EMBC.ExpenseAuthorization.Api.ETeam.Requests
{
    public abstract class GetPicklistRequest : SoapRequest
    {
        private readonly string _requestName;

        protected GetPicklistRequest(string requestName)
        {
            _requestName = requestName;
        }

        protected string Key { get; set; }

        protected override void WriteBody(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement(_requestName, "urn:extds");

            xmlWriter.WriteStartElement("key");
            xmlWriter.WriteString(Key);
            xmlWriter.WriteEndElement(); // </key>
            xmlWriter.WriteEndElement(); // </_requestName>
        }
    }
}
