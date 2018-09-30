﻿using System;
using WebMvc.Application.Lib;

namespace WebMvc.Application.Entities
{
    /// <summary>
    /// Status values returned when creating a user
    /// </summary>
    public enum MembershipCreateStatus
    {
        Success,
        DuplicateUserName,
        DuplicateEmail,
        InvalidPassword,
        InvalidEmail,
        InvalidAnswer,
        InvalidQuestion,
        InvalidUserName,
        ProviderError,
        UserRejected
    }

    public partial class MembershipUser : Entity
    {
        public MembershipUser()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string Email { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsBanned { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime LastPasswordChangedDate { get; set; }
        public DateTime LastLockoutDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public int FailedPasswordAttemptCount { get; set; }
        public int FailedPasswordAnswerAttempt { get; set; }
        public string PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenCreatedAt { get; set; }
        public string Comment { get; set; }
        public string Slug { get; set; }
        public string Signature { get; set; }
        public int? Age { get; set; }
        public string Location { get; set; }
        public string Website { get; set; }
        public string Twitter { get; set; }
        public string Facebook { get; set; }

        public string Avatar { get; set; }
        public string FacebookAccessToken { get; set; }
        public long? FacebookId { get; set; }
        public string TwitterAccessToken { get; set; }
        public string TwitterId { get; set; }
        public string GoogleAccessToken { get; set; }
        public string GoogleId { get; set; }
        public string MicrosoftAccessToken { get; set; }
        public string MicrosoftId { get; set; }
        public bool? IsExternalAccount { get; set; }
        public bool? TwitterShowFeed { get; set; }
        public DateTime? LoginIdExpires { get; set; }
        public string MiscAccessToken { get; set; }

        public bool? DisableEmailNotifications { get; set; }
        public bool? DisablePosting { get; set; }
        public bool? DisablePrivateMessages { get; set; }
        public bool? DisableFileUploads { get; set; }

        public bool? HasAgreedToTermsAndConditions { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
