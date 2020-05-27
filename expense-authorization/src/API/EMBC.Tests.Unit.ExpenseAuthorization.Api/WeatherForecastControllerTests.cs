using System;
using System.Collections.Generic;
using System.Linq;
using EMBC.ExpenseAuthorization.Api;
using EMBC.ExpenseAuthorization.Api.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EMBC.Tests.Unit.ExpenseAuthorization.Api
{
    public class WeatherForecastControllerTests
    {
        private readonly Mock<ILogger<WeatherForecastController>> _mockLogger;

        public WeatherForecastControllerTests()
        {
            _mockLogger = new Mock<ILogger<WeatherForecastController>>();
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullExceptionWhenLoggerIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new WeatherForecastController(null));
            Assert.Equal("logger", exception.ParamName);
        }

        [Fact]
        public void Index_ReturnsNextFiveDaysOfValidForecasts()
        {
            // Arrange
            WeatherForecastController controller = new WeatherForecastController(_mockLogger.Object);
            DateTime today = DateTime.Today;

            // Act
            IEnumerable<WeatherForecast> result = controller.Get();

            // Assert
            Assert.NotNull(result);

            var forecasts = result.ToList();

            int days = 1;

            // local function to assert the weather forecast is correct
            void ForecastAssertion(WeatherForecast forecast)
            {
                Assert.NotNull(forecast);
                // there is a small chance this could fail of the unit test started just before midnight
                // and the controller.Get() call executes just after, then the days would be off by 1
                Assert.Equal(today.AddDays(days++), forecast.Date);
                Assert.True(-20 <= forecast.TemperatureC && forecast.TemperatureC < 55);
                Assert.NotNull(forecast.Summary);
            }

            // we expect 5 days of forecasts
            Assert.Collection(forecasts,
                /*1*/ForecastAssertion,
                /*2*/ForecastAssertion,
                /*3*/ForecastAssertion,
                /*4*/ForecastAssertion,
                /*5*/ForecastAssertion
                );
        }
    }
}
