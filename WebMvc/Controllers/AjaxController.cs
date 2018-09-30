namespace WebMvc.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using WebMvc.Application.Context;
    using WebMvc.Application.Interfaces;
    using WebMvc.Services;
    using WebMvc.ViewModels;

    public class AjaxController : BaseController
    {
        public AjaxController(LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, CacheService cacheService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {

        }

        public ActionResult TopShowroom()
        {
            var viewModel = new AjaxShowroomViewModel
            {
                Showrooms = new List<AjaxShowroomItemViewModel>(),
            };

            var ShowroomCount = SettingsService.GetSetting("ShowroomCount");
            int count = 0;
            try
            {
                count = int.Parse(ShowroomCount);
            }
            catch { }

            for (int i = 0; i < count; i++)
            {
                viewModel.Showrooms.Add(new AjaxShowroomItemViewModel
                {
                    Addren = SettingsService.GetSetting("Showroom[" + i + "].Address"),
                    iFrameMap = SettingsService.GetSetting("Showroom[" + i + "].iFrameMap"),
                });
            }

            return PartialView(viewModel);
        }


    }
}