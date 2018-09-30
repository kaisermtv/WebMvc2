namespace WebMvc.Controllers
{
    using System.Web.Mvc;
    using WebMvc.Application.Context;
    using WebMvc.Services;
    using WebMvc.ViewModels;
    using System;
    using WebMvc.Application;
    using WebMvc.Areas.Admin.ViewModels;
    using WebMvc.Application.Interfaces;
	using System.Collections.Generic;
	using WebMvc.Application.Lib;
	using Newtonsoft.Json;

	public class CategoryController : BaseController
    {
        private readonly CategoryService _categoryService;
        private readonly TopicService _topicServic;
        private readonly ProductSevice _productSevice;

        public CategoryController(ProductSevice productSevice, TopicService topicService, LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, CacheService cacheService, LocalizationService localizationService, CategoryService categoryService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {
            _categoryService = categoryService;
            _topicServic = topicService;
            _productSevice = productSevice;
        }
        
        // GET: Category
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult ShowBySlug(string Slug,int? p)
        {
            var cat = _categoryService.GetBySlug(Slug);
            if(cat == null)
            {
                return Redirect("/"+ SiteConstants.Instance.CategoryUrlIdentifier);
            }

            int limit = 10;
			try
			{
				limit = int.Parse(ThemesSetting.getValue("TopicPageLimit").ToString());
			}
			catch { }
			var count = _topicServic.GetCount(cat.Id);

            var Paging = CalcPaging(limit,p, count);
            
            var model = new CategoryTopicListViewModel
            {
                Cat = cat,
                Paging = Paging,
                ListTopic = _topicServic.GetList(cat.Id,limit, Paging.Page)
            };
            
            return View(model);
        }

        public ActionResult ShowBySlugProduct(string Slug, int? p,List<Guid> group)
		{
			var cat = _categoryService.GetBySlug(Slug);
            if (cat == null)
            {
                return Redirect("/" + SiteConstants.Instance.ProductUrlIdentifier);
            }

			var cats = _categoryService.GetTreeCategories(cat);

			var ProductFindter = _productSevice.GetFindterProduct();
			if(group != null)
			{
				ProductFindter.AddGroupId(group);
			} else
			{
				group = new List<Guid>();
			}

			if(cats != null)
			{
				ProductFindter.AddCat(cats);
			}
			
			
			var groupclass = _productSevice.GetClassByProducts(cats);

			var attr = _productSevice.GetAttributeByClass(groupclass, true);

			List<CategoryProductFindterViewModel> Attributes = new List<CategoryProductFindterViewModel>();

			foreach (var it in attr)
			{
				var cl = new CategoryProductFindterViewModel
				{
					Name = it.LangName,
					Title = string.Concat("Product.Attribute.", it.LangName),
					Options = new List<CategoryProductFindterOption>()
				};

				if(it.ValueType == 2)
				{
					if (it.ValueOption.IsNullEmpty()) { continue; }
					dynamic json = JsonConvert.DeserializeObject(it.ValueOption);
					if (json == null) { continue; }

					foreach (var itt in json)
					{
						var pit = new CategoryProductFindterOption
						{
							Value = itt,
							Text = itt,
						};

						if(Request[string.Concat(cl.Name, ".", pit.Value)] == "1")
						{
							pit.Check = true;

							ProductFindter.SetAttributeEquas(it.Id,(string)itt);
						}

						cl.Options.Add(pit);
					}
				}
				
				Attributes.Add(cl);
			}


			int limit = 10;
			try
			{
				limit = int.Parse(ThemesSetting.getValue("ProductPageLimit").ToString());
			}
			catch { }
            var count = ProductFindter.GetCount();

            var Paging = CalcPaging(limit, p, count);

			var model = new CategoryProductListViewModel
			{
				Cat = cat,
				Paging = Paging,
				ListProduct = ProductFindter.GetList(limit, Paging.Page),
				GroupProducts = groupclass,
				GroupSelect = group,
				Attributes = Attributes
			};
			


			return View(model);
        }

        public ActionResult Show(Guid Id)
        {
            

            return View();
        }

        public PartialViewResult ListCategorySideMenu()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var viewModel = new ListCategoriesViewModel
                {
                    Categories = _categoryService.GetAll()
                };
                return PartialView(viewModel);
            }
        }
    }
}