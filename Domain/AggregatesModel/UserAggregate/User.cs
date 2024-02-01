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
        public string ContactNumber { get; private set; }
        public List<Attachment> ProfilePictures { get; private set; }

        public WorkPreference? WorkPreference { get; private set; }
        public List<SkillSet> SkillSets { get; private set; }
        public List<WorkExperience> WorkExperiences { get; private set; }

        public User(
            string firstName,
            string lastName,
            string contactNumber = null,
            WorkPreference? workPreference = null,
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
            ContactNumber = contactNumber;
            ProfilePictures = profilePictures;

            WorkPreference = workPreference;
            SkillSets = skillSets;
            WorkExperiences = workExperiences;
        }

        public void UpdateUserDetails(User user)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            ContactNumber = user.ContactNumber;
            ProfilePictures = user.ProfilePictures;

            WorkPreference = user.WorkPreference;
            SkillSets = user.SkillSets;
            WorkExperiences = user.WorkExperiences;
        }
    }
}
