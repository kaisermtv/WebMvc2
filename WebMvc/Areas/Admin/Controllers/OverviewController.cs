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
                WebSiteImage = SettingsService.GetSetting(AppConstants.STWebsiteImage),
                PageTitle = SettingsService.GetSetting(AppConstants.STPageTitle),
                MetaDesc = SettingsService.GetSetting(AppConstants.STMetaDesc),
                Keyword = SettingsService.GetSetting(AppConstants.STKeyword),
                WebsiteFooter = SettingsService.GetSetting(AppConstants.STWebsiteFooter),


                Facebook = SettingsService.GetSetting(AppConstants.STFacebook),
                Twister = SettingsService.GetSetting(AppConstants.STTwister),
                Instagram = SettingsService.GetSetting(AppConstants.STInstagram),
                Linker = SettingsService.GetSetting(AppConstants.STLinker),
                Skype = SettingsService.GetSetting(AppConstants.STSkype),
                YouTube = SettingsService.GetSetting(AppConstants.STYouTube),
                Google = SettingsService.GetSetting(AppConstants.STGoogle),
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
                        SettingsService.SetSetting(AppConstants.STWebsiteImage, setting.WebSiteImage);
                        SettingsService.SetSetting(AppConstants.STPageTitle, setting.PageTitle);
                        SettingsService.SetSetting(AppConstants.STMetaDesc, setting.MetaDesc);
                        SettingsService.SetSetting(AppConstants.STKeyword, setting.Keyword);
                        SettingsService.SetSetting(AppConstants.STWebsiteFooter, setting.WebsiteFooter);


                        SettingsService.SetSetting(AppConstants.STFacebook, setting.Facebook);
                        SettingsService.SetSetting(AppConstants.STTwister, setting.Twister);
                        SettingsService.SetSetting(AppConstants.STInstagram, setting.Instagram);
                        SettingsService.SetSetting(AppConstants.STLinker, setting.Linker);
                        SettingsService.SetSetting(AppConstants.STSkype, setting.Skype);
                        SettingsService.SetSetting(AppConstants.STYouTube, setting.YouTube);
                        SettingsService.SetSetting(AppConstants.STGoogle, setting.Google);


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

                //Fanpage = SettingsService.GetSetting(AppConstants.STFanpage),
                //FanChat = SettingsService.GetSetting(AppConstants.STFanChat),
                //Hotline = SettingsService.GetSetting(AppConstants.STHotline),
                //HotlineImg = SettingsService.GetSetting(AppConstants.STHotlineImg),
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
                        //SettingsService.SetSetting(AppConstants.STGreeting, setting.Greeting);
                        //SettingsService.SetSetting(AppConstants.STFanpage, setting.Fanpage);
                        //SettingsService.SetSetting(AppConstants.STFanChat, setting.FanChat);
                        //SettingsService.SetSetting(AppConstants.STHotline, setting.Hotline);
                        //SettingsService.SetSetting(AppConstants.STHotlineImg, setting.HotlineImg);

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

        #region Contact Information
        public ActionResult ContactInformation()
        {
            var model = new AdminContactInformationSettingViewModel
            {
                MainAddress = SettingsService.GetSetting(AppConstants.STMainAddress),
                Hotline = SettingsService.GetSetting(AppConstants.STHotline),

                Phone = new List<string>(),
                Email = new List<string>(),
            };

            var PhoneCount = SettingsService.GetSetting(AppConstants.STPhoneCount);
            int count = 0;
            try
            {
                count = int.Parse(PhoneCount);
            }
            catch { }

            for (int i = 0; i < count; i++)
            {
                model.Phone.Add(SettingsService.GetSetting("Phone[" + i + "]"));
            }

            var EmailCount = SettingsService.GetSetting(AppConstants.STEmailCount);
            int ecount = 0;
            try
            {
                ecount = int.Parse(EmailCount);
            }
            catch { }

            for (int i = 0; i < ecount; i++)
            {
                model.Email.Add(SettingsService.GetSetting("Email[" + i + "]"));
            }

            return View("ContactInformation", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactInformation(AdminContactInformationSettingViewModel setting)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        SettingsService.SetSetting(AppConstants.STMainAddress, setting.MainAddress);
                        SettingsService.SetSetting(AppConstants.STHotline, setting.Hotline);


                        if (setting.Phone != null)
                        {
                            int count = setting.Phone.Count;
                            SettingsService.SetSetting(AppConstants.STPhoneCount, count.ToString());

                            for (int i = 0; i < count; i++)
                            {
                                SettingsService.SetSetting("Phone[" + i + "]", setting.Phone[i]);
                            }
                        }
                        else
                        {
                            SettingsService.SetSetting(AppConstants.STPhoneCount, "0");
                            setting.Phone = new List<string>();
                        }

                        if (setting.Email != null)
                        {
                            int count = setting.Email.Count;
                            SettingsService.SetSetting(AppConstants.STEmailCount, count.ToString());

                            for (int i = 0; i < count; i++)
                            {
                                SettingsService.SetSetting("Email[" + i + "]", setting.Email[i]);
                            }
                        }
                        else
                        {
                            SettingsService.SetSetting(AppConstants.STEmailCount, "0");
                            setting.Email = new List<string>();
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

            return View("ContactInformation", setting);
        }

        #endregion General Setting

        #region TermsConditions Setting
        public ActionResult TermsConditions()
        {
            var model = new AdminTermsConditionsSettingViewModel
            {
                TermsConditions = SettingsService.GetSetting(AppConstants.STTermsConditions),

            };

            return View("TermsConditions", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TermsConditions(AdminTermsConditionsSettingViewModel setting)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        SettingsService.SetSetting(AppConstants.STTermsConditions, setting.TermsConditions);


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

            return View("TermsConditions", setting);
        }
        #endregion TermsConditions Setting
    }
}