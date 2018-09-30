namespace WebMvc.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using WebMvc.Application;
    using WebMvc.Application.Context;
    using WebMvc.Application.Entities;
    using WebMvc.Application.Interfaces;
    using WebMvc.Services;
    using WebMvc.ViewModels;

    public class ProductController : BaseController
    {
        public readonly ProductSevice _productSevice;
        private readonly CategoryService _categoryService;
        private readonly ProductPostSevice _productPostSevice;

        public ProductController(ProductPostSevice productPostSevice,CategoryService categoryService,ProductSevice productSevice,LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, CacheService cacheService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {
            _productSevice = productSevice;
            _categoryService = categoryService;
            _productPostSevice = productPostSevice;
        }
        // GET: Product
        public ActionResult Index(string s, int? p,bool isSale = false,bool isSelling = false)
        {
            int limit = 12;
            var count = _productSevice.GetCount(s);

            var Paging = CalcPaging(limit, p, count);

            var model = new CategoryProductListViewModel
            {
                Paging = Paging,
                ListProduct = _productSevice.GetList(s,limit, Paging.Page)
            };

            return View(model);
        }

		public ActionResult ShowByGroupSlug(string slug, int? p)
		{
			int limit = 12;
			try
			{
				limit = int.Parse(ThemesSetting.getValue("ProductPageLimit").ToString());
			}
			catch { }
			
			var group = _productSevice.GetProductClassBySlug(slug);
			if (group == null) return RedirectToAction("Index");

			var findter = _productSevice.GetFindterProduct();

			findter.AddGroupId(group.Id);

			var count = findter.GetCount();

			var Paging = CalcPaging(limit, p, count);

			var model = new CategoryProductListViewModel
			{
				Group = group,
				Paging = Paging,
				ListProduct = findter.GetList(limit, Paging.Page)
			};

			return View(model);
		}

		public ActionResult ShowBySlug(string catSlug, string Slug)
        {
            var cat = _categoryService.GetBySlug(catSlug);
            if (cat == null && !cat.IsProduct)
            {
                return RedirectToAction("index", "Catergory");
            }

            var topic = _productSevice.GetBySlug(Slug);
            if (topic == null || cat.Id != topic.Category_Id)
            {
                return RedirectToAction("ShowBySlugProduct", "Category", new { slug = cat.Slug });
            }

            ProductPost post = new ProductPost();

            if (topic.ProductPost_Id != null)
            {
                post = _productPostSevice.Get((Guid)topic.ProductPost_Id);
            }

            var model = new ProductViewModel
            {
                Cat = cat,
                product = topic,
                post = post
            };

            return View(model);
        }
		
        [HttpPost]
        public ActionResult AjaxProductForClass(Guid id, int? page,int step = 1)
        {
            var pcls = _productSevice.GetProductClass(id);
            if (pcls == null) return HttpNotFound();

            int limit = 12;
            var count = _productSevice.GetCount(pcls);
            
            var Paging = CalcPaging(limit, page, count);

            var model = new ClassProductViewModel
            {
				Step = step,
				ProductClass = pcls,
                Paging = Paging,
                ListProduct = _productSevice.GetList(pcls, limit, Paging.Page)
            };
            
            return PartialView("AjaxProductForClass",model);
        }
        
        public ActionResult AjaxGetSearch(string search)
        {
            var lst = _productSevice.GetList(search);
            List<ProductAjaxItem> rlst = new List<ProductAjaxItem>();
            if (lst != null)
            {
                var pricmodel = _productSevice.GetAttribute("Price");

                foreach (var it in lst)
                {
                    var val = AppHelpers.ProductValues(it);


                    rlst.Add(new ProductAjaxItem
                    {
                        productName = it.Name,
                        productUrl = AppHelpers.ProductUrls(it.Category_Id, it.Slug),
                        productImage = new ProductAjaxImageItem
                        {
                            fullimg = it.Image,
                            medium = AppHelpers.ImageCropUrl(it.Image,160),
                        },
                        price = _productSevice.GetAttributeValue(it.Id, pricmodel.Id).Value,
                    });
                }
            }
            
            return Json(rlst, JsonRequestBehavior.AllowGet);
        }
    }
}