using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace EMBC.ExpenseAuthorization.Api.ETeam.Responses
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateReportResponse : SoapResponse
    {
        public IDictionary<string,string> Fields { get; protected set; }

        protected override void ReadElementsFromXml(XElement element)
        {
            var createReturn = element.XPathSelectElement("./createReturn");

            IEnumerable<KeyValuePair<string, string>> keyValues = createReturn
                .XPathSelectElements("./item")
                .Select(ReadItemFromXml)
                .Where(_ => !string.IsNullOrEmpty(_.Key));

            Fields = new Dictionary<string, string>(keyValues);
        }
    }
}
