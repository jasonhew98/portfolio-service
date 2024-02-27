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

namespace Api.Features.Transaction
{
    public class DeleteTransactionCommand : IRequest<Result<bool, CommandErrorResponse>>
    {
        public string TransactionId { get; set; }
    }

    public class DeleteTransactionCommandValidator : AbstractValidator<DeleteTransactionCommand>
    {
        public DeleteTransactionCommandValidator()
        {
            RuleFor(a => a.TransactionId).NotEmpty();
        }
    }

    public class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand, Result<bool, CommandErrorResponse>>
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICurrentUserAccessor _currentUser;

        public DeleteTransactionCommandHandler(
            ILogger<DeleteTransactionCommandHandler> logger,
            IUnitOfWork unitOfWork,
            ITransactionRepository transactionRepository,
            ICurrentUserAccessor currentUser)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _transactionRepository = transactionRepository;
            _currentUser = currentUser;
        }

        public async Task<Result<bool, CommandErrorResponse>> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUser.UserId;
                var username = _currentUser.UserName;

                var transaction = await _transactionRepository.GetTransaction(
                    transactionId: request.TransactionId,
                    userId: userId);

                if (transaction == null)
                    return ResultYm.NotFound<bool>("Transaction not found.");

                _transactionRepository.Remove(x => x.TransactionId == request.TransactionId);

                await _unitOfWork.Commit();

                return ResultYm.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unknown error has occured while trying to delete transaction.");
                return ResultYm.Error<bool>(ex);
            }
        }
    }
}
