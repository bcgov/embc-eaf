using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace EMBC.ExpenseAuthorization.Api.ETeam.Responses
{
    public abstract class GetPicklistResponse : SoapResponse
    {
        private string _returnPath;

        public IList<LookupValue> Values { get; private set; } = Array.Empty<LookupValue>();

        protected GetPicklistResponse(string returnPath)
        {
            _returnPath = returnPath ?? throw new ArgumentNullException(nameof(returnPath));
        }

        protected override void ReadElementsFromXml(XElement element)
        {
            var values = new List<LookupValue>();

            foreach (var returnValue in element.XPathSelectElements(_returnPath))
            {
                var keyValues = returnValue.XPathSelectElements("./item")
                    .Select(ReadItemFromXml)
                    .Where(_ => _.Key == "id" || _.Key == "value")
                    .ToList();

                var id = keyValues.SingleOrDefault(_ => _.Key == "id");
                var value = keyValues.SingleOrDefault(_ => _.Key == "value");

                values.Add(new LookupValue { Id = id.Value, Value = value.Value });
            }

            Values = values;
        }
    }
}