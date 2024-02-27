using Api.Infrastructure.Authorization;
using Api.Model;
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
    public class LoginCommand : IRequest<Result<UserDto, CommandErrorResponse>>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(a => a.UserName).NotEmpty();
            RuleFor(a => a.Password).NotEmpty();
        }
    }

    public class UserDto
    {
        public JwtTokenDto JwtToken { get; set; }

        public UserDto(
            JwtTokenDto jwtToken)
        {
            JwtToken = jwtToken;
        }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<UserDto, CommandErrorResponse>>
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IAesSecurity _aes;
        private readonly IAuthHelper _authHelper;

        public LoginCommandHandler(
            ILogger<LoginCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IAesSecurity aes,
            IAuthHelper authHelper,
            IUserRepository userRepository)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _aes = aes;
            _authHelper = authHelper;
            _userRepository = userRepository;
        }

        public async Task<Result<UserDto, CommandErrorResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.QueryOne(x => x.UserName == request.UserName);
                if (user == null)
                    return ResultYm.NotFound<UserDto>("User not found.");

                var encryptPassword = _aes.Encrypt(request.Password);
                if (!encryptPassword.Equals(user.Password))
                    return ResultYm.Error<UserDto>(CommandErrorResponse.BusinessError(BusinessError.FailToAuthenticate__IncorrectPassword.Error()));

                await _unitOfWork.Commit();

                var jwtToken = _authHelper.GenerateJwtToken(user);

                return ResultYm.Success(new UserDto(jwtToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unknown error has occured while trying to login.");
                return ResultYm.Error<UserDto>(ex);
            }
        }
    }
}
