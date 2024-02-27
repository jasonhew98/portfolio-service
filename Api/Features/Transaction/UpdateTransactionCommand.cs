using Api.Infrastructure.Authorization;
using Api.Seedwork;
using CSharpFunctionalExtensions;
using Domain;
using Domain.AggregatesModel.TransactionAggregate;
using FluentValidation;
using Api.Infrastructure.Seedwork;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Features.Transaction
{
    public class UpdateTransactionCommand : IRequest<Result<UpdatedTransactionDto, CommandErrorResponse>>
    {
        public string TransactionId { get; set; }
        public string MainCategory { get; set; }
        public string SubCategory { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Notes { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public double PaymentAmount { get; set; }
        public DateTime ModifiedUTCDateTime { get; set; }
    }

    public class UpdatedTransactionDto
    {
        public string TransactionId { get; }

        public UpdatedTransactionDto(
            string transactionId)
        {
            TransactionId = transactionId;
        }
    }

    public class UpdateTransactionCommandValidator : AbstractValidator<UpdateTransactionCommand>
    {
        public UpdateTransactionCommandValidator()
        {
            RuleFor(a => a.TransactionId).NotEmpty();
            RuleFor(a => a.ModifiedUTCDateTime).NotNull().NotEmpty();
        }
    }

    public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, Result<UpdatedTransactionDto, CommandErrorResponse>>
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICurrentUserAccessor _currentUser;

        public UpdateTransactionCommandHandler(
            ILogger<UpdateTransactionCommandHandler> logger,
            IUnitOfWork unitOfWork,
            ITransactionRepository transactionRepository,
            ICurrentUserAccessor currentUser)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _transactionRepository = transactionRepository;
            _currentUser = currentUser;
        }

        public async Task<Result<UpdatedTransactionDto, CommandErrorResponse>> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUser.UserId;

                var transaction = await _transactionRepository.GetTransaction(transactionId: request.TransactionId, userId: userId);

                if (transaction == null)
                    return ResultYm.NotFound<UpdatedTransactionDto>("Transaction not found.");
                if (!request.ModifiedUTCDateTime.Equals(transaction.ModifiedUTCDateTime))
                    return ResultYm.Error<UpdatedTransactionDto>(CommandErrorResponse.BusinessError(BusinessError.ConcurrencyUpdate.Error()));

                transaction.UpdateTransactionDetails(
                    mainCategory: request.MainCategory,
                    subCategory: request.SubCategory,
                    transactionDate: request.TransactionDate,
                    notes: request.Notes,
                    paymentMethod: request.PaymentMethod,
                    paymentAmount: request.PaymentAmount
                );

                await _transactionRepository.UpdateTransaction(
                    transaction: transaction,
                    currentUser: _currentUser.Tuple(),
                    userId: userId);

                await _unitOfWork.Commit();

                return ResultYm.Success(new UpdatedTransactionDto(request.TransactionId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unknown error has occured while trying to update transaction details.");
                return ResultYm.Error<UpdatedTransactionDto>(ex);
            }
        }
    }
}
