using Api.Infrastructure.Authorization;
using Api.Seedwork;
using CSharpFunctionalExtensions;
using Domain.AggregatesModel.TransactionAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TransactionModel = Domain.AggregatesModel.TransactionAggregate.Transaction;

namespace Api.Features.Transaction
{
    public class GetTransactionsQuery : IRequest<Result<List<TransactionDto>, CommandErrorResponse>>
    {
        public string SortBy { get; set; }
        public int SortOrder { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }

        public GetTransactionsQuery(
            string sortBy,
            int sortOrder,
            int pageSize,
            int currentPage)
        {
            SortBy = sortBy;
            SortOrder = sortOrder;
            PageSize = pageSize;
            CurrentPage = currentPage;
        }
    }

    public class TransactionDto
    {
        public string TransactionId { get; set; }
        public string MainCategory { get; set; }
        public string SubCategory { get; set; }
        public DateTime TransactionDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public double PaymentAmount { get; set; }
        public DateTime? ModifiedUTCDateTime { get; set; }

        public static TransactionDto CreateFromDomain(TransactionModel transaction)
        {
            return new TransactionDto
            {
                TransactionId = transaction.TransactionId,
                MainCategory = transaction.MainCategory,
                SubCategory = transaction.SubCategory,
                TransactionDate = transaction.TransactionDate,
                PaymentMethod = transaction.PaymentMethod,
                PaymentAmount = transaction.PaymentAmount,
                ModifiedUTCDateTime = transaction.ModifiedUTCDateTime
            };
        }
    }

    public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, Result<List<TransactionDto>, CommandErrorResponse>>
    {
        private readonly ILogger _logger;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICurrentUserAccessor _currentUser;

        public GetTransactionsQueryHandler(
            ILogger<GetTransactionsQueryHandler> logger,
            ITransactionRepository transactionRepository,
            ICurrentUserAccessor currentUser)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
            _currentUser = currentUser;
        }

        public async Task<Result<List<TransactionDto>, CommandErrorResponse>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUser.UserId;

                var transactions = await _transactionRepository.GetTransactions(
                    limit: request.PageSize,
                    offset: request.PageSize * (request.CurrentPage < 1 ? 0 : request.CurrentPage - 1),
                    sortBy: request.SortBy,
                    sortOrder: request.SortOrder,
                    userId: userId);

                if (transactions == null)
                    return ResultYm.Success(new List<TransactionDto>());

                var result = transactions.Select(p => TransactionDto.CreateFromDomain(p)).ToList();

                return ResultYm.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "An error has occured while trying to get transaction list.");
                return ResultYm.Error<List<TransactionDto>>(ex);
            }
        }
    }
}
