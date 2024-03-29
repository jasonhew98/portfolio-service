﻿using Api.Seedwork;
using Api.Seedwork.AesEncryption;
using CSharpFunctionalExtensions;
using Domain.AggregatesModel.UserAggregate;
using Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Features.User
{
    public class GetUserQuery : IRequest<Result<UserDetailDto, CommandErrorResponse>>
    {
        public string UserId { get; set; }

        public GetUserQuery(
            string userId)
        {
            UserId = userId;
        }
    }

    public class UserDetailDto
    {
        public string UserName { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string PreferredName { get; }
        public string CountryCode { get; }
        public string ContactNumber { get; }
        public string Introduction { get; }
        public List<Attachment> ProfilePictures { get; }
        public WorkPreference[] WorkPreferences { get; }
        public List<SkillSet> SkillSets { get; }
        public List<WorkExperience> WorkExperiences { get; }
        public DateTime? ModifiedUTCDateTime { get; }

        public UserDetailDto(
            string userName,
            string email,
            string firstName,
            string lastName,
            string preferredName,
            string countryCode,
            string contactNumber,
            string introduction,
            DateTime? modifiedUTCDateTime,
            WorkPreference[] workPreferences,
            List<SkillSet> skillSets = null,
            List<WorkExperience> workExperiences = null,
            List<Attachment> profilePictures = null)
        {
            UserName = userName;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            PreferredName = preferredName;
            CountryCode = countryCode;
            ContactNumber = contactNumber;
            Introduction = introduction;
            ProfilePictures = profilePictures;
            WorkPreferences = workPreferences;
            SkillSets = skillSets;
            WorkExperiences = workExperiences;
            ModifiedUTCDateTime = modifiedUTCDateTime;
        }
    }

    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<UserDetailDto, CommandErrorResponse>>
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;
        private readonly IAesSecurity _aes;

        public GetUserQueryHandler(
            ILogger<GetUserQueryHandler> logger,
            IAesSecurity aes,
            IUserRepository userRepository)
        {
            _logger = logger;
            _aes = aes;
            _userRepository = userRepository;
        }

        public async Task<Result<UserDetailDto, CommandErrorResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetUser(
                    userId: request.UserId);

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
