using Api.Seedwork;
using CSharpFunctionalExtensions;
using Domain.AggregatesModel.TransactionAggregate;
using FluentValidation;
using Api.Infrastructure.Seedwork;
using Api.Infrastructure.Authorization;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TransactionModel = Domain.AggregatesModel.TransactionAggregate.Transaction;

namespace Api.Features.Transaction
{
    public class AddTransactionCommand : IRequest<Result<bool, CommandErrorResponse>>
    {
        public string MainCategory { get; set; }
        public string SubCategory { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Notes { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal PaymentAmount { get; set; }
    }

    public class AddTransactionCommandValidator : AbstractValidator<AddTransactionCommand>
    {
        public AddTransactionCommandValidator()
        {
            RuleFor(a => a.MainCategory).NotEmpty();
            RuleFor(a => a.SubCategory).NotEmpty();
            RuleFor(a => a.TransactionDate).NotEmpty();
            RuleFor(a => a.PaymentMethod).NotNull().IsInEnum();
            RuleFor(a => a.PaymentAmount).NotEmpty().GreaterThan(0);
        }
    }

    public class AddTransactionCommandHandler : IRequestHandler<AddTransactionCommand, Result<bool, CommandErrorResponse>>
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICurrentUserAccessor _currentUser;

        public AddTransactionCommandHandler(
            ILogger<AddTransactionCommandHandler> logger,
            IUnitOfWork unitOfWork,
            ITransactionRepository transactionRepository,
            ICurrentUserAccessor currentUser)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _transactionRepository = transactionRepository;
            _currentUser = currentUser;
        }

        public async Task<Result<bool, CommandErrorResponse>> Handle(AddTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUser.UserId;
                var username = _currentUser.UserName;

                var transaction = new TransactionModel(
                    transactionId: Guid.NewGuid().ToString("N"),
                    mainCategory: request.MainCategory,
                    subCategory: request.SubCategory,
                    transactionDate: request.TransactionDate,
                    notes: request.Notes,
                    paymentMethod: request.PaymentMethod,
                    paymentAmount: request.PaymentAmount,
                    createdBy: userId,
                    createdByName: username,
                    createdUTCDateTime: DateTime.Now,
                    modifiedBy: userId,
                    modifiedByName: username,
                    modifiedUTCDateTime: DateTime.Now);

                _transactionRepository.Add(transaction);

                await _unitOfWork.Commit();

                return ResultYm.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unknown error has occured while trying to add transaction.");
                return ResultYm.Error<bool>(ex);
            }
        }
    }
}
