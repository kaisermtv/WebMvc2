using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc.Filters;
using WebMvc.Application.Entities;
using WebMvc.Application.Interfaces;
using WebMvc.Application.Lib;
using WebMvc.Services;

namespace WebMvc.Application
{
    public class Login
    {
        private const string CookiesKey = "LoginKey";
        private const int maxInvalidPasswordAttempts = 5;
        private readonly TimeSpan TimeOut = new TimeSpan(0, 10, 0);
        protected HttpContext Context => HttpContext.Current;
        protected HttpRequest Request => Context.Request;
        protected HttpResponse Response => Context.Response;
        protected HttpCookieCollection Cookies => Request.Cookies;
        protected MembershipLoginService LoginService => ServiceFactory.Get<MembershipLoginService>();
        protected MembershipService MembershipService => ServiceFactory.Get<MembershipService>();
        protected PermissionService PermissionService => ServiceFactory.Get<PermissionService>();
        protected MembershipLoginService MembershipLogin => ServiceFactory.Get<MembershipLoginService>();
        protected IUnitOfWorkManager UnitOfWorkManager => ServiceFactory.Get<IUnitOfWorkManager>();
        protected LoggingService LoggingService => ServiceFactory.Get<LoggingService>();

        //public Login(MembershipLoginService _LoginService, MembershipService _MembershipService, MembershipLoginService _MembershipLogin, IUnitOfWorkManager _UnitOfWorkManager, LoggingService _LoggingService)
        //{
        //    LoginService = _LoginService;
        //    MembershipService = _MembershipService;
        //    MembershipLogin = _MembershipLogin;
        //    UnitOfWorkManager = _UnitOfWorkManager;
        //    LoggingService = _LoggingService;
        //}


        private Guid LoginKey;
        private MembershipUser _User;
        public MembershipUser User
        {
            get
            {
                CheckLogin();
                return _User;
            }
            private set
            {
                _User = value;
            }
        }

        private bool isCheck = false;
        private void CheckLogin()
        {
            if (isCheck) return;
            isCheck = true;
            try
            {
                var ck = Cookies.Get(CookiesKey);
                if (ck == null) return;
                LoginKey = new Guid(ck.Value);

                var login = LoginService.Get(LoginKey);
                if (login == null) return;

                if(!login.Remember && ((DateTime.UtcNow - login.OnlineDate) > TimeOut))
                {
                    MembershipLogin.Del(LoginKey);
                    //Response.Cookies.Add(new HttpCookie(CookiesKey, ""));
                    return;
                }

                User = MembershipService.Get(login.UserId);
                if (User == null) return;

                if(User.Password != login.Password)
                {
                    User = null;
                    MembershipLogin.Del(LoginKey);
                    return;
                }

                MembershipLogin.UpdateOnline(LoginKey);

                Type = (TypeLogin)login.TypeLogin;
            }
            catch(Exception ex)
            {
                User = null;
                Type = TypeLogin.NotLogin;
                LoggingService.Error(ex);
            }

        }

        private TypeLogin _Type = TypeLogin.NotLogin;
        public TypeLogin Type
        {
            get
            {
                CheckLogin();
                return _Type;
            }
            private set
            {
                _Type = value;
            }
        }
        public enum TypeLogin {
            NotLogin,
            UserLogin,
            AdminLogin,
            SuperLogin
        }

        #region Roles
        private List<MembershipRole> _Roles;
        public List<MembershipRole> Roles
        {
            get
            {
                if (_Roles != null) return _Roles;
                if (User == null) return null;

                _Roles = MembershipService.GetRoles(User);

                return _Roles;
            }
        }

        public bool IsInRole(string role)
        {
            foreach (var it in Roles)
            {
                if (it.RoleName == role) return true;
            }

            return false;
        }
        #endregion

        #region Permisions
        private List<Permission> _Permisions;
        public List<Permission> Permisions
        {
            get
            {
                if (_Permisions != null) return _Permisions;
                if (Roles == null) return null;

                _Permisions = PermissionService.GetPermissions(Roles);

                return _Permisions;
            }
        }
        public bool IsInPermision(string permision)
        {
            if (IsInRole(AppConstants.AdminRoleName)) return true;
            
            foreach (var it in Permisions)
            {
                if (it.Name == permision) return true;
            }

            return false;
        }
        #endregion

        public bool IsSuperAccount()
        {
            if (User.UserName == AppConstants.AdminUsername) return true;
            return false;
        }

        public void LogOut()
        {
            CheckLogin();

            if(LoginKey != null)
            {
                MembershipLogin.Del(LoginKey);
            }

            //Response.Cookies.Add(new HttpCookie(CookiesKey, ""));
        }

        public LoginAttemptStatus ValidateUser(string UserName, string Password, bool Remember = false, TypeLogin type = TypeLogin.UserLogin)
        {
            LogOut();

            var LastLoginStatus = LoginAttemptStatus.LoginSuccessful;

            var user = MembershipService.GetUser(UserName);

            if (user == null) 
            {
                LastLoginStatus = LoginAttemptStatus.UserNotFound;
            }
            else if (user.IsBanned)
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
            
            if (LastLoginStatus == LoginAttemptStatus.LoginSuccessful)
            {
                var allowedPasswordAttempts = maxInvalidPasswordAttempts;
                if (user.FailedPasswordAttemptCount >= allowedPasswordAttempts)
                {
                    LastLoginStatus = LoginAttemptStatus.PasswordAttemptsExceeded;
                }

                var salt = user.PasswordSalt;
                var hash = StringUtils.GenerateSaltedHash(Password, salt);
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
                }
                else
                {
                    user.LastLoginDate = DateTime.UtcNow;
                }

                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        MembershipService.UpdateLogin(user);

                        var datenow = DateTime.UtcNow;
                        var datakey = new Entities.MembershipLogin
                        {
                            UserId = user.Id,
                            Password = user.Password,
                            LoginDate = datenow,
                            OnlineDate = datenow,
                            Remember = Remember,
                            TypeLogin = Convert.ToInt32(type)
                        };

                        MembershipLogin.Add(datakey);



                        Response.Cookies.Add(new HttpCookie(CookiesKey, datakey.Id.ToString()));

                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);

                        LastLoginStatus = LoginAttemptStatus.OutOfException;
                    }
                }
                

            }


            return LastLoginStatus;
        }
    }

    public class LoginPrincipal : IPrincipal
    {
        private Login LoginRequest => ServiceFactory.Get<Login>();
        public IIdentity Identity => new LoginIdentity();

        public bool IsInRole(string role)
        {
            return LoginRequest.IsInRole(role);
        }

        private class LoginIdentity : IIdentity
        {
            private Login LoginRequest => ServiceFactory.Get<Login>();
            private MembershipUser User => LoginRequest.User;

            public string Name => User == null ? null : User.UserName;

            public string AuthenticationType => LoginRequest.Type.ToString();

            public bool IsAuthenticated => User == null ? false : true;
        }
    }
}