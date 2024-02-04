using Api.Infrastructure.Authorization;
using Api.Seedwork;
using Api.Seedwork.AesEncryption;
using CSharpFunctionalExtensions;
using Domain.AggregatesModel.UserAggregate;
using Domain.Model;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Features.User
{
    public class GetMyProfileQuery : IRequest<Result<UserDetailDto, CommandErrorResponse>>
    {
    }

    public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, Result<UserDetailDto, CommandErrorResponse>>
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;
        private readonly IAesSecurity _aes;
        private readonly ICurrentUserAccessor _currentUser;

        public GetMyProfileQueryHandler(
            ILogger<GetMyProfileQueryHandler> logger,
            IAesSecurity aes,
            IUserRepository userRepository,
            ICurrentUserAccessor currentUser)
        {
            _logger = logger;
            _aes = aes;
            _userRepository = userRepository;
            _currentUser = currentUser;
        }

        public async Task<Result<UserDetailDto, CommandErrorResponse>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUser.UserId;

                var user = await _userRepository.GetUser(
                    userId: userId);

                if (user == null)
                    return ResultYm.NotFound<UserDetailDto>("User not found.");

                var result = CreateFromDomain(user);

                return ResultYm.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "An error has occured while trying to get user detail.");
                return ResultYm.Error<UserDetailDto>(ex);
            }
        }

        public UserDetailDto CreateFromDomain(Domain.AggregatesModel.UserAggregate.User user)
        {
            return new UserDetailDto(
                userName: user.UserName,
                email: _aes.Decrypt(user.Email),
                firstName: user.FirstName,
                lastName: user.LastName,
                preferredName: user.PreferredName,
                countryCode: user.CountryCode,
                contactNumber: user.ContactNumber,
                introduction: user.Introduction,
                profilePictures: user.ProfilePictures,
                workPreferences: user.WorkPreferences,
                skillSets: user.SkillSets,
                workExperiences: user.WorkExperiences,
                modifiedUTCDateTime: user.ModifiedUTCDateTime);
        }
    }
}
