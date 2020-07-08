﻿using System.Threading;
using System.Threading.Tasks;
using EMBC.ExpenseAuthorization.Api.Controllers;
using EMBC.ExpenseAuthorization.Api.ETeam.Models;
using EMBC.ExpenseAuthorization.Api.Features;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Serilog;
using Xunit;

namespace EMBC.Tests.Unit.ExpenseAuthorization.Api.Controllers
{
    public class ResourceRequestControllerTests
    {
        [Fact]
        public async Task Foo()
        {
            Mock<IMediator> mockMediator = new Mock<IMediator>(MockBehavior.Strict);
            Mock<ILogger> mockLogger = new Mock<ILogger>();

            var response = new ResourceRequest.CreateResponse();

            mockMediator
                .Setup(m => m.Send(It.IsAny<ResourceRequest.CreateCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var sut = new ResourceRequestController(mockMediator.Object, mockLogger.Object);

            var actual = await sut.PostAsync(new ResourceRequestModel());

            Assert.IsType<OkResult>(actual);

            // Verify all verifiable expectations on all mocks created through the mediator
            mockMediator.Verify(mock => mock.Send(It.IsAny<ResourceRequest.CreateCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
