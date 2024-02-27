using Api.Infrastructure;
using Api.Infrastructure.Authorization;
using Api.Infrastructure.Helpers;
using Api.Model;
using Api.Seedwork;
using Api.Seedwork.AesEncryption;
using CSharpFunctionalExtensions;
using Domain;
using Domain.AggregatesModel.UserAggregate;
using Domain.Model;
using FluentValidation;
using Api.Infrastructure.Seedwork;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Features.User
{
    public class UpdateUserCommand : IRequest<Result<UpdatedUserDto, CommandErrorResponse>>
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PreferredName { get; set; }
        public string CountryCode { get; set; }
        public string ContactNumber { get; set; }
        public string Introduction { get; set; }
        public List<UpdateAttachment> ProfilePictures { get; set; }

        public WorkPreference[] WorkPreferences { get; set; }
        public List<SkillSet> SkillSets { get; set; }
        public List<WorkExperience> WorkExperiences { get; set; }

        public DateTime ModifiedUTCDateTime { get; set; }
    }

    public class UpdatedUserDto
    {
        public string UserId { get; }

        public UpdatedUserDto(
            string userId)
        {
            UserId = userId;
        }
    }

    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(a => a.UserId).NotEmpty();
            RuleFor(a => a.ModifiedUTCDateTime).NotNull().NotEmpty();
        }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UpdatedUserDto, CommandErrorResponse>>
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IAesSecurity _aes;
        private readonly IAttachmentHelper _serviceHelper;
        private readonly DirectoryPathConfigurationOptions _directoryPathConfiguration;

        private readonly ICurrentUserAccessor _currentUser;

        public UpdateUserCommandHandler(
            ILogger<UpdateUserCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IAesSecurity aes,
            IAttachmentHelper serviceHelper,
            IOptions<DirectoryPathConfigurationOptions> directoryPathConfiguration,
            IUserRepository userRepository,
            ICurrentUserAccessor currentUser)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _aes = aes;
            _serviceHelper = serviceHelper;
            _directoryPathConfiguration = directoryPathConfiguration.Value;
            _userRepository = userRepository;
            _currentUser = currentUser;
        }

        public async Task<Result<UpdatedUserDto, CommandErrorResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetUser(userId: request.UserId);

                if (user == null)
                    return ResultYm.NotFound<UpdatedUserDto>("User not found.");

                if (!request.ModifiedUTCDateTime.Equals(user.ModifiedUTCDateTime))
                    return ResultYm.Error<UpdatedUserDto>(CommandErrorResponse.BusinessError(BusinessError.ConcurrencyUpdate.Error()));

                
                List<Attachment> profilePictures = new List<Attachment>();

                if (request.ProfilePictures != null && request.ProfilePictures.Count > 0)
                {
                    var valid = true;

                    string[] supportedFileTypes = { "image/jpg", "image/png", "image/jpeg" };

                    request.ProfilePictures.ForEach(x =>
                    {
                        if (valid && Array.IndexOf(supportedFileTypes, x.BlobType) == -1)
                        {
                            valid = false;
                        }
                    });

                    if (!valid)
                        return ResultYm.Error<UpdatedUserDto>(CommandErrorResponse.BusinessError(BusinessError.FailToUpdateUser__InvalidFileType.Error()));

                    request.ProfilePictures.ForEach(async delegate (UpdateAttachment attachment)
                    {
                        var profilePicture = user.ProfilePictures != null ? user.ProfilePictures.FirstOrDefault(p => p.AttachmentId.Equals(attachment.AttachmentId)) : null;

                        if (profilePicture == null)
                        {
                            var newAttachmentId = Guid.NewGuid().ToString("N");
                            var attachmentFileName = $"{newAttachmentId}-{attachment.AttachmentFileName}";

                            profilePictures.Add(
                                new Attachment(
                                    attachmentId: newAttachmentId,
                                    name: attachmentFileName,
                                    blobType: attachment.BlobType
                                )
                            );

                            await _serviceHelper.CreateAttachment(attachment.AttachmentBase64, attachmentFileName, _directoryPathConfiguration.ProfilePicture);
                        }
                        else
                            profilePictures.Add(profilePicture);

                    });
                }

                user.UpdateUserDetails(
                    new Domain.AggregatesModel.UserAggregate.User(
                        firstName: request.FirstName,
                        lastName: request.LastName,
                        preferredName: request.PreferredName,
                        countryCode: request.CountryCode,
                        contactNumber: request.ContactNumber,
                        introduction: request.Introduction,
                        profilePictures: profilePictures,
                        workPreferences: request.WorkPreferences,
                        skillSets: request.SkillSets,
                        workExperiences: request.WorkExperiences
                    )
                );

                await _userRepository.UpdateUser(
                    user: user,
                    currentUser: _currentUser.Tuple(),
                    userId: request.UserId);

                await _unitOfWork.Commit();

                return ResultYm.Success(new UpdatedUserDto(user.UserId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unknown error has occured while trying to update user details.");
                return ResultYm.Error<UpdatedUserDto>(ex);
            }
        }
    }
}
