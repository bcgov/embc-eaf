using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;
using AutoFixture;
using EMBC.ExpenseAuthorization.Api.ETeam;
using EMBC.ExpenseAuthorization.Api.ETeam.Models;
using Moq;
using Refit;
using Xunit;

namespace EMBC.Tests.Unit.ExpenseAuthorization.Api
{
    public class ETeamServiceTests
    {
        private readonly IFixture _fixture = new Fixture();

        [Fact]
        public async Task CreateReportAsync_extension_method_creates_dictionary_and_calls_service_method()
        {
            // Arrange
            Mock<IETeamRestService> mockService = new Mock<IETeamRestService>();

            mockService.Setup(_ => _.CreateReportAsync(It.IsAny<Dictionary<string, object>>()));

            ResourceRequestModel resourceRequest = _fixture.Create<ResourceRequestModel>();
            string username = _fixture.Create<string>();
            string password = _fixture.Create<string>();
            string reportTypeName = _fixture.Create<string>();

            // Act
            await mockService.Object.CreateReportAsync(username, password, reportTypeName, resourceRequest);

            // Assert

            // The CreateReportAsync method should be called once
            mockService.Verify(_ => _.CreateReportAsync(It.IsAny<Dictionary<string, object>>()), Times.Once);
        }
    }


    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _response;
        private readonly HttpStatusCode _statusCode;

        public string Content { get; private set; }

        public int NumberOfCalls { get; private set; }

        public MockHttpMessageHandler(string response, HttpStatusCode statusCode)
        {
            _response = response;
            _statusCode = statusCode;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            NumberOfCalls++;
            if (request.Content != null) // Could be a GET-request without a body
            {
                Content = await request.Content.ReadAsStringAsync();
            }

            return new HttpResponseMessage
            {
                StatusCode = _statusCode,
                Content = new StringContent(_response)
            };
        }
    }
}
