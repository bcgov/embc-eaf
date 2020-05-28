using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMBC.ExpenseAuthorization.Api.ETeam;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EMBC.ExpenseAuthorization.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupController : ControllerBase
    {
        private readonly IETeamSoapService _service;

        public LookupController(IETeamSoapService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }
        
        [HttpGet("{lookupType}")]
        [ProducesResponseType(typeof(List<LookupValue>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync([FromRoute]LookupType lookupType)
        {
            var values = await _service.GetPicklistKeywords(lookupType);
            return Ok(values);
        }
    }
}
