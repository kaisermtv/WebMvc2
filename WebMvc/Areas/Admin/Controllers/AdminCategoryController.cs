using Newtonsoft.Json.Linq;
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

namespace WebMvc.Areas.Admin.Controllers
{
    //[Authorize(Roles = AppConstants.AdminRoleName)]
    public class AdminCategoryController : BaseAdminController
    {
        private readonly CategoryService _categoryService;

        public AdminCategoryController(CategoryService categoryService, LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {
            _categoryService = categoryService;
        }

        #region News Cat
        public ActionResult News()
        {
            return TreeCategory(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult News(string JsonText)
        {
            return TreeCategory(false, JsonText);
        }

        public ActionResult ListCatNews()
        {
            return List(false);
        }

        public ActionResult NewCatNews()
        {
            return Create(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewCatNews(CategoryViewModel categoryViewModel)
        {
            return Create(false, categoryViewModel);
        }

        public ActionResult EditCatNews(Guid id)
        {
            return Edit(false, id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCatNews(CategoryViewModel categoryViewModel)
        {
            return Edit(false, categoryViewModel);
        }

        public ActionResult DelCatNews(Guid id)
        {
            return Delete(false, id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DelCatNews1(Guid id)
        {
            return Delete1(false, id);
        }
        #endregion

        #region Product cat
        public ActionResult Product()
        {
            return TreeCategory(true);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Product(string JsonText)
        {
            return TreeCategory(false, JsonText);
        }

        public ActionResult ListCatProduct()
        {
            return List(true);
        }

        public ActionResult NewCatProduct()
        {
            return Create(true);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewCatProduct(CategoryViewModel categoryViewModel)
        {
            return Create(true, categoryViewModel);
        }

        public ActionResult EditCatProduct(Guid id)
        {
            return Edit(true, id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCatProduct(CategoryViewModel categoryViewModel)
        {
            return Edit(true, categoryViewModel);
        }

        public ActionResult DelCatProduct(Guid id)
        {
            return Delete(false, id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DelCatProduct1(Guid id)
        {
            return Delete1(false, id);
        }
        #endregion


        private ActionResult List(bool isProduct)
        {
            var viewModel = new ListCategoriesViewModel
            {
                isProduct = isProduct,
                Categories = _categoryService.GetList(isProduct)
            };
            return View("List", viewModel);
        }

        private ActionResult TreeCategory(bool isProduct)
        {
            var viewModel = new ListCategoriesViewModel
            {
                isProduct = isProduct,
                Categories = _categoryService.GetList(isProduct)
            };
            return View("TreeCategory", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        private ActionResult TreeCategory(bool isProduct, string JsonText)
        {
            try
            {
                JArray json = JArray.Parse(JsonText);

                var templst = new Dictionary<Guid, Guid?>();

                AddChild(templst, json, null);

                var menulst = _categoryService.GetList(isProduct);
                foreach (var it in menulst)
                {
                    if (!templst.ContainsKey(it.Id))
                    {
                        templst.Add(it.Id, null);
                    }
                }

                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        int sort = 0;
                        foreach (var it in templst)
                        {
                            _categoryService.UpdateSortAndParent(it.Key, it.Value, sort++);
                        }


                        unitOfWork.Commit();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Cập nhật thành công",
                            MessageType = GenericMessages.success
                        };
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Có lỗi xảy ra! Xin thử lại.",
                            MessageType = GenericMessages.warning
                        };
                    }
                }

            }
            catch (Exception ex)
            {
                LoggingService.Error(ex);

                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Có lỗi xảy ra! Xin thử lại.",
                    MessageType = GenericMessages.warning
                };
            }

            var viewModel = new ListCategoriesViewModel
            {
                isProduct = isProduct,
                Categories = _categoryService.GetList(isProduct)
            };
            return View("TreeCategory", viewModel);
        }

        private void AddChild(Dictionary<Guid, Guid?> templst, JArray array, Guid? paren)
        {
            foreach (var it in array)
            {
                string id = (string)it["id"];
                Guid guid = new Guid(id);

                if (!templst.ContainsKey(guid))
                {
                    templst.Add(guid, paren);
                }

                if (it["children"] != null)
                {
                    AddChild(templst, (JArray)it["children"], guid);
                }

            }
        }

        #region Create
        private ActionResult Create(bool isProduct)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var categoryViewModel = new CategoryViewModel
                {
                    IsProduct = isProduct,
                    AllCategories = _categoryService.GetBaseSelectListCategories(_categoryService.GetList(isProduct))
                };
                return View("Create", categoryViewModel);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        private ActionResult Create(bool isProduct, CategoryViewModel categoryViewModel)
        {
            categoryViewModel.IsProduct = isProduct;

            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var category = new Category
                        {
                            Name = categoryViewModel.Name,
                            Description = categoryViewModel.Description,
                            IsLocked = categoryViewModel.IsLocked,
                            ModeratePosts = categoryViewModel.ModeratePosts,
                            ModerateTopics = categoryViewModel.ModerateTopics,
                            SortOrder = categoryViewModel.SortOrder,
                            PageTitle = categoryViewModel.PageTitle,
                            MetaDescription = categoryViewModel.MetaDesc,
                            Colour = categoryViewModel.CategoryColour,
                            Category_Id = categoryViewModel.ParentCategory,
                            IsProduct = categoryViewModel.IsProduct,
                            Image = categoryViewModel.Image,
                        };

                        //if (categoryViewModel.ParentCategory != null)
                        //{
                        //    var parentCategory = _categoryService.Get(categoryViewModel.ParentCategory.Value);
                        //    category.ParentCategory = parentCategory;
                        //    SortPath(category, parentCategory);
                        //}

                        _categoryService.Add(category);

                        unitOfWork.Commit();
                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Category Created",
                            MessageType = GenericMessages.success
                        };

                        if (isProduct)
                        {
                            return RedirectToAction("Product");
                        }
                        else
                        {
                            return RedirectToAction("News");
                        }

                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError("", "There was an error creating the category");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "There was an error creating the category");
            }

            categoryViewModel.AllCategories = _categoryService.GetBaseSelectListCategories(_categoryService.GetList(isProduct));
            return View("Create", categoryViewModel);
        }

        #endregion

        #region Edit Category
        private CategoryViewModel CreateEditCategoryViewModel(Category category)
        {
            var categoryViewModel = new CategoryViewModel
            {
                Name = category.Name,
                Description = category.Description,
                IsLocked = category.IsLocked,
                ModeratePosts = category.ModeratePosts == true,
                ModerateTopics = category.ModerateTopics == true,
                SortOrder = category.SortOrder,
                Id = category.Id,
                PageTitle = category.PageTitle,
                MetaDesc = category.MetaDescription,
                Image = category.Image,
                CategoryColour = category.Colour,
                ParentCategory = category.Category_Id,
                IsProduct = category.IsProduct,
                AllCategories = _categoryService.GetBaseSelectListCategories(_categoryService.GetCategoriesParenCatregori(category))
            };

            return categoryViewModel;
        }

        private ActionResult Edit(bool isProduct, Guid id)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var category = _categoryService.Get(id);
                if (category == null || category.IsProduct != isProduct)
                {
                    string message;
                    string rAction;
                    if (isProduct)
                    {
                        message = "Nhóm sản phẩm không tồn tại hoặc đã bị xóa!";
                        rAction = "Product";
                    }
                    else
                    {
                        message = "Nhóm bài viết không tồn tại hoặc đã bị xóa!";
                        rAction = "News";
                    }

                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = message,
                        MessageType = GenericMessages.danger
                    };

                    return RedirectToAction(rAction);
                }


                var categoryViewModel = CreateEditCategoryViewModel(category);

                return View("Edit", categoryViewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        private ActionResult Edit(bool isProduct, CategoryViewModel categoryViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var category = _categoryService.Get(categoryViewModel.Id);
                        if (category == null || category.IsProduct != isProduct)
                        {
                            string message;
                            string rAction;
                            if (isProduct)
                            {
                                message = "Nhóm sản phẩm không tồn tại hoặc đã bị xóa!";
                                rAction = "Product";
                            }
                            else
                            {
                                message = "Nhóm bài viết không tồn tại hoặc đã bị xóa!";
                                rAction = "News";
                            }

                            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                            {
                                Message = message,
                                MessageType = GenericMessages.danger
                            };

                            return RedirectToAction(rAction);
                        }

                        // Check they are not trying to add a subcategory of this category as the parent or it will break
                        var cats = _categoryService.GetCategoriesParenCatregori(category);
                        var lst = cats.Where(x => x.Id == categoryViewModel.ParentCategory).ToList();
                        if (lst.Count == 0) categoryViewModel.ParentCategory = null;
                        //categoryViewModel.AllCategories = _categoryService.GetBaseSelectListCategories(cats);


                        category.Description = categoryViewModel.Description;
                        category.IsLocked = categoryViewModel.IsLocked;
                        category.ModeratePosts = categoryViewModel.ModeratePosts;
                        category.ModerateTopics = categoryViewModel.ModerateTopics;
                        category.Name = categoryViewModel.Name;
                        category.Image = categoryViewModel.Image;
                        category.SortOrder = categoryViewModel.SortOrder;
                        category.PageTitle = categoryViewModel.PageTitle;
                        category.MetaDescription = categoryViewModel.MetaDesc;
                        category.Colour = categoryViewModel.CategoryColour;
                        category.Category_Id = categoryViewModel.ParentCategory;
                        category.IsProduct = categoryViewModel.IsProduct;

                        _categoryService.Update(category);

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Category Updated",
                            MessageType = GenericMessages.success
                        };

                        categoryViewModel = CreateEditCategoryViewModel(category);

                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error(ex);
                        unitOfWork.Rollback();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Có lỗi xảy ra khi cập nhật danh mục",
                            MessageType = GenericMessages.danger
                        };
                    }
                }
            }

            return View("Edit", categoryViewModel);
        }
        #endregion

        #region delete
        public ActionResult Delete(bool isProduct, Guid id)
        {
            var model = _categoryService.Get(id);
            if (model == null || model.IsProduct != isProduct)
            {
                string message;
                string rAction;
                if (isProduct)
                {
                    message = "Nhóm sản phẩm không tồn tại hoặc đã bị xóa!";
                    rAction = "Product";
                }
                else
                {
                    message = "Nhóm bài viết không tồn tại hoặc đã bị xóa!";
                    rAction = "News";
                }

                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = message,
                    MessageType = GenericMessages.danger
                };

                return RedirectToAction(rAction);
            }

            var submenu = _categoryService.GetSubCategory(model);
            if (submenu.Count > 0)
            {
                return View("NotDel", model);
            }

            var _topicService = ServiceFactory.Get<TopicService>();
            var subnews = _topicService.GetList(model.Id);
            if (subnews.Count > 0)
            {
                return View("NotDel", model);
            }

            var _productService = ServiceFactory.Get<ProductSevice>();
            var subproduct = _productService.GetList(model.Id);
            if (subnews.Count > 0)
            {
                return View("NotDel", model);
            }


            return View("Delete",model);
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult Delete1(bool isProduct, Guid id)
        {
            var model = _categoryService.Get(id);
            if (model == null || model.IsProduct != isProduct)
            {
                string message;
                string rAction;
                if (isProduct)
                {
                    message = "Nhóm sản phẩm không tồn tại hoặc đã bị xóa!";
                    rAction = "Product";
                }
                else
                {
                    message = "Nhóm bài viết không tồn tại hoặc đã bị xóa!";
                    rAction = "News";
                }

                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = message,
                    MessageType = GenericMessages.danger
                };

                return RedirectToAction(rAction);
            }

            var submenu = _categoryService.GetSubCategory(model);
            if (submenu.Count > 0)
            {
                return View("NotDel", model);
            }


            var _topicService = ServiceFactory.Get<TopicService>();
            var subnews = _topicService.GetList(model.Id);
            if (subnews.Count > 0)
            {
                return View("NotDel", model);
            }

            var _productService = ServiceFactory.Get<ProductSevice>();
            var subproduct = _productService.GetList(model.Id);
            if (subnews.Count > 0)
            {
                return View("NotDel", model);
            }


            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    _categoryService.Del(model);

                    unitOfWork.Commit();

                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "Xóa danh mục thành công",
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
                        Message = "Có lỗi xảy ra khi xóa danh mục",
                        MessageType = GenericMessages.warning
                    };
                }
            }


            return View("Delete",model);
        }
        #endregion

    }
}