﻿using Newtonsoft.Json;
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
    public class SettingController : BaseAdminController
    {
        public SettingController(LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager,MembershipService membershipService, SettingsService settingsService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {

        }

        public ActionResult index()
        {
            return General();
        }

        #region General Setting
        public ActionResult General()
        {
            var model = new AdminGeneralSettingViewModel
            {
                WebsiteName = SettingsService.GetSetting("WebsiteName"),
                WebsiteUrl = SettingsService.GetSetting("WebsiteUrl"),
                PageTitle = SettingsService.GetSetting("PageTitle"),
                MetaDesc = SettingsService.GetSetting("MetaDesc"),
            };

            return View("General", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult General(AdminGeneralSettingViewModel setting)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        SettingsService.SetSetting("WebsiteName", setting.WebsiteName);
                        SettingsService.SetSetting("WebsiteUrl", setting.WebsiteUrl);
                        SettingsService.SetSetting("PageTitle", setting.PageTitle);
                        SettingsService.SetSetting("MetaDesc", setting.MetaDesc);


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



            return View("General", setting);
        }

        #endregion General Setting

        #region company information
        public ActionResult Business()
        {
            var model = new AdminBusinessSettingViewModel 
            {
                BusinessName = SettingsService.GetSetting("BusinessName"),
                RepresentAtive = SettingsService.GetSetting("RepresentAtive"),
                RepresentPosition = SettingsService.GetSetting("RepresentPosition"),
                Introduce = SettingsService.GetSetting("Introduce"),
                Greeting = SettingsService.GetSetting("Greeting"),

                Fanpage = SettingsService.GetSetting("Fanpage"),
                FanChat = SettingsService.GetSetting("FanChat"),
                Hotline = SettingsService.GetSetting("Hotline"),
                HotlineImg = SettingsService.GetSetting("HotlineImg"),
                Addrens = new List<AdminShowroomSettingViewModel>(),

                BankID = SettingsService.GetSetting("BankID"),
                BankName = SettingsService.GetSetting("BankName"),
                BankUser = SettingsService.GetSetting("BankUser"),
                BankPay = SettingsService.GetSetting("BankPay"),
            };

            var ShowroomCount = SettingsService.GetSetting("ShowroomCount");
            int count = 0;
            try
            {
                count = int.Parse(ShowroomCount);
            }
            catch { }

            for(int i = 0; i < count; i++)
            {
                model.Addrens.Add(new AdminShowroomSettingViewModel {
                   Addren = SettingsService.GetSetting("Showroom["+i+"].Address"),
                   iFrameMap = SettingsService.GetSetting("Showroom[" + i + "].iFrameMap"),
                   Tel = SettingsService.GetSetting("Showroom[" + i + "].Tel"),
                   Hotline = SettingsService.GetSetting("Showroom[" + i + "].Hotline"),
                   Name = SettingsService.GetSetting("Showroom[" + i + "].Name"),
				});
            }

            return View( model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Business(AdminBusinessSettingViewModel setting)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        SettingsService.SetSetting("BusinessName", setting.BusinessName);
                        SettingsService.SetSetting("Introduce", setting.Introduce);
                        SettingsService.SetSetting("Greeting", setting.Greeting); 
                        SettingsService.SetSetting("Fanpage", setting.Fanpage);
                        SettingsService.SetSetting("FanChat", setting.FanChat);
                        SettingsService.SetSetting("Hotline", setting.Hotline);
                        SettingsService.SetSetting("HotlineImg", setting.HotlineImg);
                        
                        SettingsService.SetSetting("BankID", setting.BankID);
                        SettingsService.SetSetting("BankName", setting.BankName);
                        SettingsService.SetSetting("BankUser", setting.BankUser);

                        if (setting.Addrens != null)
                        {
                            int count = setting.Addrens.Count;
                            SettingsService.SetSetting("ShowroomCount", count.ToString());

                            for (int i = 0; i < count; i++)
                            {
                                SettingsService.SetSetting("Showroom[" + i + "].Address", setting.Addrens[i].Addren);
                                SettingsService.SetSetting("Showroom[" + i + "].iFrameMap", setting.Addrens[i].iFrameMap);
                                SettingsService.SetSetting("Showroom[" + i + "].Tel", setting.Addrens[i].Tel);
                                SettingsService.SetSetting("Showroom[" + i + "].Hotline", setting.Addrens[i].Hotline);
                                SettingsService.SetSetting("Showroom[" + i + "].Name", setting.Addrens[i].Name);
							}
                        }
                        else
                        {
                            SettingsService.SetSetting("ShowroomCount", "0");
                            setting.Addrens = new List<AdminShowroomSettingViewModel>();
                        }
                        

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

        #region TermsConditions Setting
        public ActionResult TermsConditions()
        {

            return View();
        }
        #endregion TermsConditions Setting

        #region Email Setting
        public ActionResult Email()
        {
            var viewModel = new AdminEmailSettingViewModel
            {
                InEmail = SettingsService.GetSetting("InEmail"),
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
                        SettingsService.SetSetting("InEmail", setting.InEmail);


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

            return View();
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