using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MediatR;
using Api.Seedwork;
using System.Net;

namespace Api.Features.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public AuthController(
            ILogger<AuthController> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        [Route("signup")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> SignUp([FromBody] SignUpCommand command)
        {
            return this.OkOrError(await _mediator.Send(command));
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            return this.OkOrError(await _mediator.Send(command));
        }
    }
}
