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
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public class AdminContactController : BaseAdminController
    {
        private readonly ContactService _contactSevice;

        public AdminContactController(ContactService contactSevice, LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {
            _contactSevice = contactSevice;
        }

        // GET: Admin/AdminContact
        public ActionResult Index()
        {
            AdminContactViewModel viewModel = new AdminContactViewModel
            {
                ListContact = _contactSevice.GetList(10, 1)
            };
            

            return View(viewModel);
        }

        public ActionResult Edit(Guid Id)
        {
            var contact = _contactSevice.Get(Id);
            if(contact == null)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = LocalizationService.GetResourceString("Errors.NoFindContact"),
                    MessageType = GenericMessages.warning
                };

                return RedirectToAction("Index");
            }


            var viewModel = new AdminContactEditViewModel
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                Content = contact.Content,
                IsCheck = contact.IsCheck,
                Note = contact.Note
            };


            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminContactEditViewModel viewModel)
        {
            //if (ModelState.IsValid)
            //{
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var contact = _contactSevice.Get(viewModel.Id);
                    if (contact == null)
                    {
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Errors.NoFindContact"),
                            MessageType = GenericMessages.warning
                        };

                        return RedirectToAction("Index");
                    }
                    viewModel.Name = contact.Name;
                    viewModel.Email = contact.Email;
                    viewModel.Content = contact.Content;

                    contact.IsCheck = viewModel.IsCheck;
                    contact.Note = viewModel.Note;
                    try
                    {
                        _contactSevice.Update(contact);

                        unitOfWork.Commit();
                    }
                    catch(Exception ex)
                    {
                        LoggingService.Error(ex.Message);
                        unitOfWork.Rollback();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Error.ContactEditError"),
                            MessageType = GenericMessages.warning
                        };
                    }
                }
            //}

            return View(viewModel);
        }


        #region delete
        public ActionResult Delete(Guid id)
        {
            var model = _contactSevice.Get(id);
            if (model == null)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Liê hệ không tồn tại",
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
            var model = _contactSevice.Get(id);
            if (model == null)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Liên hệ không tồn tại",
                    MessageType = GenericMessages.warning
                };

                return RedirectToAction("index");
            }

            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    _contactSevice.Del(model);


                    unitOfWork.Commit();

                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "Xóa liên hệ thành công",
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
                        Message = "Có lỗi xảy ra khi xóa liên hệ",
                        MessageType = GenericMessages.warning
                    };
                }
            }


            return View(model);
        }
        #endregion
    }
}