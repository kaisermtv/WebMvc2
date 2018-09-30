namespace WebMvc.Controllers
{
    using System;
    using System.Web.Mvc;
    using WebMvc.Application.Context;
    using WebMvc.Application.Entities;
    using WebMvc.Application.Interfaces;
    using WebMvc.Areas.Admin.ViewModels;
    using WebMvc.Services;
    using WebMvc.ViewModels;


    public class BookingController : BaseController
    {
        public readonly BookingSevice _bookingSevice;
        public readonly TypeRoomSevice _typeRoomSevice;


        public BookingController(TypeRoomSevice typeRoomSevice,BookingSevice bookingSevice,LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, CacheService cacheService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {
            _bookingSevice = bookingSevice;
            _typeRoomSevice = typeRoomSevice;
        }

        // GET: Booking
        public ActionResult Index()
        {
            BookingCreateViewModel viewModel = new BookingCreateViewModel();

            viewModel.CheckIn = DateTime.Now;

            viewModel.CheckOut = DateTime.Now.AddDays(1);

            viewModel.ListTypeRoom = _typeRoomSevice.GetBaseSelectListTypeRooms(_typeRoomSevice.GetList(true));

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(BookingCreateViewModel modelView)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    Booking bk = new Booking();

                    bk.NamePartner = modelView.NamePartner;
                    bk.Email = modelView.Email;
                    bk.CheckIn = modelView.CheckIn;
                    bk.CheckOut = modelView.CheckOut;
                    bk.Adukts = modelView.Adukts;
                    bk.Adolescent = modelView.Adolescent;
                    bk.Children = modelView.Children;
                    bk.Phone = modelView.Phone;
                    bk.TypeRoom_Id = modelView.TypeRoom_Id;

                    try
                    {
                        _bookingSevice.Add(bk);

                        unitOfWork.Commit();

                        return RedirectToAction("Ok");
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);

                        ViewBag.Message = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Booking.Error"),
                            MessageType = GenericMessages.warning
                        };
                    }

                }

            }

            modelView.ListTypeRoom = _typeRoomSevice.GetBaseSelectListTypeRooms(_typeRoomSevice.GetList(true));
            return View(modelView);
        }

        public ActionResult Ok()
        {
            return View();
        }
    }
}