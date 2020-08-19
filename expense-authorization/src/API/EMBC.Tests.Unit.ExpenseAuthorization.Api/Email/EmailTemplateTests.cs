using System;
using AutoFixture;
using EMBC.ExpenseAuthorization.Api.Email;
using EMBC.ExpenseAuthorization.Api.ETeam.Models;
using EMBC.ExpenseAuthorization.Api.ETeam.Responses;
using Xunit;

namespace EMBC.Tests.Unit.ExpenseAuthorization.Api.Email
{
    public class EmailTemplateTests
    {
        private readonly EmailTemplate _sut = new EmailTemplate();
        private readonly Fixture _fixture = new Fixture();
        
        [Fact]
        public void can_apply_resource_request_to_template()
        {
            var resourceRequest = _fixture.Create<ResourceRequestModel>();

            _sut.Apply(resourceRequest);
            var actual = _sut.Content;

            Assert.Contains(resourceRequest.ApprovedTime.ToString(EmailTemplate.DateFormat), actual);
        }


        [Fact]
        public void can_apply_create_report_response_to_template()
        {
            string id = _fixture.Create<string>();

            var createReportResponse = new CreateReportResponse();
            createReportResponse.Fields.Add("id", id);

            var url = _fixture.Create<Uri>();

            _sut.Apply(createReportResponse, url);
            var actual = _sut.Content;

            Assert.Contains(url.ToString(), actual);

        }


        //[Fact]
        //public void all_placeholders_are_removed_from_final_output()
        //{
        //    var createReportResponse = _fixture.Create<CreateReportResponse>();
        //    var url = _fixture.Create<Uri>();
        //    var resourceRequest = _fixture.Create<ResourceRequestModel>();

        //    // apply all the data
        //    _sut.Apply(createReportResponse, url);
        //    _sut.Apply(resourceRequest);

        //    var actual = _sut.Content;

        //    Assert.DoesNotContain("{{", actual);
        //    Assert.DoesNotContain("}}", actual);

        //}


    }
}
