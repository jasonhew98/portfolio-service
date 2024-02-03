using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MediatR;
using Api.Seedwork;
using System.Net;
using Microsoft.Extensions.Options;
using Api.Infrastructure;

namespace Api.Features.Health
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IOptions<PortfolioRepositoryOptions> _portfolioRepositoryOptions;

        public HealthController(
            ILogger<HealthController> logger,
            IOptions<PortfolioRepositoryOptions> portfolioRepositoryOptions,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _portfolioRepositoryOptions = portfolioRepositoryOptions;
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> Health()
        {
            return Ok("Ok");
        }

        [HttpGet]
        [Route("configuration")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> Configuration()
        {
            return Ok(_portfolioRepositoryOptions.Value.MongoDbUrl);
        }
    }
}
