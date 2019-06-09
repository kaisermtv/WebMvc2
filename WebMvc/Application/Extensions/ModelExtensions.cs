using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMvc.Application.Entities;
using WebMvc.Application.Lib;
using WebMvc.Services;
using WebMvc.ViewModels;

namespace WebMvc.Application.Extensions
{
    public static class ModelExtensions
    {

        #region Product
        public static ImageMng Images(this Product product)
        {
            var helper = ServiceFactory.Get<RequestHelpers>();

            string keycache = string.Concat("ModelExtensions.Product-", product.Id, ".Images");
            return helper.Cache(keycache, () => {
                return new ImageMng(product.Image);
            });
        }

        public static string Image(this Product product)
        {
            return product.Images().First();
        }

        public static string ImageCrop(this Product product, int size)
        {
            return product.Images().FirstCrop(size);
        }

        public static string ImageCrop(this Product product, int xsize, int ysize)
        {
            return product.Images().FirstCrop(xsize, ysize);
        }

        #region Product Attribute
        public static ProductAttributeValue Attribute(this Product product,string key)
        {
            var helper = ServiceFactory.Get<RequestHelpers>();

            string keycache = string.Concat("ModelExtensions.Product-", product.Id, ".Attribute-", key);
            return helper.Cache(keycache, () => {
                var productService = ServiceFactory.Get<ProductSevice>();
                return productService.GetAttributeValue(product, key);
            });
            
        }

        public static List<ProductAttributeValue> Attributes(this Product product)
        {
            var helper = ServiceFactory.Get<RequestHelpers>();

            string key = string.Concat("ModelExtensions.Product-", product.Id, ".Attributes");
            return helper.Cache(key, () => {
                var productService = ServiceFactory.Get<ProductSevice>();
                return productService.GetAllAttributeValue(product);
            });
        }
        #endregion

        #region GetLinkDetail
        public static string GetLink(this Product product)
        {
            var helper = ServiceFactory.Get<RequestHelpers>();

            string key = string.Concat("ModelExtensions.Product-", product.Id, ".GetLink");
            return helper.Cache(key, () => {
                return helper.Url.Action("ShowBySlug", "Product", new { area = "", catslug = product.GetCatergory()?.Slug, slug = product.Slug });
            });

            //return helper.Url.Action("ShowBySlug","Product",new { area = "", catslug = product.GetCatergory()?.Slug, slug = product.Slug});
        }
        #endregion

        #region JoinTable
        public static Category GetCatergory(this Product product)
        {
            if (product.Category_Id == null) return null;

             var helper = ServiceFactory.Get<RequestHelpers>();

            string keycache = string.Concat("ModelExtensions.Product-", product.Id, ".GetCatergory");
            return helper.Cache(keycache, () => {
                var categoryService = ServiceFactory.Get<CategoryService>();
                return categoryService.Get((Guid)product.Category_Id);
            });
        }

        #endregion

        #endregion

        #region Catergory

        #region GetLinkDetail
        public static string GetLink(this Category cat)
        {
            var helper = ServiceFactory.Get<RequestHelpers>();

            string key = string.Concat("ModelExtensions.Category-", cat.Id, ".GetLink");
            return helper.Cache(key, () => {
                if (cat.IsProduct)
                {
                    return helper.Url.Action("ShowBySlugProduct", "Catergory", new { area = "", slug = cat.Slug });
                    
                }

                return helper.Url.Action("ShowBySlug", "Catergory", new { area = "", slug = cat.Slug });
            });
        }
        #endregion
        #endregion

        #region Topic
        public static ImageMng Images(this Topic topic)
        {
            var helper = ServiceFactory.Get<RequestHelpers>();

            string keycache = string.Concat("ModelExtensions.Topic-", topic.Id, ".Images");
            return helper.Cache(keycache, () => {
                return new ImageMng(topic.Image);
            });
        }

        public static string Image(this Topic topic)
        {
            return topic.Images().First();
        }

        public static string ImageCrop(this Topic topic, int size)
        {
            return topic.Images().FirstCrop(size);
        }

        public static string ImageCrop(this Topic topic, int xsize, int ysize)
        {
            return topic.Images().FirstCrop(xsize, ysize);
        }

        #region GetLinkDetail
        public static string GetLink(this Topic topic)
        {
            var helper = ServiceFactory.Get<RequestHelpers>();

            string key = string.Concat("ModelExtensions.Topic-", topic.Id, ".GetLink");
            return helper.Cache(key, () => {
                return helper.Url.Action("ShowBySlug", "Topic", new { area = "", catslug = topic.GetCatergory()?.Slug, slug = topic.Slug });
            });
        }
        #endregion

        #region JoinTable
        public static Category GetCatergory(this Topic topic)
        {
            if (topic.Category_Id == null) return null;

            var helper = ServiceFactory.Get<RequestHelpers>();

            string keycache = string.Concat("ModelExtensions.Topic-", topic.Id, ".GetCatergory");
            return helper.Cache(keycache, () => {
                var categoryService = ServiceFactory.Get<CategoryService>();
                return categoryService.Get((Guid)topic.Category_Id);
            });
        }

        public static MembershipUser GetUser(this Topic topic)
        {
            if (topic.MembershipUser_Id == null) return null;

            var helper = ServiceFactory.Get<RequestHelpers>();

            string keycache = string.Concat("ModelExtensions.Topic-", topic.Id, ".GetUser");
            return helper.Cache(keycache, () => {
                var _membershipService = ServiceFactory.Get<MembershipService>();
                return _membershipService.Get((Guid)topic.MembershipUser_Id);
            });
        }

        #endregion


        #endregion
    }
}