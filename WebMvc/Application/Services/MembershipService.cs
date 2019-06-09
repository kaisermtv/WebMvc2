using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using WebMvc.Application;
using WebMvc.Application.Context;
using WebMvc.Application.Entities;
using WebMvc.Application.Lib;

namespace WebMvc.Services
{
    public enum LoginAttemptStatus
    {
        LoginSuccessful,
        UserNotFound,
        PasswordIncorrect,
        PasswordAttemptsExceeded,
        UserLockedOut,
        UserNotApproved,
        Banned,
        OutOfException
    }

    public partial class MembershipService
    {
        private readonly WebMvcContext _context;
        private readonly CacheService _cacheService;
        private readonly LocalizationService _localizationService;
        private readonly LoggingService _loggingService;

        public MembershipService(LoggingService loggingService,WebMvcContext context, CacheService cacheService, LocalizationService localizationService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
            _localizationService = localizationService;
            _loggingService = loggingService;
        }

        #region Status Codes
        public string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return _localizationService.GetResourceString("Members.Errors.DuplicateUserName");

                case MembershipCreateStatus.DuplicateEmail:
                    return _localizationService.GetResourceString("Members.Errors.DuplicateEmail");

                case MembershipCreateStatus.InvalidPassword:
                    return _localizationService.GetResourceString("Members.Errors.InvalidPassword");

                case MembershipCreateStatus.InvalidEmail:
                    return _localizationService.GetResourceString("Members.Errors.InvalidEmail");

                case MembershipCreateStatus.InvalidAnswer:
                    return _localizationService.GetResourceString("Members.Errors.InvalidAnswer");

                case MembershipCreateStatus.InvalidQuestion:
                    return _localizationService.GetResourceString("Members.Errors.InvalidQuestion");

                case MembershipCreateStatus.InvalidUserName:
                    return _localizationService.GetResourceString("Members.Errors.InvalidUserName");

                case MembershipCreateStatus.ProviderError:
                    return _localizationService.GetResourceString("Members.Errors.ProviderError");

                case MembershipCreateStatus.UserRejected:
                    return _localizationService.GetResourceString("Members.Errors.UserRejected");

