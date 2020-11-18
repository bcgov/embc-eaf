using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace EMBC.ExpenseAuthorization.Api.ETeam.Responses
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateReportResponse : SoapResponse
    {
        private IDictionary<string, string> _fields;

        public IDictionary<string, string> Fields
        {
            get
            {
                return _fields ??= new Dictionary<string, string>();
            }
            protected set
            {
                _fields = value;
            }
        }

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
