using Api.Seedwork;
using Api.Seedwork.AesEncryption;
using CSharpFunctionalExtensions;
using Domain;
using Domain.AggregatesModel.UserAggregate;
using FluentValidation;
using Api.Infrastructure.Seedwork;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Features.Authentication
{
    public class SignUpCommand : IRequest<Result<bool, CommandErrorResponse>>
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class SignUpCommandValidator : AbstractValidator<SignUpCommand>
    {
        public SignUpCommandValidator()
        {
            RuleFor(a => a.Email).NotEmpty();
            RuleFor(a => a.FirstName).NotEmpty();
            RuleFor(a => a.LastName).NotEmpty();
            RuleFor(a => a.UserName).NotEmpty();
            RuleFor(a => a.Password).NotEmpty();
        }
    }

    public class SignUpCommandHandler : IRequestHandler<SignUpCommand, Result<bool, CommandErrorResponse>>
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IAesSecurity _aes;

        public SignUpCommandHandler(
            ILogger<SignUpCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IAesSecurity aes,
            IUserRepository userRepository)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _aes = aes;
            _userRepository = userRepository;
        }

        public async Task<Result<bool, CommandErrorResponse>> Handle(SignUpCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.QueryOne(x => x.UserName == request.UserName || x.Email == _aes.Encrypt(request.Email));
                if (user != null)
                    return ResultYm.Error<bool>(CommandErrorResponse.BusinessError(BusinessError.FailToSignUp__UserAlreadyExist.Error()));

                user = new Domain.AggregatesModel.UserAggregate.User(
                    userId: Guid.NewGuid().ToString("N"),
                    userName: request.UserName,
                    firstName: request.FirstName,
                    lastName: request.LastName,
                    password: _aes.Encrypt(request.Password),
                    email: _aes.Encrypt(request.Email),
                    createdBy: "System",
                    createdByName: "System",
                    createdUTCDateTime: DateTime.Now,
                    modifiedBy: "System",
                    modifiedByName: "System",
                    modifiedUTCDateTime: DateTime.Now);

                _userRepository.Add(user);

                await _unitOfWork.Commit();

                return ResultYm.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unknown error has occured while trying to sign up.");
                return ResultYm.Error<bool>(ex);
            }
        }
    }
}
