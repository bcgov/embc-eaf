using System;
using System.Xml;

namespace EMBC.ExpenseAuthorization.Api.ETeam.Requests
{
    public class LoginRequest : SoapRequest
    {
        private readonly string _username;
        private readonly string _password;

        public LoginRequest(string username, string password)
        {
            _username = username ?? throw new ArgumentNullException(nameof(username));
            _password = password ?? throw new ArgumentNullException(nameof(password));
        }
        
        protected override void WriteBody(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("login", "urn:extds");

            xmlWriter.WriteStartElement("username");
            xmlWriter.WriteString(_username);
            xmlWriter.WriteEndElement(); // </username>

            xmlWriter.WriteStartElement("password");
            xmlWriter.WriteString(_password);
            xmlWriter.WriteEndElement(); // </password>

            xmlWriter.WriteEndElement(); // </login>
        }
    }
}
