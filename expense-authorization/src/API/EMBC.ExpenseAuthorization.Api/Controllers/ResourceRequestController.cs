using System;
using System.Threading.Tasks;
using EMBC.ExpenseAuthorization.Api.Features;
using EMBC.ExpenseAuthorization.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace EMBC.ExpenseAuthorization.Api.Controllers
{
    /// <summary></summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseAuthorizationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="logger"></param>
        public ExpenseAuthorizationController(IMediator mediator, ILogger logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates an Expense Authorization Request.
        /// </summary>
        /// <param name="request">The expense authorization request to create.</param>
        /// <param name="files">The optional list of files to attach to the request.</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostAsync([FromForm] ExpenseAuthorizationRequest request, [FromForm] IFormFileCollection files)
        {
            // By annotating the controller with ApiControllerAttribute,
            // the ModelStateInvalidFilter will automatically check ModelState.IsValid
            // see https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.1#automatic-http-400-responses

            try
            {
                //var command = new ResourceRequest.CreateCommand(resourceRequest, files);
                //ResourceRequest.CreateResponse response = await _mediator.Send(command);
                
                return Ok();
            }
            catch (Exception e)
            {
                // create an error instance id to correlate the log error message with the problem details returned to 
                // caller
                var errorInstanceId = Guid.NewGuid().ToString("d");

                _logger.Warning(e, "An error occured while processing the request Error Id: {ErrorInstanceId}", errorInstanceId);
                return Problem(e.Message, errorInstanceId);
            }
        }
    }
}
