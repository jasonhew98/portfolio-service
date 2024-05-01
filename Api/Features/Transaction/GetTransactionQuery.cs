using Api.Infrastructure.Authorization;
using Api.Seedwork;
using CSharpFunctionalExtensions;
using Domain.AggregatesModel.TransactionAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TransactionModel = Domain.AggregatesModel.TransactionAggregate.Transaction;

namespace Api.Features.Transaction
{
    public class GetTransactionQuery : IRequest<Result<TransactionDetailDto, CommandErrorResponse>>
    {
        public string TransactionId { get; set; }

        public GetTransactionQuery(
            string transactionId)
        {
            TransactionId = transactionId;
        }
    }

    public class TransactionDetailDto
    {
        public string TransactionId { get; set; }
        public string MainCategory { get; set; }
        public string SubCategory { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Notes { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime? ModifiedUTCDateTime { get; set; }

        public static TransactionDetailDto CreateFromDomain(TransactionModel transaction)
        {
            return new TransactionDetailDto
            {
                TransactionId = transaction.TransactionId,
                MainCategory = transaction.MainCategory,
                SubCategory = transaction.SubCategory,
                TransactionDate = transaction.TransactionDate,
                Notes = transaction.Notes,
                PaymentMethod = transaction.PaymentMethod,
                PaymentAmount = transaction.PaymentAmount,
                ModifiedUTCDateTime = transaction.ModifiedUTCDateTime
            };
        }
    }

    public class GetTransactionQueryHandler : IRequestHandler<GetTransactionQuery, Result<TransactionDetailDto, CommandErrorResponse>>
    {
        private readonly ILogger _logger;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICurrentUserAccessor _currentUser;

        public GetTransactionQueryHandler(
            ILogger<GetTransactionQueryHandler> logger,
            ITransactionRepository transactionRepository,
            ICurrentUserAccessor currentUser)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
            _currentUser = currentUser;
        }

        public async Task<Result<TransactionDetailDto, CommandErrorResponse>> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUser.UserId;

                var transaction = await _transactionRepository.GetTransaction(
                    transactionId: request.TransactionId,
                    userId: userId);

                if (transaction == null)
                    return ResultYm.NotFound<TransactionDetailDto>("Transaction not found.");

                var result = TransactionDetailDto.CreateFromDomain(transaction);

                return ResultYm.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "An error has occured while trying to get transaction detail.");
                return ResultYm.Error<TransactionDetailDto>(ex);
            }
        }
    }
}
