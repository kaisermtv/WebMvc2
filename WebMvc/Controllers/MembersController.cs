namespace WebMvc.Controllers
{
    using System;
    using System.Web.Mvc;
    using WebMvc.Application.Attribute;
    using WebMvc.Application.Context;
    using WebMvc.Application.Entities;
    using WebMvc.Application.Interfaces;
    using WebMvc.Services;
    using WebMvc.ViewModels;

    [Login]
    public partial class MembersController : BaseController
    {
        public MembersController(LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, CacheService cacheService,LocalizationService localizationService)
            :base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {

        }

        // GET: Members
        public ActionResult Index()
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var model = MembershipService.GetUser(User.Identity.Name);



                return View(model);
            }
        }

        #region Register
        [NotLogin]
        public ActionResult Register()
        {
            if (SettingsService.GetSetting("SuspendRegistration") != "true")
            {
                // Populate empty viewmodel
                var viewModel = new MemberAddViewModel();

                // See if a return url is present or not and add it
                var returnUrl = Request["ReturnUrl"];
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    viewModel.ReturnUrl = returnUrl;
                }

                return View(viewModel);
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotLogin]
        public ActionResult Register(MemberAddViewModel userModel)
        {
            if (SettingsService.GetSetting("SuspendRegistration") != "true" && SettingsService.GetSetting("DisableStandardRegistration") != "true")
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    if (userModel.Password != userModel.ConfirmPassword)
                    {
                        ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Error.PasswordNotConfirm"));
                        return View(userModel);
                    }

                    // First see if there is a spam question and if so, the answer matches
                    if (!string.IsNullOrEmpty(SettingsService.GetSetting("SpamQuestion")))
                    {
                        // There is a spam question, if answer is wrong return with error
                        if (userModel.SpamAnswer == null || userModel.SpamAnswer.Trim() != SettingsService.GetSetting("SpamAnswer"))
                        {
                            // POTENTIAL SPAMMER!
                            ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Error.WrongAnswerRegistration"));
                            return View(userModel);
                        }
                    }

                    // Secondly see if the email is banned
                    //if (_bannedEmailService.EmailIsBanned(userModel.Email))
                    //{
                    //    ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Error.EmailIsBanned"));
                    //    return View();
                    //}

                    MembershipUser newuser = new MembershipUser();
                    newuser.UserName = userModel.UserName;
                    newuser.Password = userModel.Password;
                    newuser.Email = userModel.Email;


                    var createStatus = MembershipService.NewUser(newuser);
                    if (createStatus != MembershipCreateStatus.Success)
                    {
                        ModelState.AddModelError(string.Empty, MembershipService.ErrorCodeToString(createStatus));
                        return View(userModel);
                    }
                    else
                    {

                    }

                    try
                    {
                        unitOfWork.Commit();

                        if (Url.IsLocalUrl(userModel.ReturnUrl) && userModel.ReturnUrl.Length > 1 && userModel.ReturnUrl.StartsWith("/")
                            && !userModel.ReturnUrl.StartsWith("//") && !userModel.ReturnUrl.StartsWith("/\\"))
                        {
                            return Redirect(userModel.ReturnUrl);
                        }
                        return RedirectToAction("Index", "Home", new { area = string.Empty });
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        //System.Web.Security.FormsAuthentication.SignOut();
                        ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }


                }
            }
            return View(userModel);
        }
        #endregion


        #region Login
        [AllowAnonymous]
        [NotLogin]
        public ActionResult Login()
        {
            LogOnViewModel viewModel = new LogOnViewModel();

            var returnUrl = Request["ReturnUrl"];
            if (!string.IsNullOrEmpty(returnUrl))
            {
                viewModel.ReturnUrl = returnUrl;
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotLogin]
        public ActionResult Login(LogOnViewModel model)
        {
            if (ModelState.IsValid)
            {
                switch (LoginRequest.ValidateUser(model.UserName, model.Password, model.RememberMe, Application.Login.TypeLogin.UserLogin))
                {
                    case LoginAttemptStatus.LoginSuccessful:
                        if (Url.IsLocalUrl(model.ReturnUrl) && model.ReturnUrl.Length > 1 && model.ReturnUrl.StartsWith("/")
                                        && !model.ReturnUrl.StartsWith("//") && !model.ReturnUrl.StartsWith("/\\"))
                        {
                            return Redirect(model.ReturnUrl);
                        }

                        return RedirectToAction("Index", "Home");
                    case LoginAttemptStatus.UserNotFound:
                    case LoginAttemptStatus.PasswordIncorrect:
                        ModelState.AddModelError(string.Empty, "Tài khoản hoặc mật khẩu không chính xác!");
                        break;

                    case LoginAttemptStatus.PasswordAttemptsExceeded:
                        ModelState.AddModelError("Password", "Vượt quá số lần thử mật khẩu!");
                        break;

                    case LoginAttemptStatus.UserLockedOut:
                        ModelState.AddModelError("UserName", "Tài khoản này đã bị khóa!");
                        break;

                    case LoginAttemptStatus.Banned:
                        ModelState.AddModelError("UserName", "Tài khoản bị cấm!");
                        break;

                    case LoginAttemptStatus.UserNotApproved:
                        ModelState.AddModelError("UserName", "Tài khoản người dùng chưa được kích hoạt!");
                        //user = MembershipService.GetUser(username);
                        //SendEmailConfirmationEmail(user);
                        break;

                    default:
                        ModelState.AddModelError(string.Empty, "Lỗi đăng nhập không xác định!");
                        break;
                }
            }


            return View(model);
        }


        public ActionResult LogOut()
        {
            LoginRequest.LogOut();
            return RedirectToAction("Index", "Home", new { area = string.Empty });
        }

        #endregion



    }
}