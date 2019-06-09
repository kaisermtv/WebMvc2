using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
    public class OverviewController : BaseAdminController
    {
        public OverviewController(LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {

        }

        public ActionResult index()
        {
            return RedirectToAction("General");
        }


        #region General Setting
        public ActionResult General()
        {
            var model = new AdminGeneralSettingViewModel
            {
                WebsiteName = SettingsService.GetSetting(AppConstants.STWebsiteName),
                WebsiteUrl = SettingsService.GetSetting(AppConstants.STWebsiteUrl),
                PageTitle = SettingsService.GetSetting(AppConstants.STPageTitle),
                MetaDesc = SettingsService.GetSetting(AppConstants.STMetaDesc),
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
                        SettingsService.SetSetting(AppConstants.STWebsiteName, setting.WebsiteName);
                        SettingsService.SetSetting(AppConstants.STWebsiteUrl, setting.WebsiteUrl);
                        SettingsService.SetSetting(AppConstants.STPageTitle, setting.PageTitle);
                        SettingsService.SetSetting(AppConstants.STMetaDesc, setting.MetaDesc);


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
                BusinessName = SettingsService.GetSetting(AppConstants.STBusinessName),
                RepresentAtive = SettingsService.GetSetting(AppConstants.STRepresentAtive),
                RepresentPosition = SettingsService.GetSetting(AppConstants.STRepresentPosition),
                Introduce = SettingsService.GetSetting(AppConstants.STIntroduce),
                Greeting = SettingsService.GetSetting(AppConstants.STGreeting),

                Fanpage = SettingsService.GetSetting(AppConstants.STFanpage),
                FanChat = SettingsService.GetSetting(AppConstants.STFanChat),
                Hotline = SettingsService.GetSetting(AppConstants.STHotline),
                HotlineImg = SettingsService.GetSetting(AppConstants.STHotlineImg),
                Addrens = new List<AdminShowroomSettingViewModel>(),

                BankID = SettingsService.GetSetting(AppConstants.STBankID),
                BankName = SettingsService.GetSetting(AppConstants.STBankName),
                BankUser = SettingsService.GetSetting(AppConstants.STBankUser),
                BankPay = SettingsService.GetSetting(AppConstants.STBankPay),
            };

            var ShowroomCount = SettingsService.GetSetting(AppConstants.STShowroomCount);
            int count = 0;
            try
            {
                count = int.Parse(ShowroomCount);
            }
            catch { }

            for (int i = 0; i < count; i++)
            {
                model.Addrens.Add(new AdminShowroomSettingViewModel
                {
                    Addren = SettingsService.GetSetting("Showroom[" + i + "].Address"),
                    iFrameMap = SettingsService.GetSetting("Showroom[" + i + "].iFrameMap"),
                    Tel = SettingsService.GetSetting("Showroom[" + i + "].Tel"),
                    Hotline = SettingsService.GetSetting("Showroom[" + i + "].Hotline"),
                    Name = SettingsService.GetSetting("Showroom[" + i + "].Name"),
                });
            }

            return View(model);
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
                        SettingsService.SetSetting(AppConstants.STBusinessName, setting.BusinessName);
                        SettingsService.SetSetting(AppConstants.STIntroduce, setting.Introduce);
                        SettingsService.SetSetting(AppConstants.STGreeting, setting.Greeting);
                        SettingsService.SetSetting(AppConstants.STFanpage, setting.Fanpage);
                        SettingsService.SetSetting(AppConstants.STFanChat, setting.FanChat);
                        SettingsService.SetSetting(AppConstants.STHotline, setting.Hotline);
                        SettingsService.SetSetting(AppConstants.STHotlineImg, setting.HotlineImg);

                        SettingsService.SetSetting(AppConstants.STBankID, setting.BankID);
                        SettingsService.SetSetting(AppConstants.STBankName, setting.BankName);
                        SettingsService.SetSetting(AppConstants.STBankUser, setting.BankUser);

                        if (setting.Addrens != null)
                        {
                            int count = setting.Addrens.Count;
                            SettingsService.SetSetting(AppConstants.STShowroomCount, count.ToString());

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
                            SettingsService.SetSetting(AppConstants.STShowroomCount, "0");
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
    }
}