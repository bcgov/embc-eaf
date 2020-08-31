using System;
using System.Collections.Generic;
using System.Text;
using AutoFixture;
using EMBC.ExpenseAuthorization.Api.Models;
using Xunit;

namespace EMBC.Tests.Unit.ExpenseAuthorization.Api.Models
{
    public class ExpenseAuthorizationRequestTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void AmountRequested_rounds_up()
        {
            ExpenseAuthorizationRequest sut = new ExpenseAuthorizationRequest();

            sut.AmountRequested = _fixture.Create<int>() + 0.01m;
            var expected = Math.Ceiling(sut.AmountRequested); // expect to be round up

            Assert.Equal(expected, sut.AmountRequested);
        }


        [Fact]
        public void ExpenditureNotToExceed_rounds_up()
        {
            ExpenseAuthorizationRequest sut = new ExpenseAuthorizationRequest();

            sut.ExpenditureNotToExceed = _fixture.Create<int>() + 0.01m;
            var expected = Math.Ceiling(sut.ExpenditureNotToExceed); // expect to be round up

            Assert.Equal(expected, sut.ExpenditureNotToExceed);
        }

    }
}
