using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using WebMvc.Application;
using WebMvc.Application.Context;
using WebMvc.Application.Entities;
using WebMvc.Application.Interfaces;
using WebMvc.Application.Lib;
using WebMvc.Areas.Admin.ViewModels;
using WebMvc.Services;
using WebMvc.ViewModels;

namespace WebMvc.Areas.Admin.Controllers
{
    //[Authorize(Roles = AppConstants.AdminRoleName)]
    public class AdminTopicController : BaseAdminController
    {

        private readonly CategoryService _categoryService;
        private readonly TopicService _topicServic;
        private readonly PostSevice _postSevice;

        public AdminTopicController(PostSevice postSevice, TopicService topicService, CategoryService categoryService, LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {
            _categoryService = categoryService;
            _topicServic = topicService;
            _postSevice = postSevice;
        }
		// GET: Admin/AdminTopic
		public ActionResult Index(int? p, Guid? catid,string seach = null)
		{
			Category cat = null;
			if (catid != null)
			{
				cat = _categoryService.Get((Guid)catid);
			}

			int count = _topicServic.GetCount(cat?.Id,seach);
            int limit = 10;
			//if (cat != null)
			//{
			//	count = _topicServic.GetCount(cat.Id);
			//}
			//else
			//{
			//	count = _topicServic.GetCount();
			//}
			var Paging = CalcPaging(limit, p, count);

			List<Topic> lst = _topicServic.GetList(cat?.Id,seach,limit,Paging.Page);
			//if (cat != null)
			//{
			//	lst = _topicServic.GetList(cat.Id, limit, Paging.Page);

			//}
			//else
			//{
			//	lst = _topicServic.GetList(limit, Paging.Page);
			//}

			var model = new AdminTopicListViewModel
			{
                Seach = seach,
                Cat = cat,
				Paging = Paging,
				ListTopic = lst,
				//AllCategories = _categoryService.GetBaseSelectListCategories(_categoryService.GetList(false))
			};

			return View(model);
		}


        public ActionResult PopupSelect(string seach, string cat, int? p)
        {
            int limit = 10;
            var count = _topicServic.GetCount();

            var Paging = CalcPaging(limit, p, count);

            var viewModel = new AdminTopicListViewModel
            {
                Paging = Paging,
                ListTopic = _topicServic.GetList(limit, Paging.Page)
            };
            return PartialView(viewModel);
        }
        

        #region Create Topic
        public ActionResult Create()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var cats = _categoryService.GetAllowedEditCategories(UsersRole,false);
                if (cats.Count > 0)
                {
                    var viewModel = new AdminCreateEditTopicViewModel();
                    viewModel.Categories = _categoryService.GetBaseSelectListCategories(cats);

                    
                    return View(viewModel);
                }
                return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdminCreateEditTopicViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var cats = _categoryService.GetAllowedEditCategories(UsersRole, false);
                if (cats.Count > 0)
                {
                    if (ModelState.IsValid)
                    {
                        if (CheckCats(viewModel.Category, cats))
                        {
                            var topic = new Topic();
                            var post = new Post();

                            topic.Name = viewModel.Name;
                            topic.Category_Id = viewModel.Category;
                            topic.Image = viewModel.Image;
                            topic.IsLocked = viewModel.IsLocked;
                            topic.IsSticky = viewModel.IsSticky;
                            topic.MembershipUser_Id = LoggedOnReadOnlyUser.Id;
                            topic.Post_Id = post.Id;
                            
                            post.PostContent = viewModel.Content;
                            post.MembershipUser_Id = LoggedOnReadOnlyUser.Id;
                            post.Topic_Id = topic.Id;
                            post.IsTopicStarter = true;

                            topic.ShotContent = string.Concat(StringUtils.ReturnAmountWordsFromString(StringUtils.StripHtmlFromString(post.PostContent), 50), "....");
                            topic.isAutoShotContent = true;
							var i = topic.ShotContent.Length;
							if (i > 450)
							{
								topic.ShotContent = topic.ShotContent.Substring(0, 440) + "...";
							}

							try
                            {
                                _topicServic.Add(topic);
                                _postSevice.Add(post);
                                

                                unitOfWork.Commit();

                                return RedirectToAction("Edit",new { Id = topic.Id });
                            }
                            catch (Exception ex)
                            {
                                LoggingService.Error(ex.Message);
                                unitOfWork.Rollback();
                            }
                        }
                        else
                        {
                            //viewModel.Category = null;
                            //No permission to create a Poll so show a message but create the topic
                            //TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                            //{
                            //    Message = LocalizationService.GetResourceString("Errors.NoPermissionCatergory"),
                            //    MessageType = GenericMessages.info
                            //};
                            ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.CatergoryMessage"));
                        }

                    }
                    viewModel.Categories = _categoryService.GetBaseSelectListCategories(cats);
                    return View(viewModel);
                }
                return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
            }
        }
        #endregion

