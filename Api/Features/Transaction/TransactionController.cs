using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MediatR;
using Api.Seedwork;
using System.Net;
using Api.Model;
using Api.Infrastructure.Authorization;

namespace Api.Features.Transaction
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public TransactionController(
            ILogger<TransactionController> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> AddTransaction([FromBody] AddTransactionCommand command)
        {
            return this.OkOrError(await _mediator.Send(command));
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(TransactionDto[]), (int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] string sortBy,
            [FromQuery] int sortOrder,
            [FromQuery] int currentPage,
            [FromQuery] int pageSize)
        {
            var command = new GetTransactionsQuery(
                sortBy: sortBy,
                sortOrder: sortOrder,
                currentPage: currentPage,
                pageSize: pageSize);

            return this.OkOrError(await _mediator.Send(command));
        }

        [HttpGet]
        [Route("pageSize")]
        [Authorize]
        [ProducesResponseType(typeof(PageSizeDto), (int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> GetTransactionPageSize(
            [FromQuery] int pageSize)
        {
            var command = new GetTransactionPageSizeQuery(
                pageSize: pageSize);

            return this.OkOrError(await _mediator.Send(command));
        }

        [HttpGet]
        [Route("detail")]
        [ProducesResponseType(typeof(TransactionDetailDto), (int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> GetTransaction(
            [FromQuery] string transactionId)
        {
            var command = new GetTransactionQuery(
                transactionId: transactionId);

            return this.OkOrError(await _mediator.Send(command));
        }

        [HttpPatch]
        [Authorize]
        [ProducesResponseType(typeof(UpdatedTransactionDto), (int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> UpdateTransaction(
            [FromBody] UpdateTransactionCommand command)
        {
            return this.OkOrError(await _mediator.Send(command));
        }

        [HttpDelete]
        [Authorize]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> DeleteTransaction(
            [FromBody] DeleteTransactionCommand command)
        {
            return this.OkOrError(await _mediator.Send(command));
        }
    }
}
