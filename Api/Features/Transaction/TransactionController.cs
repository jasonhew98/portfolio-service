using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MediatR;
using Api.Seedwork;
using System.Net;
using Api.Model;
using Api.Infrastructure.Authorization;
using System;
using Domain.AggregatesModel.TransactionAggregate;

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
            [FromQuery] int pageSize,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string mainCategory = null,
            [FromQuery] string subCategory = null,
            [FromQuery] PaymentMethod? paymentMethod = null,
            [FromQuery] double? startPaymentAmount = null,
            [FromQuery] double? endPaymentAmount = null)
        {
            var command = new GetTransactionsQuery(
                sortBy: sortBy,
                sortOrder: sortOrder,
                currentPage: currentPage,
                pageSize: pageSize,
                startDate: startDate,
                endDate: endDate,
                mainCategory: mainCategory,
                subCategory: subCategory,
                paymentMethod: paymentMethod,
                startPaymentAmount: startPaymentAmount,
                endPaymentAmount: endPaymentAmount);

            return this.OkOrError(await _mediator.Send(command));
        }

        [HttpGet]
        [Route("pageSize")]
        [Authorize]
        [ProducesResponseType(typeof(PageSizeDto), (int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> GetTransactionPageSize(
            [FromQuery] int pageSize,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string mainCategory = null,
            [FromQuery] string subCategory = null,
            [FromQuery] PaymentMethod? paymentMethod = null,
            [FromQuery] double? startPaymentAmount = null,
            [FromQuery] double? endPaymentAmount = null)
        {
            var command = new GetTransactionPageSizeQuery(
                pageSize: pageSize,
                startDate: startDate,
                endDate: endDate,
                mainCategory: mainCategory,
                subCategory: subCategory,
                paymentMethod: paymentMethod,
                startPaymentAmount: startPaymentAmount,
                endPaymentAmount: endPaymentAmount);

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
        [Route("{transactionId}")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> DeleteTransaction(string transactionId)
        {
            var command = new DeleteTransactionCommand(
                transactionId: transactionId);
            return this.OkOrError(await _mediator.Send(command));
        }
    }
}
