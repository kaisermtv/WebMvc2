using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using static WebMvc.Areas.Admin.ViewModels.ListLanguagesViewModel;

namespace WebMvc.Areas.Admin.Controllers
{
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public class AdminLanguageController : BaseAdminController
    {
        public AdminLanguageController( LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult ImportExport()
        {
            return View();
        }

		public ActionResult DeleteLanguage(Guid Id)
		{
			var language = LocalizationService.Get(Id);
			if (language == null) return RedirectToAction("Index");

			return View(language);
		}

		[HttpPost]
		[ActionName("DeleteLanguage")]
		public ActionResult DeleteLanguage1(Guid Id)
		{
			var language = LocalizationService.Get(Id);
			if (language == null) return RedirectToAction("Index");

			using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
			{
				try
				{
					LocalizationService.Delete(language);
					
					unitOfWork.Commit();
					LocalizationService.Clear();
					TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
					{
						Message = LocalizationService.GetResourceString("Xóa gói ngôn ngữ thành công"),
						MessageType = GenericMessages.success
					};
					return RedirectToAction("Index");
				}
				catch(Exception ex)
				{
					unitOfWork.Rollback();
					LoggingService.Error(ex.Message);
					TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
					{
						Message = LocalizationService.GetResourceString("Có lỗi xảy ra!"),
						MessageType = GenericMessages.warning
					};
				}
			}

				return View(language);
		}


		public ActionResult DeleteResourceKey(Guid Id)
		{
			var LocaleResourceKey = LocalizationService.GetResourceKey(Id);
			if (LocaleResourceKey == null) return RedirectToAction("ManageResourceKeys");

			return View(LocaleResourceKey);
		}

		[HttpPost]
		[ActionName("DeleteResourceKey")]
		public ActionResult DeleteResourceKey1(Guid Id)
		{
			var LocaleResourceKey = LocalizationService.GetResourceKey(Id);
			if (LocaleResourceKey == null) return RedirectToAction("ManageResourceKeys");

			using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
			{
				try
				{
					LocalizationService.Delete(LocaleResourceKey);

					unitOfWork.Commit();
					LocalizationService.Clear();
					TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
					{
						Message = LocalizationService.GetResourceString("Xóa khóa ngôn ngữ thành công"),
						MessageType = GenericMessages.success
					};
					return RedirectToAction("Index");
				}
				catch (Exception ex)
				{
					unitOfWork.Rollback();
					LoggingService.Error(ex.Message);
					TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
					{
						Message = LocalizationService.GetResourceString("Có lỗi xảy ra!"),
						MessageType = GenericMessages.warning
					};
				}
			}

			return View(LocaleResourceKey);
		}


		/// <summary>
		/// Manage resource keys (for all languages)
		/// </summary>
		/// <returns></returns>
		public ActionResult ManageResourceKeys(int? p, string search)
        {
			int limit = 30;
			var count = LocalizationService.GetCountKey(search);

			var Paging = CalcPaging(limit, p, count);

			var resourceListModel = new ResourceKeyListViewModel
			{
				Search = search,
				Paging = Paging,
				ResourceKeys = LocalizationService.GetListKey(search, Paging.Page, limit)
			};

			return View("ListKeys", resourceListModel);
        }

        /// <summary>
        /// Get - create a new language
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public PartialViewResult CreateLanguage()
        {
            return PartialView();
        }

        /// <summary>
        /// Post - create a new language
        /// </summary>
        /// <param name="languageViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.AdminRoleName)]
        public ActionResult CreateLanguage(CreateLanguageViewModel languageViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Get the culture info
                    var cultureInfo = LanguageUtils.GetCulture(languageViewModel.Name);

                    using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                    {
                        try
                        {
                            LocalizationService.Add(cultureInfo);
                            unitOfWork.Commit();
                            ShowSuccess("Language Created");
                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();
                            LoggingService.Error(ex);
                            throw;
                        }
                    }
                }
                else
                {

                    var errors = (from key in ModelState.Keys select ModelState[key] into state where state.Errors.Any() select state.Errors.First().ErrorMessage).ToList();
                    ShowErrors(errors);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }

            // Default ie error
            return RedirectToAction("Index");
        }

        public ActionResult AddResourceKey()
        {
            var viewModel = new LocaleResourceKeyViewModel();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddResourceKey(LocaleResourceKeyViewModel newResourceKeyViewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var resourceKeyToSave = new LocaleResourceKey
                    {
                        Name = newResourceKeyViewModel.Name,
                        Notes = newResourceKeyViewModel.Notes
                    };

                    LocalizationService.Add(resourceKeyToSave);
                    unitOfWork.Commit();
					LocalizationService.Clear();
					ShowSuccess("Resource key created successfully");
                    //var currentLanguage = SettingsService.GetSetting("DefaultLanguage");
                    return RedirectToAction("ManageResourceKeys");
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    ShowError(ex.Message);
                    LoggingService.Error(ex);
                    return RedirectToAction("AddResourceKey");
                }
            }
        }

