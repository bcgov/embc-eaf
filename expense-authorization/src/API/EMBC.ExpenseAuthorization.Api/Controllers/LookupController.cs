using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMBC.ExpenseAuthorization.Api.ETeam;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EMBC.ExpenseAuthorization.Api.Controllers
{
    /// <summary></summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LookupController : ControllerBase
    {
        private readonly IETeamSoapService _service;
        private readonly ILogger<LookupController> _logger;


        /// <summary>Initializes a new instance of the <see cref="LookupController" /> class.</summary>
        /// <param name="service">The service.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">service
        /// or
        /// logger</exception>
        public LookupController(IETeamSoapService service, ILogger<LookupController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lookupType"></param>
        /// <returns></returns>
        [HttpGet("{lookupType}")]
        [ProducesResponseType(typeof(List<LookupValue>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync([FromRoute]LookupType lookupType)
        {
            // By annotating the controller with ApiControllerAttribute,
            // the ModelStateInvalidFilter will automatically check ModelState.IsValid
            // see https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.1#automatic-http-400-responses

            try
            {
                IList<LookupValue> values = Array.Empty<LookupValue>();

                switch (lookupType)
                {
                    case LookupType.PriorityResource:
                    case LookupType.StatusResource:
                        values = await _service.GetPicklistColorsAsync(lookupType);
                        break;

                    default:
                        values = await _service.GetPicklistKeywordsAsync(lookupType);
                        break;

                }

                return Ok(values);
            }
            catch (Refit.ApiException exception)
            {
                // create an error instance id to correlate the log error message with the problem details returned to 
                // caller
                var errorInstanceId = Guid.NewGuid().ToString("d");

                _logger.LogWarning(exception, "Error getting lookup values for {LookupType}, Error Message: {ErrorResponse}, Status Code = {HttpStatusCode}, Error Id: {ErrorInstanceId}",
                    lookupType,
                    exception.Content, 
                    exception.StatusCode,
                    errorInstanceId);

                var problem = new ProblemDetails
                {
                    Detail = "E-Teams web service was not successful",
                    Instance = errorInstanceId,
                    Status = (int)exception.StatusCode
                };

                // throw the error to the caller
                return StatusCode(500, problem);
            }
        }
    }
}
