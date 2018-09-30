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
    public class AdminMembersController : BaseAdminController
    {
        public AdminMembersController(LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {

        }

        // GET: Admin/AdminMembership
        public ActionResult Index(int? p)
        {
            int limit = 10;
            var count = MembershipService.GetCount();

            var Paging = CalcPaging(limit, p, count);

            var viewModel = new AdminListMembersViewModel
            {
                Paging = Paging,
                ListMembers = MembershipService.GetList(limit, Paging.Page)
            };

            return View(viewModel);
        }

        #region Create Accout

        public ActionResult Create()
        {
            var viewModel = new AdminCreateMemberViewModel
            {

            };


            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdminCreateMemberViewModel viewModel)
        {
            if (ModelState.IsValid)
            {




            }
           
            return View(viewModel);
        }
        #endregion

        #region Edit info Accout
        public ActionResult Edit()
        {
            return View();
        }
        #endregion

        #region New pass
        public ActionResult NewPass(Guid id)
        {
            if (LoginUser.Id == id) return RedirectToAction("ChangePass");
            var user = MembershipService.Get(id,true);
            if(user == null) return RedirectToAction("index");
            if(user.UserName == AppConstants.AdminUsername)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Bạn không đủ quyền hạn để thực hiện thao tác này!",
                    MessageType = GenericMessages.warning
                };

                return RedirectToAction("index");
            }

            var viewModel = new AdminNewPassViewModel
            {
                UserName = user.UserName,
            };


            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPass(Guid id,AdminNewPassViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (LoginUser.Id == id) return RedirectToAction("ChangePass");
                var user = MembershipService.Get(id, true);
                if (user == null) return RedirectToAction("index");
                if (user.UserName == AppConstants.AdminUsername)
                {
                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "Bạn không đủ quyền hạn để thực hiện thao tác này!",
                        MessageType = GenericMessages.warning
                    };

                    return RedirectToAction("index");
                }

                // Check password is match
                if (viewModel.NewPassword != viewModel.NewPassword2)
                {
                    ModelState.AddModelError("NewPassword2", "Nhập lại mật khẩu mới không khớp");
                    return View(viewModel);
                }

                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var salt = user.PasswordSalt;
                        var newhash = StringUtils.GenerateSaltedHash(viewModel.NewPassword, salt);

                        user.Password = newhash;
                        user.LastPasswordChangedDate = DateTime.UtcNow;
                        MembershipService.Update(user);

                        unitOfWork.Commit();
                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Đổi mật khẩu thành công!",
                            MessageType = GenericMessages.success
                        };
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError("", "Có lỗi xảy ra khi đổi mật khẩu!");
                    }

                }
                    

            }
            
            return View(viewModel);
        }
        #endregion

        #region change info
        public ActionResult ChangeInfo()
        {
            var viewModel = new AdminChangeInfo
            {
                UserName = LoginUser.UserName,
                Email = LoginUser.Email,
            };


            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeInfo(AdminChangeInfo viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var user = MembershipService.GetUser(LoginUser.UserName);
                        user.Email = viewModel.Email;


                        MembershipService.Update(user);

                        unitOfWork.Commit();
                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Đổi thông tin thành công!",
                            MessageType = GenericMessages.success
                        };

                        return RedirectToAction("index", "Admin");
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError("", "Có lỗi xảy ra khi đổi thông tin tài khoản!");
                    }
                }
            }

            return View(viewModel);
        }
        #endregion

        #region change pass
        public ActionResult ChangePass()
        {
            var viewModel = new AdminChangePass
            {
                UserName = LoginUser.UserName,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePass(AdminChangePass viewModel)
        {
            if (ModelState.IsValid)
            {
                // Check password is match
                if (viewModel.NewPassword != viewModel.NewPassword2)
                {
                    ModelState.AddModelError("NewPassword2", "Nhập lại mật khẩu mới không khớp");
                    return View(viewModel);
                }

                if (viewModel.OldPassword == viewModel.NewPassword)
                {
                    ModelState.AddModelError("NewPassword", "Mật khẩu mới không được trùng với mật khẩu cũ");
                    return View(viewModel);
                }

                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var ischange = MembershipService.ChangePassword(LoginUser.UserName, viewModel.OldPassword, viewModel.NewPassword);
                        if (!ischange)
                        {
                            // Rollback transaction 
                            unitOfWork.Rollback();

                            ModelState.AddModelError("OldPassword", "Sai mật khẩu");
                            return View(viewModel);
                        }

                        unitOfWork.Commit();
                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Đổi mật khẩu thành công!",
                            MessageType = GenericMessages.success
                        };

                        return RedirectToAction("index", "Admin");
                    }
                    catch(Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError("", "Có lỗi xảy ra khi đổi mật khẩu!");
                    }
                }

            }

                return View(viewModel);
        }
        #endregion
    }
}