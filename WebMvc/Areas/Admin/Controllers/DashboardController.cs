using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using WebMvc.Application;
using WebMvc.Application.Context;
using WebMvc.Application.Entities;
using WebMvc.Application.Interfaces;
using WebMvc.Application.Lib;
using WebMvc.Areas.Admin.ViewModels;
using WebMvc.Services;

namespace WebMvc.Areas.Admin.Controllers
{
    //[Authorize(Roles = AppConstants.AdminRoleName)]
    public class DashboardController : BaseAdminController
    {
        public DashboardController(LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {

        }

        [ChildActionOnly]
        public PartialViewResult MainAdminNav()
        {
            var viewModel = new MainDashboardNavViewModel
            {
                PrivateMessageCount = 0,
                ModerateCount = 0
            };
            return PartialView(viewModel);
        }

    }
}