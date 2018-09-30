namespace WebMvc.Controllers
{
    using System;
    using System.Web.Mvc;
    using WebMvc.Application.Context;
    using WebMvc.Application.Interfaces;
    using WebMvc.Services;
    using WebMvc.ViewModels;

    public class EmployeesController : BaseController
    {
        public readonly EmployeesRoleService _employeesRoleService;
        public readonly EmployeesService _employeesService;

        // GET: Employees
        public EmployeesController(EmployeesService employeesService, EmployeesRoleService employeesRoleService,LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, CacheService cacheService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {
            _employeesService = employeesService;
            _employeesRoleService = employeesRoleService;
        }

        [ChildActionOnly]
        public ActionResult OnlineOrder()
        {
            var modelView = new EmployeesViewModel
            {
                employeesRoles = _employeesRoleService.GetAll(),
                employees = _employeesService.GetAll(),
            };

            return PartialView(modelView);
        }


    }
}