using EMBC.ExpenseAuthorization.Api.ETeam.Responses;
using Xunit;

namespace EMBC.Tests.Unit.ExpenseAuthorization.Api.ETeam.Responses
{
    public class GetPicklistKeywordsResponseTests
    {
        [Fact]
        public void ParseResponse()
        {
            string xml = EmbeddedResource.Get<GetPicklistKeywordsResponseTests>("GetPicklistKeywordsResponse.txt");

            GetPicklistKeywordsResponse sut = new GetPicklistKeywordsResponse();
            sut.LoadFromXml(xml);

            Assert.NotEmpty(sut.Values);
        }
    }
}
