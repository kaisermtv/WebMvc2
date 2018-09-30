namespace WebMvc.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using WebMvc.Application;
    using WebMvc.Application.Context;
    using WebMvc.Application.Entities;
    using WebMvc.Application.Interfaces;
    using WebMvc.Services;
    using WebMvc.ViewModels;

    public class PluginController : BaseController
    {
        private readonly ProductSevice _productSevice;

        public PluginController(ProductSevice productSevice, LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, CacheService cacheService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {
            _productSevice = productSevice;
        }

        // GET: Plugin
        public ActionResult ComputerBuilding()
        {
            var viewModel = new PluginComputerBuildingViewModel();
            viewModel.ProductClass = new List<PluginProductClassViewModel>();

            dynamic value = ThemesSetting.getValue("ComputerBuilding");

            foreach(var it in value)
            {
                var id = new Guid((string)it.Value);

                var cat = _productSevice.GetProductClass(id);
                if (cat == null) continue;

                var a = new PluginProductClassViewModel
                {
                    Id = id,
                    Name = cat.Name.ToUpper()
                };

                viewModel.ProductClass.Add(a);
            }


            return View(viewModel);
        }
    }
}