                default:
                    return _localizationService.GetResourceString("Members.Errors.Unknown");
            }
        }
        #endregion

        public MembershipUser SanitizeUser(MembershipUser membershipUser)
        {
            membershipUser.Avatar = StringUtils.SafePlainText(membershipUser.Avatar);
            membershipUser.Comment = StringUtils.SafePlainText(membershipUser.Comment);
            membershipUser.Email = StringUtils.SafePlainText(membershipUser.Email);
            membershipUser.Password = StringUtils.SafePlainText(membershipUser.Password);
            membershipUser.PasswordAnswer = StringUtils.SafePlainText(membershipUser.PasswordAnswer);
            membershipUser.PasswordQuestion = StringUtils.SafePlainText(membershipUser.PasswordQuestion);
            membershipUser.Signature = StringUtils.GetSafeHtml(membershipUser.Signature, true);
            membershipUser.Twitter = StringUtils.SafePlainText(membershipUser.Twitter);
            membershipUser.UserName = StringUtils.SafePlainText(membershipUser.UserName);
            membershipUser.Website = StringUtils.SafePlainText(membershipUser.Website);
            return membershipUser;
        }

        public void Add(MembershipUser User)
        {
            //string cachekey = string.Concat(CacheKeys.Member.StartsWith, "getSetting-", key);

            using (var Cmd = _context.CreateCommand())
            {
                bool ret = Cmd.Add<MembershipUser>(User) > 0;
                Cmd.cacheStartsWithToClear(CacheKeys.Member.StartsWith);

                if (!ret) throw new Exception("Add MembershipUser false");
            }

            //var Cmd = _context.CreateCommand();

            //Cmd.CommandText = "IF NOT EXISTS (SELECT * FROM MembershipUser WHERE Id = @Id OR UserName = @UserName OR Email = @Email)";
            //Cmd.CommandText += " BEGIN INSERT INTO MembershipUser(Id,UserName,Password,PasswordSalt,Email,CreateDate,LastLockoutDate,LastPasswordChangedDate,LastLoginDate,IsLockedOut,IsApproved,IsBanned,FailedPasswordAttemptCount,FailedPasswordAnswerAttempt,Slug)";
            //Cmd.CommandText += " VALUES(@Id,@UserName,@Password,@PasswordSalt,@Email,@CreateDate,@LastLockoutDate,@LastPasswordChangedDate,@LastLoginDate,@IsLockedOut,@IsApproved,@IsBanned,@FailedPasswordAttemptCount,@FailedPasswordAnswerAttempt,@Slug) END ";

            //Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = User.Id;
            //Cmd.Parameters.Add("UserName", SqlDbType.NVarChar).Value = User.UserName;
            //Cmd.Parameters.Add("Password", SqlDbType.NVarChar).Value = User.Password;
            //Cmd.Parameters.Add("PasswordSalt", SqlDbType.NVarChar).Value = User.PasswordSalt;
            //Cmd.AddParameters("Email", User.Email);
            //Cmd.AddParameters("CreateDate", User.CreateDate);
            //Cmd.AddParameters("LastLockoutDate", User.LastLockoutDate);
            //Cmd.AddParameters("LastPasswordChangedDate", User.LastPasswordChangedDate);
            //Cmd.AddParameters("LastLoginDate", User.LastLoginDate);
            //Cmd.AddParameters("IsLockedOut", User.IsLockedOut);
            //Cmd.AddParameters("IsApproved", User.IsApproved);
            //Cmd.AddParameters("IsBanned", User.IsBanned);
            //Cmd.AddParameters("FailedPasswordAttemptCount", User.FailedPasswordAttemptCount);
            //Cmd.AddParameters("FailedPasswordAnswerAttempt", User.FailedPasswordAnswerAttempt);
            //Cmd.AddParameters("Slug", User.Slug);
            

            //bool ret = Cmd.command.ExecuteNonQuery() > 0;

            //Cmd.cacheStartsWithToClear(CacheKeys.Member.StartsWith);
            //Cmd.Close();

            //if (!ret) throw new Exception("Add MembershipUser false");
        }

        public void ClearRolesByUser(MembershipUser user)
        {
            using (var Cmd = _context.CreateCommand())
            {
                Cmd.CommandText = "DELETE FROM [dbo].[MembershipUsersInRoles] WHERE UserIdentifier = @UserId";
                Cmd.AddParameters("UserId", user.Id);

                Cmd.command.ExecuteNonQuery();
            }
        }

        public void AddRoleByUser(Guid userId,Guid roleId)
        {
            using (var Cmd = _context.CreateCommand())
            {
                Cmd.CommandText = "INSERT INTO [dbo].[MembershipUsersInRoles](UserIdentifier,RoleIdentifier) VALUES (@UserId,@RoleId) ";
                Cmd.AddParameters("UserId", userId);
                Cmd.AddParameters("RoleId", roleId);

                Cmd.command.ExecuteNonQuery();
            }
        }

        public MembershipUser Get(Guid Id,bool nocache = false)
        {
            string cachekey = string.Concat(CacheKeys.Member.StartsWith, "Get-", Id);
            var data = _cacheService.Get<MembershipUser>(cachekey);
            if (data == null || nocache)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT * FROM [MembershipUser] WHERE Id = @Id";

                Cmd.AddParameters("Id", Id);

                data = Cmd.FindFirst<MembershipUser>();
                if (data == null) return null;

                if(!nocache) _cacheService.Set(cachekey, data, CacheTimes.OneDay);
            }
            return data;
        }

        public void Update(MembershipUser User)
        {
            var Cmd = _context.CreateCommand();

            //Cmd.CommandText = "IF NOT EXISTS (SELECT * FROM [Topic] WHERE [Id] = @Id)";
            Cmd.CommandText = "UPDATE [dbo].[MembershipUser] SET UserName = @UserName,Password = @Password,PasswordSalt = @PasswordSalt,Email = @Email,LastLockoutDate = @LastLockoutDate,FailedPasswordAnswerAttempt = @FailedPasswordAnswerAttempt,Slug = @Slug,"
                            + "LastPasswordChangedDate = @LastPasswordChangedDate,LastLoginDate = @LastLoginDate,IsLockedOut = @IsLockedOut,IsApproved = @IsApproved,IsBanned = @IsBanned,FailedPasswordAttemptCount = @FailedPasswordAttemptCount"
                            + " WHERE [Id] = @Id";

            Cmd.AddParameters("Id", User.Id);
            Cmd.AddParameters("UserName", User.UserName);
            Cmd.AddParameters("Password", User.Password);
            Cmd.AddParameters("PasswordSalt", User.PasswordSalt);
            Cmd.AddParameters("Email", User.Email);
            Cmd.AddParameters("CreateDate", User.CreateDate);
            Cmd.AddParameters("LastLockoutDate", User.LastLockoutDate);
            Cmd.AddParameters("LastPasswordChangedDate", User.LastPasswordChangedDate);
            Cmd.AddParameters("LastLoginDate", User.LastLoginDate);
            Cmd.AddParameters("IsLockedOut", User.IsLockedOut);
            Cmd.AddParameters("IsApproved", User.IsApproved);
            Cmd.AddParameters("IsBanned", User.IsBanned);
            Cmd.AddParameters("FailedPasswordAttemptCount", User.FailedPasswordAttemptCount);
            Cmd.AddParameters("FailedPasswordAnswerAttempt", User.FailedPasswordAnswerAttempt);
            Cmd.AddParameters("Slug", User.Slug);


            bool ret = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Member.StartsWith);

            Cmd.Close();

            if (!ret) throw new Exception("Update MembershipUser false");
        }

        public MembershipCreateStatus NewUser (MembershipUser newUser)
        {
            //string cachekey = string.Concat(CacheKeys.Member.StartsWith, "getSetting-", key);

            newUser = SanitizeUser(newUser);

            var status = MembershipCreateStatus.Success;

            if (string.IsNullOrEmpty(newUser.UserName))
            {
                status = MembershipCreateStatus.InvalidUserName;
            }

            // get by username
            if (GetUser(newUser.UserName) != null)
            {
                status = MembershipCreateStatus.DuplicateUserName;
            }

            // Add get by email address
            if (GetUserByEmail(newUser.Email) != null)
            {
                status = MembershipCreateStatus.DuplicateEmail;
            }

            if (string.IsNullOrEmpty(newUser.Password))
            {
                status = MembershipCreateStatus.InvalidPassword;
            }

            if (status == MembershipCreateStatus.Success)
            {
                var salt = StringUtils.CreateSalt(AppConstants.SaltSize);
                var hash = StringUtils.GenerateSaltedHash(newUser.Password, salt);
                newUser.Password = hash;
                newUser.PasswordSalt = salt;

                //newUser.Roles = new List<MembershipRole> { settings.NewMemberStartingRole };


                newUser.CreateDate = newUser.LastPasswordChangedDate = DateTime.UtcNow;
                newUser.LastLockoutDate = (DateTime)SqlDateTime.MinValue;
                newUser.LastLoginDate = DateTime.UtcNow;
                newUser.IsLockedOut = false;
                newUser.Slug = newUser.UserName;

                try
                {
                    Add(newUser);

                }
                catch(Exception ex)
                {
                    _loggingService.Error(ex);
                    status = MembershipCreateStatus.UserRejected;
                }
                
            }


            return status;
        }

        public MembershipUser GetUserByEmail(string email)
        {
            email = StringUtils.SafePlainText(email);
            var cacheKey = string.Concat(CacheKeys.Member.StartsWith, "GetUserByEmail-", email);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                using (var Cmd = _context.CreateCommand())
                {
                    Cmd.CommandText = "SELECT * FROM [MembershipUser] WHERE Email = @Email";

                    Cmd.AddParameters("Email", email);

                    return Cmd.FindFirst<MembershipUser>();
                }
            });

        }

        public MembershipUser GetUser(string username)
        {
            var cacheKey = string.Concat(CacheKeys.Member.StartsWith, "GetUser-", username);
            
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                using (var Cmd = _context.CreateCommand())
                {
                    Cmd.CommandText = "SELECT * FROM [MembershipUser] WHERE UserName = @UserName";

                    Cmd.AddParameters("UserName", username);

                    return Cmd.FindFirst<MembershipUser>();
                }
            });
        }

        #region Roles
        public MembershipRole GetRole(Guid Id,bool iscache = true)
        {
            if (iscache)
            {
                string cachekey = string.Concat(CacheKeys.Role.StartsWith, "GetRole-",Id);
                var role = _cacheService.Get<MembershipRole>(cachekey);
                if(role == null)
                {
                    using (var Cmd = _context.CreateCommand())
                    {
                        Cmd.CommandText = " SELECT * FROM [MembershipRole] WHERE [Id] = @Id";
                        Cmd.AddParameters("Id", Id);

                        role = Cmd.FindFirst<MembershipRole>();

                        _cacheService.Set(cachekey, role, CacheTimes.OneDay);
                    }
                }

                return role;
            } else
            {
                using (var Cmd = _context.CreateCommand())
                {
                    Cmd.CommandText = " SELECT * FROM [MembershipRole] WHERE [Id] = @Id";
                    Cmd.AddParameters("Id", Id);

                    return Cmd.FindFirst<MembershipRole>();
                }
            }
        }

        public List<MembershipRole> GetRoles(MembershipUser user)
        {
            using (var Cmd = _context.CreateCommand())
            {
                Cmd.CommandText = " SELECT RL.* FROM [MembershipRole] AS RL INNER JOIN [MembershipUsersInRoles] AS UR ON RL.Id = UR.RoleIdentifier WHERE UR.UserIdentifier = @UserId";

                Cmd.AddParameters("UserId", user.Id);

                return Cmd.FindAll<MembershipRole>();
            }
        }

        public bool UserInRole(MembershipUser user,string roleName)
        {
            using (var Cmd = _context.CreateCommand())
            {
                Cmd.CommandText = "IF EXITS( SELECT RL.* FROM [MembershipRole] AS RL INNER JOIN [MembershipUsersInRoles] AS UR ON RL.Id = UR.RoleIdentifier WHERE UR.UserIdentifier = @UserId && RL.[RoleName] = @RoleName)" +
                    " BEGIN SELECT 1 END" +
                    " ELSE BEGIN SELECT 0 END";

                Cmd.AddParameters("UserId", user.Id);
                Cmd.AddParameters("RoleName", roleName);

                return (bool)Cmd.command.ExecuteScalar();
            }
        }

        public List<MembershipRole> GetAllRoles()
        {
            string cachekey = string.Concat(CacheKeys.Role.StartsWith, "GetAllRoles");

            var roles = _cacheService.Get<List<MembershipRole>>(cachekey);
            if (roles == null)
            {
                using (var Cmd = _context.CreateCommand())
                {
                    Cmd.CommandText = " SELECT * FROM [MembershipRole] ";

                    roles = Cmd.FindAll<MembershipRole>();
                }
                
                _cacheService.Set(cachekey, roles, CacheTimes.OneDay);
            }
            return roles;
            
        }

        public List<MembershipRole> GetAllRoles(int limit = 10, int page = 1)
        {
            string cachekey = string.Concat(CacheKeys.Role.StartsWith, "GetAllRoles-",limit,"-",page);

            var roles = _cacheService.Get<List<MembershipRole>>(cachekey);
            if (roles == null)
            {
                using (var Cmd = _context.CreateCommand())
                {
                    Cmd.CommandText = "SELECT TOP " + limit + " * FROM (SELECT *,(ROW_NUMBER() OVER(ORDER BY [RoleName] ASC)) AS RowNum FROM [MembershipRole]) AS MyDerivedTable WHERE RowNum > @Offset";

                    Cmd.AddParameters("Offset", (page - 1) * limit);

                    roles = Cmd.FindAll<MembershipRole>();
                }

                _cacheService.Set(cachekey, roles, CacheTimes.OneDay);
            }
            return roles;
        }

        public void Add(MembershipRole Role)
        {
            //string cachekey = string.Concat(CacheKeys.Member.StartsWith, "getSetting-", key);

            using (var Cmd = _context.CreateCommand())
            {
                Cmd.CommandText = "IF NOT EXISTS (SELECT * FROM [dbo].[MembershipRole] WHERE Id = @Id OR RoleName = @RoleName)";
                Cmd.CommandText += " BEGIN INSERT INTO [dbo].[MembershipRole](Id,RoleName) VALUES(@Id,@RoleName) END ";

                Cmd.AddParameters("Id", Role.Id);
                Cmd.AddParameters("RoleName", Role.RoleName);

                bool ret = Cmd.command.ExecuteNonQuery() > 0;

                Cmd.cacheStartsWithToClear(CacheKeys.Role.StartsWith);

                if (!ret) throw new Exception("Add MembershipRole false");
            }   
        }

        public void Update(MembershipRole Role)
        {
            using (var Cmd = _context.CreateCommand())
            {
                Cmd.CommandText = "UPDATE [dbo].[MembershipRole] SET RoleName = @RoleName WHERE [Id] = @Id";

                Cmd.AddParameters("Id", Role.Id);
                Cmd.AddParameters("RoleName", Role.RoleName);

                bool ret = Cmd.command.ExecuteNonQuery() > 0;
                Cmd.cacheStartsWithToClear(CacheKeys.Member.StartsWith);

                if (!ret) throw new Exception("Update MembershipRole false");
            }

        }
        #endregion

        /// <summary>
        /// Return last login status
        /// </summary>
        public LoginAttemptStatus LastLoginStatus { get; private set; } = LoginAttemptStatus.LoginSuccessful;

        /// <summary>
        /// Validate a user by password
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="maxInvalidPasswordAttempts"> </param>
        /// <returns></returns>
        public MembershipUser ValidateUser(string userName, string password, int maxInvalidPasswordAttempts)
        {
            userName = StringUtils.SafePlainText(userName);
            password = StringUtils.SafePlainText(password);

            LastLoginStatus = LoginAttemptStatus.LoginSuccessful;

            var user = GetUser(userName);

            if (user == null)
            {
                LastLoginStatus = LoginAttemptStatus.UserNotFound;
            } else if (user.IsBanned)
            {
                LastLoginStatus = LoginAttemptStatus.Banned;
            }
            else if (user.IsLockedOut)
            {
                LastLoginStatus = LoginAttemptStatus.UserLockedOut;
            }
            else if (!user.IsApproved)
            {
                LastLoginStatus = LoginAttemptStatus.UserNotApproved;
            }

            var allowedPasswordAttempts = maxInvalidPasswordAttempts;
            if (user.FailedPasswordAttemptCount >= allowedPasswordAttempts)
            {
                LastLoginStatus = LoginAttemptStatus.PasswordAttemptsExceeded;
            }

            if (LastLoginStatus == LoginAttemptStatus.LoginSuccessful )
            {
                var salt = user.PasswordSalt;
                var hash = StringUtils.GenerateSaltedHash(password, salt);
                var passwordMatches = hash == user.Password;

                user.FailedPasswordAttemptCount = passwordMatches ? 0 : user.FailedPasswordAttemptCount + 1;

                if (user.FailedPasswordAttemptCount >= allowedPasswordAttempts)
                {
                    user.IsLockedOut = true;
                    user.LastLockoutDate = DateTime.UtcNow;
                }

                if (!passwordMatches)
                {
                    LastLoginStatus = LoginAttemptStatus.PasswordIncorrect;
                } else
                {
                    user.LastLoginDate = DateTime.UtcNow;
                }


                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "UPDATE [MembershipUser] SET FailedPasswordAttemptCount = @FailedPasswordAttemptCount,IsLockedOut = @IsLockedOut,LastLockoutDate = @LastLockoutDate,LastLoginDate = @LastLoginDate  WHERE [Id] = @Id";

                Cmd.AddParameters("Id", user.Id);
                Cmd.AddParameters("FailedPasswordAttemptCount", user.FailedPasswordAttemptCount);
                Cmd.AddParameters("IsLockedOut", user.IsLockedOut);
                Cmd.AddParameters("LastLockoutDate", user.LastLockoutDate);
                Cmd.AddParameters("LastLoginDate", user.LastLoginDate);

                Cmd.command.ExecuteNonQuery();

                Cmd.cacheStartsWithToClear(CacheKeys.Member.StartsWith);
                Cmd.Close();
            }

            if (LastLoginStatus != LoginAttemptStatus.LoginSuccessful) return null;
            return user;
        }

        public void UpdateLogin(MembershipUser user)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "UPDATE [MembershipUser] SET FailedPasswordAttemptCount = @FailedPasswordAttemptCount,IsLockedOut = @IsLockedOut,LastLockoutDate = @LastLockoutDate,LastLoginDate = @LastLoginDate  WHERE [Id] = @Id";


            Cmd.AddParameters("Id", user.Id);
            Cmd.AddParameters("FailedPasswordAttemptCount", user.FailedPasswordAttemptCount);
            Cmd.AddParameters("IsLockedOut", user.IsLockedOut);
            Cmd.AddParameters("LastLockoutDate", user.LastLockoutDate);
            Cmd.AddParameters("LastLoginDate", user.LastLoginDate);

            Cmd.command.ExecuteNonQuery();

            Cmd.cacheStartsWithToClear(CacheKeys.Member.StartsWith);
            Cmd.Close();
        }


        public bool ChangePassword(string userName, string password, string newpassword)
        {
            userName = StringUtils.SafePlainText(userName);
            password = StringUtils.SafePlainText(password);
            newpassword = StringUtils.SafePlainText(newpassword);

            LastLoginStatus = LoginAttemptStatus.LoginSuccessful;

            var user = GetUser(userName);

            if (user == null)
            {
                LastLoginStatus = LoginAttemptStatus.UserNotFound;
                return false;
            }

            var salt = user.PasswordSalt;
            var hash = StringUtils.GenerateSaltedHash(password, salt);
            var passwordMatches = hash == user.Password;

            if (!passwordMatches)
            {
                LastLoginStatus = LoginAttemptStatus.PasswordIncorrect;
                return false;
            }

            var newhash = StringUtils.GenerateSaltedHash(newpassword, salt);

            user.Password = newhash;
            user.LastPasswordChangedDate = DateTime.UtcNow;
            Update(user);


            return LastLoginStatus == LoginAttemptStatus.LoginSuccessful;
        }
        
        public int GetCount()
        {
            string cachekey = string.Concat(CacheKeys.Member.StartsWith, "GetCount");
            var count = _cacheService.Get<int?>(cachekey);
            if (count == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT COUNT(*) FROM  [MembershipUser]";

                count = (int)Cmd.command.ExecuteScalar();
                Cmd.Close();

                _cacheService.Set(cachekey, count, CacheTimes.OneDay);
            }
            return (int)count;
        }

        public List<MembershipUser> GetList(int limit = 10, int page = 1)
        {
            string cachekey = string.Concat(CacheKeys.Member.StartsWith, "GetList-", limit, "-", page);
            var list = _cacheService.Get<List<MembershipUser>>(cachekey);
            if (list == null)
            {
                using (var Cmd = _context.CreateCommand())
                {
                    if (page == 0) page = 1;

                    Cmd.CommandText = "SELECT TOP " + limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY UserName ASC)) AS RowNum FROM  [MembershipUser]) AS MyDerivedTable WHERE RowNum > @Offset";

                    //Cmd.Parameters.Add("limit", SqlDbType.Int).Value = limit;
                    Cmd.AddParameters("Offset", (page - 1) * limit);

                    list = Cmd.FindAll<MembershipUser>();
                    if (list == null) return null;

                    _cacheService.Set(cachekey, list, CacheTimes.OneDay);
                }
            }
            return list;
        }
    }
}
