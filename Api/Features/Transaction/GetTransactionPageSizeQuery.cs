using Api.Infrastructure.Authorization;
using Api.Model;
using Api.Seedwork;
using CSharpFunctionalExtensions;
using Domain.AggregatesModel.TransactionAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Features.Transaction
{
    public class GetTransactionPageSizeQuery : IRequest<Result<PageSizeDto, CommandErrorResponse>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string MainCategory { get; set; }
        public string SubCategory { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public decimal? StartPaymentAmount { get; set; }
        public decimal? EndPaymentAmount { get; set; }
        public int PageSize { get; set; }

        public GetTransactionPageSizeQuery(
            int pageSize,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string mainCategory = null,
            string subCategory = null,
            PaymentMethod? paymentMethod = null,
            decimal? startPaymentAmount = null,
            decimal? endPaymentAmount = null)
        {
            PageSize = pageSize;
            StartDate = startDate;
            EndDate = endDate;
            MainCategory = mainCategory;
            SubCategory = subCategory;
            PaymentMethod = paymentMethod;
            StartPaymentAmount = startPaymentAmount;
            EndPaymentAmount = endPaymentAmount;
        }
    }

    public class GetTransactionPageSizeQueryHandler : IRequestHandler<GetTransactionPageSizeQuery, Result<PageSizeDto, CommandErrorResponse>>
    {
        private readonly ILogger _logger;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICurrentUserAccessor _currentUser;

        public GetTransactionPageSizeQueryHandler(
            ILogger<GetTransactionPageSizeQueryHandler> logger,
            ITransactionRepository transactionRepository,
            ICurrentUserAccessor currentUser)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
            _currentUser = currentUser;
        }

        public async Task<Result<PageSizeDto, CommandErrorResponse>> Handle(GetTransactionPageSizeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUser.UserId;

                var count = await _transactionRepository.GetTransactionCount(
                    userId: userId,
                    startDate: request.StartDate,
                    endDate: request.EndDate,
                    mainCategory: request.MainCategory,
                    subCategory: request.SubCategory,
                    paymentMethod: request.PaymentMethod.HasValue ? Enum.GetName(typeof(PaymentMethod), request.PaymentMethod) : null,
                    startPaymentAmount: request.StartPaymentAmount,
                    endPaymentAmount: request.EndPaymentAmount);

                var result = new PageSizeDto(count, request.PageSize);

                return ResultYm.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "An error has occured while trying to get transaction page size");
                return ResultYm.Error<PageSizeDto>(ex);
            }
        }
    }
}
