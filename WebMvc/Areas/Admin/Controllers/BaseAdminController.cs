using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using WebMvc.Application;
using WebMvc.Application.Attribute;
using WebMvc.Application.Context;
using WebMvc.Application.Entities;
using WebMvc.Application.Interfaces;
using WebMvc.Application.Lib;
using WebMvc.Areas.Admin.ViewModels;
using WebMvc.Services;

namespace WebMvc.Areas.Admin.Controllers
{
    [AdminLogin]
    //[Authorize(Roles = AppConstants.AdminRoleName)]
    public class BaseAdminController : Controller
    {
        protected readonly IUnitOfWorkManager UnitOfWorkManager;
        protected readonly MembershipService MembershipService;
        protected readonly LocalizationService LocalizationService;
        protected readonly SettingsService SettingsService;
        protected readonly LoggingService LoggingService;


        //protected 

        protected Login LoginRequest => ServiceFactory.Get<Login>();

        protected MembershipUser LoggedOnReadOnlyUser => LoginRequest.User;
        protected MembershipUser LoginUser => LoginRequest.User;
        protected Guid UsersRole;

        public BaseAdminController(LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, LocalizationService localizationService)
        {
            UnitOfWorkManager = unitOfWorkManager;
            MembershipService = membershipService;
            LocalizationService = localizationService;
            SettingsService = settingsService;
            LoggingService = loggingService;
            
        }

       
        protected bool UserIsAuthenticated => System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
        protected string Username => UserIsAuthenticated ? System.Web.HttpContext.Current.User.Identity.Name : null;


        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            base.OnAuthentication(filterContext);

            var lstMAttribute = filterContext.ActionDescriptor.GetCustomAttributes(typeof(ModulAttribute), true);
            if (lstMAttribute.Count() == 0) lstMAttribute = this.GetType().GetCustomAttributes(typeof(ModulAttribute), true);
            foreach (var obj in lstMAttribute)
            {
                if (obj is ModulAttribute)
                {
                    var loginatrribute = obj as ModulAttribute;

                    loginatrribute.OnAuthorModul(filterContext);
                }
            }

            var lstAttribute = filterContext.ActionDescriptor.GetCustomAttributes(typeof(LoginAttribute), true);
            if (lstAttribute.Count() == 0) lstAttribute = this.GetType().GetCustomAttributes(typeof(LoginAttribute), true);
            foreach (var obj in lstAttribute)
            {
                if (obj is LoginAttribute)
                {
                    var loginatrribute = obj as LoginAttribute;

                    loginatrribute.OnAuthorization(filterContext);
                }
            }

        }

        internal ActionResult ErrorToHomePage(string errorMessage)
        {
            // Use temp data as its a redirect
            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
            {
                Message = errorMessage,
                MessageType = GenericMessages.danger
            };
            // Not allowed in here so
            return RedirectToAction("Index", "Home",new { area = "" });
        }

        protected internal AdminPagingViewModel CalcPaging(int limit, int? page, int count)
        {
            var paging = new AdminPagingViewModel
            {
                Count = count,
                Page = page ?? 1,
                MaxPage = (count / limit) + ((count % limit > 0) ? 1 : 0),
            };

            if (paging.Page > paging.MaxPage) paging.Page = paging.MaxPage;

            return paging;
        }
    }
}