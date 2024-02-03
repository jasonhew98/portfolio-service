using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MediatR;
using Api.Seedwork;
using System.Net;

namespace Api.Features.Health
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public HealthController(
            ILogger<HealthController> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> Health()
        {
            return Ok("Ok");
        }
    }
}
