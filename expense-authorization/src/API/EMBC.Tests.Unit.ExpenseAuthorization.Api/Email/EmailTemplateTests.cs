using System;
using AutoFixture;
using EMBC.ExpenseAuthorization.Api.Email;
using EMBC.ExpenseAuthorization.Api.ETeam.Responses;
using EMBC.ExpenseAuthorization.Api.Models;
using Xunit;
using Xunit.Abstractions;

namespace EMBC.Tests.Unit.ExpenseAuthorization.Api.Email
{
    public class EmailTemplateTests
    {
        private readonly ITestOutputHelper _output;
        private readonly EmailTemplate _sut = new EmailTemplate();
        private readonly Fixture _fixture = new Fixture();

        public EmailTemplateTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void can_apply_resource_request_to_template()
        {
            var request = _fixture.Create<ExpenseAuthorizationRequest>();

            _sut.Apply(request);
            var actual = _sut.Content;

            _output.WriteLine(actual);

            Assert.Contains(request.Event, actual);
            Assert.Contains(request.DateTime.ToString(EmailTemplate.DateFormat), actual);
            Assert.Contains(request.EAFNo, actual);
            Assert.Contains(request.EMBCTaskNo, actual);
            Assert.Contains(request.RequestingOrg, actual);
            Assert.Contains(request.ResourceType, actual);

            Assert.Contains(request.AuthName, actual);
            Assert.Contains(request.AuthTelephone, actual);
            Assert.Contains(request.AuthEmail, actual);

            Assert.Contains(request.Description, actual);

            Assert.Contains(request.AmountRequested.ToString(), actual);
            Assert.Contains(request.ExpenditureNotToExceed.ToString(), actual);

            Assert.Contains(request.EocApprovals.Processing.ApprovedBy, actual);
            Assert.Contains(request.EocApprovals.Processing.Position, actual);
            Assert.Contains(request.EocApprovals.Processing.ApprovalDateTime.ToString(EmailTemplate.DateFormat), actual);

            Assert.Contains(request.EocApprovals.ExpenditureRequest.ApprovedBy, actual);
            Assert.Contains(request.EocApprovals.ExpenditureRequest.Position, actual);
            Assert.Contains(request.EocApprovals.ExpenditureRequest.ApprovalDateTime.ToString(EmailTemplate.DateFormat), actual);
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
