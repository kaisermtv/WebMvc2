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
    public class PermissionsController : BaseAdminController
    {
        protected readonly PermissionService _permissionService;

        public PermissionsController(PermissionService permissionService,LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {
            _permissionService = permissionService;
        }

        // GET: Admin/Permissions
        public ActionResult Index()
        {
            var viewModel = new PermissionsViewModel
            {
                Permissions = _permissionService.GetAll()
            };


            return View(viewModel);
        }

        public ActionResult Create()
        {
            var viewModel = new EditPermissionsViewModel
            {
                Permissions = _permissionService.GetBaseSelectListPermissions(_permissionService.GetAll())
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EditPermissionsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var permission = new Permission
                        {
                            Name = viewModel.Name,
                            PermissionId = viewModel.PermissionId,
                            Lock = false
                        };

                        _permissionService.Add(permission);

                        unitOfWork.Commit();
                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Thêm quyền hạn thành công",
                            MessageType = GenericMessages.success
                        };

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError("", "Có lỗi xảy ra khi thêm quyền hạn!");
                    }
                }
            }

            //viewModel.Permissions = _permissionService.GetBaseSelectListPermissions(_permissionService.GetPermissionsParenPermission());
            viewModel.Permissions = _permissionService.GetBaseSelectListPermissions(_permissionService.GetAll());

            return View(viewModel);
        }

        public ActionResult Edit(Guid Id)
        {
            var permission = _permissionService.Get(Id);
            if (permission == null) return RedirectToAction("Index");

            var viewModel = new EditPermissionsViewModel
            {
                Id = permission.Id,
                Name = permission.Name,
                Lock = permission.Lock,
                PermissionId = permission.PermissionId,
                Permissions = _permissionService.GetBaseSelectListPermissions(_permissionService.GetPermissionsParenPermission(permission))
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditPermissionsViewModel viewModel)
        {
            var permission = _permissionService.Get(viewModel.Id,false);
            if (permission == null) return RedirectToAction("Index");
            if(permission.Lock) return RedirectToAction("Edit",new { Id = viewModel.Id });

            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        permission.Name = viewModel.Name;
                        permission.PermissionId = viewModel.PermissionId;

                        _permissionService.Update(permission);

                        unitOfWork.Commit();
                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Sửa quyền hạn thành công",
                            MessageType = GenericMessages.success
                        };

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError("", "Có lỗi xảy ra khi sửa quyền hạn!");
                    }
                }
            }

            viewModel.Lock = permission.Lock;
            viewModel.Permissions = _permissionService.GetBaseSelectListPermissions(_permissionService.GetPermissionsParenPermission(permission));

            return View(viewModel);
        }

        #region delete
        public ActionResult Del(Guid id)
        {
            var model = _permissionService.Get(id);
            if (model == null)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Quyền hạn không tồn tại",
                    MessageType = GenericMessages.warning
                };

                return RedirectToAction("index");
            }

            if (model.Lock)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Bạn không đủ quyền hạn để thực hiwnj thao tác này.",
                    MessageType = GenericMessages.warning
                };
                return RedirectToAction("index");
            }

            return View(model);
        }

        [HttpPost]
        [ActionName("Del")]
        public ActionResult Del1(Guid id)
        {
            var model = _permissionService.Get(id);
            if (model == null)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Quyền hạn không tồn tại",
                    MessageType = GenericMessages.warning
                };

                return RedirectToAction("index");
            }

            if (model.Lock)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Bạn không đủ quyền hạn để thực hiwnj thao tác này.",
                    MessageType = GenericMessages.warning
                };
                return RedirectToAction("index");
            }

            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    _permissionService.Delete(model.Id);

                    unitOfWork.Commit();

                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "Xóa quyền hạn thành công",
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
                        Message = "Có lỗi xảy ra khi xóa quyền hạn",
                        MessageType = GenericMessages.warning
                    };
                }
            }


            return View(model);
        }
        #endregion
    }
}