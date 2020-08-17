using System;
using System.Xml.Linq;
using System.Xml.XPath;

namespace EMBC.ExpenseAuthorization.Api.ETeam.Responses
{
    public class SoapFaultException : Exception
    {
        public SoapFaultException(XElement faultElement)
            : base("An SOAP Fault error occurred")
        {
            using var reader = faultElement.CreateReader();
            reader.MoveToContent();

            SoapFault = reader.ReadOuterXml();
        }

        public string SoapFault { get; }
    }
}
