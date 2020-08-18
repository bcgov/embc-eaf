using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Refit;

namespace EMBC.ExpenseAuthorization.Api.ETeam.Responses
{
    public class SoapFaultException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="SoapFaultException" /> class.</summary>
        /// <param name="exception">The exception.</param>
        public SoapFaultException(ApiException exception)
        {
            var document = XDocument.Parse(exception.Content);

            XElement faultElement = (XElement)document.Document
                .Elements(XName.Get(@"{http://www.w3.org/2003/05/soap-envelope}Envelope"))
                .Elements(XName.Get(@"{http://www.w3.org/2003/05/soap-envelope}Body"))
                .Single()
                .FirstNode;

            using var reader = faultElement.CreateReader();
            reader.MoveToContent();

            SoapFault = reader.ReadOuterXml();
        }

        public SoapFaultException(XElement faultElement)
            : base("An SOAP Fault error occurred")
        {
            using var reader = faultElement.CreateReader();
            reader.MoveToContent();

            SoapFault = reader.ReadOuterXml();
        }

        public string SoapFault { get; }

        public override string ToString()
        {
            return this.Message + ":" + SoapFault;
        }
    }
}
