using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using WebMvc.Application;
using WebMvc.Application.Context;
using WebMvc.Application.Entities;
using WebMvc.Application.General;
using WebMvc.Application.Interfaces;
using WebMvc.Application.Lib;
using WebMvc.Areas.Admin.ViewModels;
using WebMvc.Services;

namespace WebMvc.Areas.Admin.Controllers
{
    //[Authorize(Roles = AppConstants.AdminRoleName)]
    public class LogController : BaseAdminController
    {
        public LogController(LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {

        }

        public ActionResult Index()
        {
            IList<LogEntry> logs = new List<LogEntry>();

            try
            {
                logs = LoggingService.ListLogFile();
            }
            catch (Exception ex)
            {
                var err = $"Unable to access logs: {ex.Message}";
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = err,
                    MessageType = GenericMessages.danger
                };

                LoggingService.Error(err);
            }

            return View(new ListLogViewModel { LogFiles = logs });
        }

        public ActionResult ClearLog()
        {
            LoggingService.ClearLogFiles();

            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
            {
                Message = "Log File Cleared",
                MessageType = GenericMessages.success
            };
            return RedirectToAction("Index");
        }
    }
}