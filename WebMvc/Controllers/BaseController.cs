﻿namespace WebMvc.Controllers
{
    using System;
    using System.Web.Mvc;
    using System.Web.Mvc.Filters;
    using WebMvc.Application.Context;
    using WebMvc.Services;
    using WebMvc.Application.Entities;
    using WebMvc.Application.Lib;
    using WebMvc.Application;
    using WebMvc.ViewModels;
    using WebMvc.Areas.Admin.ViewModels;
    using WebMvc.Application.Interfaces;
    using WebMvc.Application.Attribute;

    public class BaseController : Controller
    {
        protected readonly IUnitOfWorkManager UnitOfWorkManager;
        protected readonly MembershipService MembershipService;
        protected readonly LocalizationService LocalizationService;
        protected readonly SettingsService SettingsService;
        protected readonly LoggingService LoggingService;
        protected readonly CacheService CacheService;

        private Login _LoginRequest;
        protected Login LoginRequest {
            get
            {
                if (_LoginRequest == null) _LoginRequest = ServiceFactory.Get<Login>();
                return _LoginRequest;
            }
        }

        protected MembershipUser LoggedOnReadOnlyUser => LoginRequest.User;
        protected MembershipUser LoginUser => LoginRequest.User;
        protected Guid UsersRole;

        public BaseController(LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, CacheService cacheService,LocalizationService localizationService)
        {
            UnitOfWorkManager = unitOfWorkManager;
            MembershipService = membershipService;
            LocalizationService = localizationService;
            SettingsService = settingsService;
            CacheService = cacheService;
            LoggingService = loggingService;
            
                //UsersRole = LoggedOnReadOnlyUser.ro
                //UsersRole = LoggedOnReadOnlyUser == null ? RoleService.GetRole(AppConstants.GuestRoleName, true) : LoggedOnReadOnlyUser.Roles.FirstOrDefault();

        }



        protected bool UserIsAuthenticated => System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
        protected string Username => UserIsAuthenticated ? System.Web.HttpContext.Current.User.Identity.Name : null;


        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            base.OnAuthentication(filterContext);

            var lstAttribute = filterContext.ActionDescriptor.GetCustomAttributes(typeof(LoginAttribute), true);
            if(lstAttribute == null) lstAttribute = this.GetType().GetCustomAttributes(typeof(LoginAttribute), true);
            foreach (var obj in lstAttribute)
            {
                if (obj is LoginAttribute)
                {
                    var loginatrribute = obj as LoginAttribute;

                    loginatrribute.OnAuthorization(filterContext);
                }
            }
            
        }
        

        protected internal ActionResult ErrorToHomePage(string errorMessage)
        {
            // Use temp data as its a redirect
            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
            {
                Message = errorMessage,
                MessageType = GenericMessages.danger
            };
            // Not allowed in here so
            return RedirectToAction("Index", "Home");
        }

        protected internal PagingViewModel CalcPaging(int limit,int? page,int count)
        {
            var paging = new PagingViewModel
            {
                Count = count,
                Page = page ?? 1,
                MaxPage = (count / limit) + ((count % limit > 0) ? 1 : 0),
            };

            if (paging.Page > paging.MaxPage) paging.Page = paging.MaxPage;

            return paging;
        }

        #region User
        protected static bool IsRole(string role)
        {
            return System.Web.HttpContext.Current.User.IsInRole(role);
        }
        #endregion
    }
}