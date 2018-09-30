using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMvc.Application;
using WebMvc.Application.Entities;
using WebMvc.Application.Interfaces;
using WebMvc.Areas.Admin.ViewModels;
using WebMvc.Services;

namespace WebMvc.Areas.Admin.Controllers
{
    public class AdminCarouselController : BaseAdminController
    {
        public readonly CarouselService _carouselService;

        public AdminCarouselController(CarouselService carouselService, LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {
            _carouselService = carouselService;
        }

        // GET: Admin/AdminMenu
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            var viewModel = new AdminCarouselsViewModel
            {
                Carousels = _carouselService.GetAll()
            };
            return View(viewModel);
        }


        [ChildActionOnly]
        public PartialViewResult GetMainCarousels()
        {
            var viewModel = new AdminCarouselsViewModel
            {
                Carousels = _carouselService.GetAll()
            };
            return PartialView(viewModel);
        }


        #region Create
        public ActionResult Create()
        {
            var ViewModel = new AdminCarouselEditViewModel
            {
                AllCarousels = _carouselService.GetBaseSelectListCarousel(_carouselService.GetAll()),
            };
            return View(ViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdminCarouselEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var menu = new Carousel
                        {
                            Carousel_Id = viewModel.ParentCarousel,
                            Name = viewModel.Name,
                            Description = viewModel.Description,
                            SortOrder = viewModel.SortOrder,
                            Image = viewModel.Image,
                            Link = viewModel.Link,
                        };

                        //if (categoryViewModel.ParentCategory != null)
                        //{
                        //    var parentCategory = _categoryService.Get(categoryViewModel.ParentCategory.Value);
                        //    category.ParentCategory = parentCategory;
                        //    SortPath(category, parentCategory);
                        //}

                        _carouselService.Add(menu);

                        unitOfWork.Commit();
                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Thêm Carousel thành công",
                            MessageType = GenericMessages.success
                        };

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError("", "Có lỗi xảy ra khi thêm Carousel");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "There was an error creating the category");
            }

            viewModel.AllCarousels = _carouselService.GetBaseSelectListCarousel(_carouselService.GetAll());
            return View(viewModel);
        }

        #endregion


        #region Edit Menu
        private AdminCarouselEditViewModel CreateEditCarouselViewModel(Carousel menu)
        {
            var viewModel = new AdminCarouselEditViewModel
            {
                Id = menu.Id,
                ParentCarousel = menu.Carousel_Id,
                Name = menu.Name,
                Description = menu.Description,
                Image = menu.Image,
                SortOrder = menu.SortOrder,
                Link = menu.Link,
                AllCarousels = _carouselService.GetBaseSelectListCarousel(_carouselService.GetCarouselsParenCarousel(menu)),
            };
			
			return viewModel;
        }

        public ActionResult Edit(Guid id)
        {
            var menu = _carouselService.Get(id);
            var ViewModel = CreateEditCarouselViewModel(menu);

            return View(ViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminCarouselEditViewModel viewModel)
        {
            var menu = _carouselService.Get(viewModel.Id);
            if (menu == null) RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        // Check they are not trying to add a subcategory of this category as the parent or it will break
                        var cats = _carouselService.GetCarouselsParenCarousel(menu);
                        var lst = cats.Where(x => x.Id == viewModel.ParentCarousel).ToList();
                        if (lst.Count == 0) viewModel.ParentCarousel = null;
                        //categoryViewModel.AllCategories = _categoryService.GetBaseSelectListCategories(cats);

                        menu.Image = viewModel.Image;
                        menu.Carousel_Id = viewModel.ParentCarousel;
                        menu.Name = viewModel.Name;
                        menu.Description = viewModel.Description;
                        menu.Link = viewModel.Link;
                        menu.SortOrder = viewModel.SortOrder;

                        _carouselService.Update(menu);

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Cập nhật thành công",
                            MessageType = GenericMessages.success
                        };

                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error(ex);
                        unitOfWork.Rollback();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Có lỗi xảy ra khi cập nhật Carousel",
                            MessageType = GenericMessages.danger
                        };
                    }
                }
            }

            viewModel.AllCarousels = _carouselService.GetBaseSelectListCarousel(_carouselService.GetCarouselsParenCarousel(menu));
            return View(viewModel);
        }
        #endregion


        #region delete
        public ActionResult Del(Guid id)
        {
            var model = _carouselService.Get(id);
            if (model == null)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Carousel không tồn tại",
                    MessageType = GenericMessages.warning
                };

                return RedirectToAction("index");
            }

            var submenu = _carouselService.GetSubCarousel(model);
            if (submenu.Count > 0)
            {

                return View("NotDel", model);
            }

            return View(model);
        }

        [HttpPost]
        [ActionName("Del")]
        public ActionResult Del1(Guid id)
        {
            var model = _carouselService.Get(id);
            if (model == null)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Carousel không tồn tại",
                    MessageType = GenericMessages.warning
                };

                return RedirectToAction("index");
            }

            var submenu = _carouselService.GetSubCarousel(model);
            if (submenu.Count > 0)
            {

                return View("NotDel", model);
            }


            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    _carouselService.Del(model);

                    unitOfWork.Commit();

                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "Xóa Carousel thành công",
                        MessageType = GenericMessages.success
                    };
                    return RedirectToAction("index");
                }
                catch (Exception ex)
                {
                    LoggingService.Error(ex.Message);
                    unitOfWork.Rollback();

                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "Có lỗi xảy ra khi xóa Carousel",
                        MessageType = GenericMessages.warning
                    };
                }
            }


            return View(model);
        }
        #endregion
        
    }
}