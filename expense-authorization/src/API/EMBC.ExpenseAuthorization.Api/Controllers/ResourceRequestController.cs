using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMBC.ExpenseAuthorization.Api.ETeam.Models;
using EMBC.ExpenseAuthorization.Api.Features;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace EMBC.ExpenseAuthorization.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceRequestController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="logger"></param>
        public ResourceRequestController(IMediator mediator, ILogger logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a Resource Request..
        /// </summary>
        /// <param name="resourceRequest">The resource request to create.</param>
        /// <param name="files">The optional list of files to attach to the request.</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostAsync([FromForm]ResourceRequestModel resourceRequest, [FromForm] IList<IFormFile> files)
        {
            // By annotating the controller with ApiControllerAttribute,
            // the ModelStateInvalidFilter will automatically check ModelState.IsValid
            // see https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.1#automatic-http-400-responses

            try
            {
                var command = new ResourceRequest.CreateCommand(resourceRequest, files);
                ResourceRequest.CreateResponse response = await _mediator.Send(command);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.Warning(e, "An error occured while processing the request");
                return Problem(e.Message);
            }
        }
    }
}
