using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMBC.ExpenseAuthorization.Api.Email;
using EMBC.ExpenseAuthorization.Api.ETeam;
using FlatFiles;
using FlatFiles.TypeMapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

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

        /// <summary>Gets the asynchronous.</summary>
        /// <returns></returns>
        [HttpGet("ExpenditureAuthorizationResourceTypes")]
        [ProducesResponseType(typeof(List<LookupValue>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync()
        {
            // By annotating the controller with ApiControllerAttribute,
            // the ModelStateInvalidFilter will automatically check ModelState.IsValid
            // see https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.1#automatic-http-400-responses

            try
            {
                var values = await _service.GetExpenditureAuthorizationResourceTypesAsync();
                return Ok(values);
            }
            catch (Refit.ApiException exception)
            {
                // create an error instance id to correlate the log error message with the problem details returned to 
                // caller
                var errorInstanceId = Guid.NewGuid().ToString("d");

                _logger.LogWarning(exception,
                    "Error getting lookup values for Expenditure Authorization Resource Types, Error Message: {ErrorResponse}, Status Code = {HttpStatusCode}, Error Id: {ErrorInstanceId}",
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lookupType"></param>
        /// <returns></returns>
        [HttpGet("{lookupType}")]
        [ProducesResponseType(typeof(List<LookupValue>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync([FromRoute] LookupType lookupType)
        {
            // By annotating the controller with ApiControllerAttribute,
            // the ModelStateInvalidFilter will automatically check ModelState.IsValid
            // see https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.1#automatic-http-400-responses

            try
            {
                IList<LookupValue> values = await _service.GetLookupAsync(lookupType);

                if (lookupType == LookupType.LeadAgencyDeptList)
                {
                    // LeadAgencyDeptList requires just values and not internal id
                    foreach (var value in values)
                    {
                        value.Id = value.Value;
                    }
                }

                return Ok(values);
            }
            catch (Refit.ApiException exception)
            {
                // create an error instance id to correlate the log error message with the problem details returned to 
                // caller
                var errorInstanceId = Guid.NewGuid().ToString("d");

                _logger.LogWarning(exception,
                    "Error getting lookup values for {LookupType}, Error Message: {ErrorResponse}, Status Code = {HttpStatusCode}, Error Id: {ErrorInstanceId}",
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
            catch (Exception exception)
            {
                // create an error instance id to correlate the log error message with the problem details returned to 
                // caller
                var errorInstanceId = Guid.NewGuid().ToString("d");

                _logger.LogWarning(exception,
                    "Error getting lookup values for {LookupType}, Error Id: {ErrorInstanceId}",
                    lookupType,
                    errorInstanceId);

                var problem = new ProblemDetails
                {
                    Detail = "E-Teams web service was not successful", Instance = errorInstanceId, Status = 500
                };

                // throw the error to the caller
                return StatusCode(500, problem);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("LeadAgencyDeptListCsv")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetLeadAgencyDeptListAsCsvAsync()
        {
            // By annotating the controller with ApiControllerAttribute,
            // the ModelStateInvalidFilter will automatically check ModelState.IsValid
            // see https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.1#automatic-http-400-responses

            IList<LookupValue> lookupValues = await _service
                .GetLookupAsync(LookupType.LeadAgencyDeptList);

            var values = lookupValues.Select(_ => new CommunityEmailRecipient
            {
                Community = _.Value,
                To = string.Empty,
                Cc = string.Empty,
                Bcc = string.Empty
            });

            var mapper = SeparatedValueTypeMapper.Define<CommunityEmailRecipient>();
            mapper.Property(c => c.Community).ColumnName("community");
            mapper.Property(c => c.To).ColumnName("to");
            mapper.Property(c => c.Cc).ColumnName("cc");
            mapper.Property(c => c.Bcc).ColumnName("bcc");

            StringBuilder buffer = new StringBuilder();
            StringWriter writer = new StringWriter(buffer);

            await mapper.WriteAsync(writer, values, new SeparatedValueOptions { IsFirstRecordSchema = true });

            return new FileContentResult(Encoding.UTF8.GetBytes(buffer.ToString()), new MediaTypeHeaderValue("text/csv"));
        }
    }
}
