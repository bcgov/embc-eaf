using System.IO;
using EMBC.ExpenseAuthorization.Api.ETeam.Responses;
using Xunit;

namespace EMBC.Tests.Unit.ExpenseAuthorization.Api.ETeam.Responses
{
    public class CreateReportResponseTests
    {
        [Fact]
        public void ParseResponse()
        {

            string xml = EmbeddedResource.Get<CreateReportResponseTests>("CreateResponse.txt");
            CreateReportResponse sut = new CreateReportResponse();
            sut.LoadFromXml(xml);

            Assert.Equal("Control4222019144914-embcTraining-159742987459901082006", sut.Fields["reportId"]);
            Assert.Equal("Control4222019144914-embcTraining-159742987459901082006", sut.Fields["id"]);
            Assert.Equal("resource_request", sut.Fields["reportType"]);
        }
    }
}
