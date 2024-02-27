using Api.Infrastructure.Authorization;
using Api.Model;
using Api.Seedwork;
using CSharpFunctionalExtensions;
using Domain.AggregatesModel.TransactionAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Features.Transaction
{
    public class GetTransactionPageSizeQuery : IRequest<Result<PageSizeDto, CommandErrorResponse>>
    {
        public int PageSize { get; set; }

        public GetTransactionPageSizeQuery(
            int pageSize)
        {
            PageSize = pageSize;
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

                var count = await _transactionRepository.GetTransactionCount(userId: userId);

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
