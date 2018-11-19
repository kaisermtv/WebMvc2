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
    public class ShoppingCartController : BaseAdminController
    {
        public readonly ShoppingCartProductService _shoppingCartProductService;
        public readonly ShoppingCartService _shoppingCartService;

        public ShoppingCartController(ShoppingCartService shoppingCartService,ShoppingCartProductService shoppingCartProductService,LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {
            _shoppingCartService = shoppingCartService;
            _shoppingCartProductService = shoppingCartProductService;
        }

        // GET: Admin/ShoppingCart
        public ActionResult Index(int? p)
        {
            int limit = 20;
            var count = _shoppingCartService.GetCount();

            var Paging = CalcPaging(limit, p, count);

            var viewModel = new ShoppingCartViewModel
            {
                Paging = Paging,
                shoppingCarts = _shoppingCartService.GetList(limit, Paging.Page),
            };

            return View(viewModel);
        }

        public ActionResult Edit(Guid id)
        {
            var cart = _shoppingCartService.Get(id);
            if (cart == null) return RedirectToAction("Index", "ShoppingCart");

            var viewModel = new ShoppingCartEditViewModel
            {
                Id = cart.Id,
                Name = cart.Name,
                Email = cart.Email,
                Phone = cart.Phone,
                Addren = cart.Addren,
                ShipName = cart.ShipName,
                ShipPhone = cart.ShipPhone,
                ShipAddren = cart.ShipAddren,
                ShipNote = cart.ShipNote,
                TotalMoney = cart.TotalMoney,
                Note = cart.Note,
                Status = cart.Status,
                CreateDate = cart.CreateDate,

                products = _shoppingCartProductService.GetList(cart),
            };

            
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ShoppingCartEditViewModel viewModel)
        {
            var cart = _shoppingCartService.Get(viewModel.Id);
            if (cart == null) return RedirectToAction("Index", "ShoppingCart");

            viewModel.Id = cart.Id;
            viewModel.Name = cart.Name;
            viewModel.Email = cart.Email;
            viewModel.Phone = cart.Phone;
            viewModel.Addren = cart.Addren;
            viewModel.ShipName = cart.ShipName;
            viewModel.ShipPhone = cart.ShipPhone;
            viewModel.ShipAddren = cart.ShipAddren;
            viewModel.ShipNote = cart.ShipNote;
            viewModel.TotalMoney = cart.TotalMoney;
            viewModel.CreateDate = cart.CreateDate;

            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var t = Request.Form;
                        cart.Note = viewModel.Note;
                        cart.Status = viewModel.Status;

                        _shoppingCartService.Update(cart);

                        unitOfWork.Commit();
                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Cập nhật thành công",
                            MessageType = GenericMessages.success
                        };
                    }
                    catch(Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Lỗi khi cập nhật!"));
                    }
                }
            }

            viewModel.products = _shoppingCartProductService.GetList(cart);
            return View(viewModel);
        }


        #region delete
        public ActionResult Delete(Guid id)
        {
            var model = _shoppingCartService.Get(id);
            if (model == null)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Đơn đặt hàng không tồn tại",
                    MessageType = GenericMessages.warning
                };

                return RedirectToAction("index");
            }

            return View(model);
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult Delete1(Guid id)
        {
            var model = _shoppingCartService.Get(id);
            if (model == null)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Đơn đặt hàng không tồn tại",
                    MessageType = GenericMessages.warning
                };

                return RedirectToAction("index");
            }

            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    _shoppingCartService.Del(model);
                    _shoppingCartProductService.Del(model);


                    unitOfWork.Commit();

                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "Xóa đơn đặt hàng thành công",
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
                        Message = "Có lỗi xảy ra khi xóa đơn đặt hàng",
                        MessageType = GenericMessages.warning
                    };
                }
            }


            return View(model);
        }
        #endregion

    }
}