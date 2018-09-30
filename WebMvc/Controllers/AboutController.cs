namespace WebMvc.Controllers
{
    using System.Web.Mvc;
    using WebMvc.Application.Context;
    using WebMvc.Application.Interfaces;
    using WebMvc.Services;

    public class AboutController : BaseController
    {
        public AboutController(LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, CacheService cacheService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {

        }

        // GET: About
        public ActionResult Index()
        {
            return View();
        }
    }
}