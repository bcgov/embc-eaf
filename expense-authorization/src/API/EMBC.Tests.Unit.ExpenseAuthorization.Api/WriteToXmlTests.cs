using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using EMBC.ExpenseAuthorization.Api.ETeam;
using EMBC.ExpenseAuthorization.Api.ETeam.Requests;
using Xunit;

namespace EMBC.Tests.Unit.ExpenseAuthorization.Api
{
    public class WriteToXmlTests
    {
        [Fact]
        public void WriteToXml_generates_valid_xml()
        {
            GetPicklistKeywordsRequest sut = new GetPicklistKeywordsRequest(LookupType.LeadAgencyDeptList);

            var xml = sut.CreateSoapRequest();
        }

        [Fact]
        public void X()
        {
            Dictionary<string,string> items = new Dictionary<string, string>();
            items.Add("reportType", "resource_request");

            CreaterReportRequest sut = new CreaterReportRequest(items);

            var xml = sut.CreateSoapRequest();

        }

        private void AssertValidXml(string xml)
        {
            XDocument.Parse(xml);
        }
    }
}
