using Api.Infrastructure.Authorization;
using Api.Seedwork;
using CSharpFunctionalExtensions;
using Domain.AggregatesModel.TransactionAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
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
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string MainCategory { get; set; }
        public string SubCategory { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public double? StartPaymentAmount { get; set; }
        public double? EndPaymentAmount { get; set; }
        public string SortBy { get; set; }
        public int SortOrder { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }

        public GetTransactionsQuery(
            string sortBy,
            int sortOrder,
            int pageSize,
            int currentPage,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string mainCategory = null,
            string subCategory = null,
            PaymentMethod? paymentMethod = null,
            double? startPaymentAmount = null,
            double? endPaymentAmount = null)
        {
            SortBy = sortBy;
            SortOrder = sortOrder;
            PageSize = pageSize;
            CurrentPage = currentPage;
            StartDate = startDate;
            EndDate = endDate;
            MainCategory = mainCategory;
            SubCategory = subCategory;
            PaymentMethod = paymentMethod;
            StartPaymentAmount = startPaymentAmount;
            EndPaymentAmount = endPaymentAmount;
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

                var filter = GenerateFilter(request, userId);

                var transactions = await _transactionRepository.Aggregate<TransactionModel>(filter);

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

        public List<BsonDocument> GenerateFilter(GetTransactionsQuery request, string userId)
        {
            var pipeline = new List<BsonDocument>
            {
                new BsonDocument("$match", new BsonDocument("createdBy", userId)),
                new BsonDocument("$skip", request.PageSize * (request.CurrentPage < 1 ? 0 : request.CurrentPage - 1)),
                new BsonDocument("$limit", request.PageSize),
                new BsonDocument("$sort", new BsonDocument(request.SortBy, request.SortOrder)),
            };

            if (request.StartDate.HasValue)
                pipeline.Add(new BsonDocument("$match", new BsonDocument { { "transactionDate", new BsonDocument("$gte", request.StartDate) } }));

            if (request.EndDate.HasValue)
                pipeline.Add(new BsonDocument("$match", new BsonDocument { { "transactionDate", new BsonDocument("$lte", request.EndDate) } }));

            if (!string.IsNullOrEmpty(request.MainCategory))
                pipeline.Add(new BsonDocument("$match", new BsonDocument("mainCategory", request.MainCategory)));

            if (!string.IsNullOrEmpty(request.SubCategory))
                pipeline.Add(new BsonDocument("$match", new BsonDocument("subCategory", request.SubCategory)));

            if (request.PaymentMethod.HasValue)
                pipeline.Add(new BsonDocument("$match", new BsonDocument { { "paymentMethod", new BsonDocument("$regex", Enum.GetName(typeof(PaymentMethod), request.PaymentMethod.Value)) } }));

            if (request.StartPaymentAmount.HasValue)
                pipeline.Add(new BsonDocument("$match", new BsonDocument { { "paymentAmount", new BsonDocument("$gte", request.StartPaymentAmount.Value) } }));

            if (request.EndPaymentAmount.HasValue)
                pipeline.Add(new BsonDocument("$match", new BsonDocument { { "paymentAmount", new BsonDocument("$lte", request.EndPaymentAmount.Value) } }));

            return pipeline;
        }
    }
}
