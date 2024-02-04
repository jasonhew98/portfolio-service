using Domain.Model;
using Domain.Seedwork;
using System;
using System.Collections.Generic;

namespace Domain.AggregatesModel.UserAggregate
{
    public class User : AuditableEntity, IAggregateRoot
    {
        public string UserId { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string PreferredName { get; private set; }
        public string CountryCode { get; private set; }
        public string ContactNumber { get; private set; }
        public string Introduction { get; private set; }
        public List<Attachment> ProfilePictures { get; private set; }

        public WorkPreference[] WorkPreferences { get; private set; }
        public List<SkillSet> SkillSets { get; private set; }
        public List<WorkExperience> WorkExperiences { get; private set; }

        public User(
            string firstName,
            string lastName,
            string preferredName = null,
            string countryCode = null,
            string contactNumber = null,
            string introduction = null,
            WorkPreference[] workPreferences = null,
            List<Attachment> profilePictures = null,
            List<SkillSet> skillSets = null,
            List<WorkExperience> workExperiences = null,

            string userId = null,
            string userName = null,
            string email = null,
            string password = null,
            string createdBy = null,
            string createdByName = null,
            DateTime? createdUTCDateTime = null,
            string modifiedBy = null,
            string modifiedByName = null,
            DateTime? modifiedUTCDateTime = null)
            : base(
                  createdBy: createdBy,
                  createdByName: createdByName,
                  createdUTCDateTime: createdUTCDateTime,
                  modifiedBy: modifiedBy,
                  modifiedByName: modifiedByName,
                  modifiedUTCDateTime: modifiedUTCDateTime)
        {
            UserId = userId;
            UserName = userName;
            Email = email;
            Password = password;

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
        }

        public void UpdateUserDetails(User user)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            PreferredName = user.PreferredName;
            CountryCode = user.CountryCode;
            ContactNumber = user.ContactNumber;
            Introduction = user.Introduction;
            ProfilePictures = user.ProfilePictures;

            WorkPreferences = user.WorkPreferences;
            SkillSets = user.SkillSets;
            WorkExperiences = user.WorkExperiences;
        }
    }
}