        [HttpPost]
        public void UpdateResourceValue(AjaxEditLanguageValueViewModel viewModel)
        {
            if (Request.IsAjaxRequest())
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        LocalizationService.UpdateResourceString(viewModel.LanguageId, viewModel.ResourceKey,
                                                                 viewModel.NewValue);
                        unitOfWork.Commit();
						LocalizationService.Clear();
					}
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a partial view listing all languages
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public PartialViewResult GetLanguages()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var viewModel = new ListLanguagesViewModel { Languages = new List<LanguageDisplayViewModel>() };

                try
                {
                    foreach (var language in LocalizationService.GetAll())
                    {
                        var languageViewModel = new LanguageDisplayViewModel
                        {
                            Id = language.Id,
                            IsDefault = language.Id == LocalizationService.CurrentLanguage.Id,
                            Name = language.Name,
                            LanguageCulture = language.LanguageCulture,
                        };

                        viewModel.Languages.Add(languageViewModel);

                    }
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                    LoggingService.Error(ex);
                }

                return PartialView(viewModel);
            }
        }

        public ActionResult ManageLanguageResourceValues(Guid languageId, int? p, string search = "",string SearchKeys = "")
        {
			var language = LocalizationService.Get(languageId);
			if (language == null) return RedirectToAction("Index");

			int limit = 30;
			var count = LocalizationService.GetListKeyResourceCount(language.Id, SearchKeys, search);

			var Paging = CalcPaging(limit, p, count);

			var resourceListModel = new LanguageListResourcesViewModel
			{
				LanguageId = language.Id,
				LanguageName = language.Name,
				Search = search,
				SearchKeys = SearchKeys,
				Paging = Paging,
				LocaleResources = LocalizationService.GetListKeyResource(language.Id, SearchKeys, search, Paging.Page, limit)
			};
			
			return View("ListValues", resourceListModel);
		}
		
		#region Private
		
        /// <summary>
        /// Create a message to be displayed as an error
        /// </summary>
        /// <param name="message"></param>
        private void ShowError(string message)
        {
            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
            {
                Message = message,
                MessageType = GenericMessages.danger
            };
        }

        /// <summary>
        /// Create a message to be displayed when some action is successful
        /// </summary>
        /// <param name="message"></param>
        private void ShowSuccess(string message)
        {
            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
            {
                Message = message,
                MessageType = GenericMessages.success
            };
        }

        /// <summary>
        /// Create a message to be displayed as an error from 
        /// a set of messages
        /// </summary>
        /// <param name="messages"></param>
        private void ShowErrors(IEnumerable<string> messages)
        {
            var errors = new StringBuilder();

            foreach (var message in messages)
            {
                errors.AppendLine(message);
            }

            ShowError(errors.ToString());
        }
		
        #endregion

    }
}