        #region Edit Topic
        public ActionResult Edit(Guid Id)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var cats = _categoryService.GetAllowedEditCategories(UsersRole,false);
                if (cats.Count > 0)
                {
                    var viewModel = new AdminCreateEditTopicViewModel();
                    viewModel.Categories = _categoryService.GetBaseSelectListCategories(cats);

                    var topic = _topicServic.Get(Id);

                    if(topic == null)
                    {
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Errors.NoFindTopic"),
                            MessageType = GenericMessages.warning
                        };

                        return RedirectToAction("Index");
                    }

                    viewModel.Id = topic.Id;
                    viewModel.Name = topic.Name;
                    viewModel.Category = topic.Category_Id;
                    viewModel.IsLocked = topic.IsLocked;
                    viewModel.IsSticky = topic.IsSticky;
                    viewModel.Image = topic.Image;

                    if (topic.Post_Id != null)
                    {
                        var post = _postSevice.Get((Guid)topic.Post_Id);
                        if (post != null)
                        {
                            viewModel.Content = post.PostContent;
                        }
                    }
                    //viewModel.Po = topic.IsLocked;



                    return View(viewModel);
                }
                return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));

            }
        }

        [HttpPost]
        //[Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminCreateEditTopicViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var cats = _categoryService.GetAllowedEditCategories(UsersRole,false);
                if (cats.Count > 0)
                {
                    if (ModelState.IsValid)
                    {
                        if (CheckCats(viewModel.Category, cats))
                        {
                            var topic = _topicServic.Get(viewModel.Id);

                            if (topic == null)
                            {
                                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                                {
                                    Message = LocalizationService.GetResourceString("Errors.NoFindTopic"),
                                    MessageType = GenericMessages.warning
                                };

                                return RedirectToAction("Index");
                            }

                            
                            bool pn = false;
                            Post post;
                            if (topic.Post_Id != null)
                            {
                                post = _postSevice.Get((Guid)topic.Post_Id);
                            } else
                            {
                                post = new Post();
                                pn = true;
                            }
                            
                            topic.Name = viewModel.Name;
                            topic.Category_Id = viewModel.Category;
                            topic.Image = viewModel.Image;
                            topic.IsLocked = viewModel.IsLocked;
                            topic.IsSticky = viewModel.IsSticky;
                            topic.MembershipUser_Id = LoggedOnReadOnlyUser.Id;
                            topic.Post_Id = post.Id;

                            post.PostContent = viewModel.Content;
                            post.MembershipUser_Id = LoggedOnReadOnlyUser.Id;
                            post.Topic_Id = topic.Id;
                            post.IsTopicStarter = true;

                            topic.ShotContent = string.Concat(StringUtils.ReturnAmountWordsFromString(StringUtils.StripHtmlFromString(post.PostContent), 50), "....");
                            topic.isAutoShotContent = true;
							var i = topic.ShotContent.Length;
							if (i > 450)
							{
								topic.ShotContent = topic.ShotContent.Substring(0, 440) + "...";
							}

							try
                            {
                                _topicServic.Update(topic);
                                if(pn) _postSevice.Add(post);
                                else _postSevice.Update(post);


                                unitOfWork.Commit();

                                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                                {
                                    Message = LocalizationService.GetResourceString("Success.TopicCreateSuccess"),
                                    MessageType = GenericMessages.success
                                };
                                //return RedirectToAction("Edit", new { Id = topic.Id });
                            }
                            catch (Exception ex)
                            {
                                LoggingService.Error(ex.Message);
                                unitOfWork.Rollback();

                                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                                {
                                    Message = LocalizationService.GetResourceString("Error.TopicCreateError"),
                                    MessageType = GenericMessages.warning
                                };
                            }
                        }
                        else
                        {
                            //viewModel.Category = null;
                            //No permission to create a Poll so show a message but create the topic
                            //TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                            //{
                            //    Message = LocalizationService.GetResourceString("Errors.NoPermissionCatergory"),
                            //    MessageType = GenericMessages.info
                            //};
                            ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.CatergoryMessage"));
                        }

                    }
                    viewModel.Categories = _categoryService.GetBaseSelectListCategories(cats);
                    return View(viewModel);
                }
                return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
            }
        }
        #endregion


        #region delete
        public ActionResult Del(Guid id)
        {
            var model = _topicServic.Get(id);
            if (model == null)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Bài viết không tồn tại",
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
            var model = _topicServic.Get(id);
            if (model == null)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Bài viết không tồn tại",
                    MessageType = GenericMessages.warning
                };

                return RedirectToAction("index");
            }

            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    _postSevice.Del(model);
                    _topicServic.Del(model);


                    unitOfWork.Commit();

                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "Xóa bài viết thành công",
                        MessageType = GenericMessages.success
                    };
                    return RedirectToAction("index");
                }
                catch(Exception ex)
                {
                    LoggingService.Error(ex.Message);
                    unitOfWork.Rollback();

                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "Có lỗi xảy ra khi xóa bài viết",
                        MessageType = GenericMessages.warning
                    };
                }
            }

            
            return View(model);
        }
        #endregion


        #region Function 
        private bool CheckCats(Guid? Id, List<Category> cats)
        {
            bool ret = false;
            foreach (var it in cats)
            {
                if (it.Id == Id)
                {
                    ret = true;
                    break;
                }
            }

            return ret;
        }
        #endregion
    }
}