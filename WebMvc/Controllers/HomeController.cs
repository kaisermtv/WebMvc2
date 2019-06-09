namespace WebMvc.Controllers
{
    using System;
    using System.Web.Mvc;
    using WebMvc.Application.Attribute;
    using WebMvc.Application.Context;
    using WebMvc.Application.Interfaces;
    using WebMvc.Services;
    using WebMvc.ViewModels;

    public class HomeController : BaseController
    {
        public HomeController(LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, CacheService cacheService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {

        }

        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public PartialViewResult Slider()
        {


            return PartialView();
        }
    }
}
