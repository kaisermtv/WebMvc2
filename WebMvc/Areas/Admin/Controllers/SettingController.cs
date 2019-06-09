using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using WebMvc.Application;
using WebMvc.Application.Context;
using WebMvc.Application.Entities;
using WebMvc.Application.Extensions;
using WebMvc.Application.Interfaces;
using WebMvc.Application.Lib;
using WebMvc.Areas.Admin.ViewModels;
using WebMvc.Services;

namespace WebMvc.Areas.Admin.Controllers
{
    //[Authorize(Roles = AppConstants.AdminRoleName)]
    public class SettingController : BaseAdminController
    {
        public SettingController(LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager,MembershipService membershipService, SettingsService settingsService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {

        }

        //public ActionResult index()
        //{
        //    return General();
        //}

        #region Email Setting
        public ActionResult Email()
        {
            var viewModel = new AdminEmailSettingViewModel
            {
                InEmail = SettingsService.GetSetting(AppConstants.STInEmail),
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Email(AdminEmailSettingViewModel setting)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        SettingsService.SetSetting(AppConstants.STInEmail, setting.InEmail);


                        unitOfWork.Commit();
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Cập nhật thành công!"),
                            MessageType = GenericMessages.success
                        };
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                    }
                }

            }

            return View(setting);
        }
        #endregion Email Setting

        #region Registration Setting
        public ActionResult Registration()
        {
            var model = new AdminRegistrationSettingViewModel
            {
                LockRegister = SettingsService.GetSetting(AppConstants.STLockRegister).ToBool(),
                //WebsiteName = SettingsService.GetSetting("WebsiteName"),
                //WebsiteUrl = SettingsService.GetSetting("WebsiteUrl"),
                //PageTitle = SettingsService.GetSetting("PageTitle"),
                //MetaDesc = SettingsService.GetSetting("MetaDesc"),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(AdminRegistrationSettingViewModel setting)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        SettingsService.SetSetting(AppConstants.STLockRegister, setting.LockRegister);


                        unitOfWork.Commit();
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Cập nhật thành công!"),
                            MessageType = GenericMessages.success
                        };
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                    }
                }

            }

            return View(setting);
        }
        #endregion Registration Setting

        #region Language Setting
        public ActionResult Language()
        {
            var model = new AdminLanguageSettingViewModel
            {
                LanguageDefault = LocalizationService.DefaultLanguage.Id,
                AllLanguage = LocalizationService.GetBaseSelectListLanguages(LocalizationService.GetAll())
            };

            return View("Language", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Language(AdminLanguageSettingViewModel setting)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    setting.AllLanguage = LocalizationService.GetBaseSelectListLanguages(LocalizationService.GetAll());

                    try
                    {
                        SettingsService.SetSetting("LanguageDefault", setting.LanguageDefault.ToString());


                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                    }
                }

            }



            return View("Language", setting);
        }
        #endregion

        #region Themes Setting
        public ActionResult Themes()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Themes(string activetheme,string atv)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        if(atv == "Deaactive")
                        {
                            SettingsService.SetSetting("Theme", "");
                        }
                        else
                        {
                            SettingsService.SetSetting("Theme", activetheme);
                        }

                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                    }
                }

            }

            return View();
        }

        public ActionResult ThemeConfig(string id)
        {
            ViewBag.id = id;
            ViewBag.json = ThemesSetting.getSettingTheme(id);
            return View();
        }

        [HttpPost]
        [ActionName("ThemeConfig")]
        public ActionResult ThemeConfig1(string id)
        {
            //Request.Form["svd"]
            ViewBag.id = id;
            ViewBag.json = ThemesSetting.getSettingTheme(id);

            foreach (var item in ViewBag.json)
            {
				switch ((string)item.Value.Type)
				{
					case "Number":
					case "Text":
					case "MenuID":
                    case "CategoryID":
                    case "NewsID":
                    case "ProductID":
                    case "ProductClassID":
					case "CarouselID":
						string buf = Request.Form[(string)item.Name];
                        item.Value.Value = buf;
                        break;
                    case "ListProductClass":
                    case "ListNews":
					case "ListCategoryProduct":
						var lst = new List<string>();

						int i = 1;
						while (true)
                        {
                            string key = string.Concat(item.Name,"[",i,"]");
                            string text = Request.Form[key];
                            if (text == null) break;
                            if(text != "")
                            {
                                lst.Add(text);
                            }
                            i++;
                        }

                        item.Value.Value = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(lst));
                        break;
					
				}
                

                
            }

            ThemesSetting.setSettingTheme(id, ViewBag.json);

            return View("ThemeConfig");
        }
        #endregion Themes Setting

        #region CustomCode
        public ActionResult CustomCode()
        {
            var viewModel = new CustomCodeViewModels
            {
                CustomFooterCode = SettingsService.GetSetting("CustomFooterCode"),
                CustomHeaderCode = SettingsService.GetSetting("CustomHeaderCode")
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomCode(CustomCodeViewModels setting)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        SettingsService.SetSetting("CustomFooterCode", setting.CustomFooterCode);
                        SettingsService.SetSetting("CustomHeaderCode", setting.CustomHeaderCode);


                        unitOfWork.Commit();
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Cập nhật thành công!"),
                            MessageType = GenericMessages.success
                        };
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                    }
                }

            }
            
            return View(setting);
        }
        #endregion
    }
}