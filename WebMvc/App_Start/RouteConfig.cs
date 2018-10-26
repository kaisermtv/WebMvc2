using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using WebMvc.Application;
using WebMvc.Services;

namespace WebMvc
{
    public class RouteConfig
    {
		private static RouteCollection routes;

		public static void RegisterRoutes(RouteCollection routes)
		{
			RouteConfig.routes = routes;

			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			CheckRouteTheme();

			//routes.
			//routes.MapRoute(
			//  name: "Contact",
			//  url: "lien-he",
			//  defaults: new { controller = "Contact", action = "index" }
			//);

			//routes.MapRoute(
			//  name: "product",
			//  url: SiteConstants.Instance.ProductUrlIdentifier,
			//  defaults: new { controller = "Product", action = "index" }
			//);

			//routes.MapRoute(
			//  name: "topic",
			//  url: SiteConstants.Instance.TopicUrlIdentifier,
			//  defaults: new { controller = "Topic", action = "index" }
			//);

			//routes.MapRoute(
			//  name: "NewProductSelling",
			//  url: "san-pham-ban-chay",
			//  defaults: new { controller = "Product", action = "index", isSelling = true }
			//);

			//routes.MapRoute(
			//  name: "NewProductSale",
			//  url: "san-pham-xa-hang",
			//  defaults: new { controller = "Product", action = "index", isSale = true }
			//);

			//routes.MapRoute(
			//  name: "NewProduct",
			//  url: "san-pham-moi",
			//  defaults: new { controller = "Product", action = "index" }
			//);

			//routes.MapRoute(
			//   name: "ComputerBuilding",
			//   url: "xay-dung-may-tinh",
			//   defaults: new { controller = "Plugin", action = "ComputerBuilding" }
			//);

			//routes.MapRoute(
			//	name: "login",
			//	url: "dang-nhap",
			//	defaults: new { controller = "Members", action = "Login" }
			//);

			//routes.MapRoute(
			//	name: "logout",
			//	url: "dang-xuat",
			//	defaults: new { controller = "Members", action = "LogOut" }
			//);

			//routes.MapRoute(
			//	name: "register",
			//	url: "dang-ky",
			//	defaults: new { controller = "Members", action = "Register" }
			//);

			//routes.MapRoute(
			//	name: "category",
			//	url: SiteConstants.Instance.CategoryUrlIdentifier,
			//	defaults: new { controller = "Category", action = "Index" }
			//);

			//routes.MapRoute(
			//	"categoryUrls", // Route name
			//	string.Concat(SiteConstants.Instance.CategoryUrlIdentifier, "/{slug}"), // URL with parameters
			//	new { controller = "Category", action = "ShowBySlug", slug = UrlParameter.Optional } // Parameter defaults
			//);

			//routes.MapRoute(
			//	"productUrls", // Route name
			//	string.Concat(SiteConstants.Instance.ProductUrlIdentifier, "/{slug}"), // URL with parameters
			//	new { controller = "Category", action = "ShowBySlugProduct", slug = UrlParameter.Optional } // Parameter defaults
			//);

			//routes.MapRoute(
			//	"GroupProductUrls", // Route name
			//	string.Concat(SiteConstants.Instance.GroupProductUrlIdentifier, "/{slug}"), // URL with parameters
			//	new { controller = "Product", action = "ShowByGroupSlug", slug = UrlParameter.Optional } // Parameter defaults
			//);

			//routes.MapRoute(
			//	"topicShowUrls", // Route name
			//	string.Concat(SiteConstants.Instance.CategoryUrlIdentifier, "/{catslug}/{slug}"), // URL with parameters
			//	new { controller = "Topic", action = "ShowBySlug", catslug = UrlParameter.Optional, slug = UrlParameter.Optional } // Parameter defaults
			//);

			//routes.MapRoute(
			//	"productShowUrls", // Route name
			//	string.Concat(SiteConstants.Instance.ProductUrlIdentifier, "/{catslug}/{slug}"), // URL with parameters
			//	new { controller = "Product", action = "ShowBySlug", catslug = UrlParameter.Optional, slug = UrlParameter.Optional } // Parameter defaults
			//);


			//routes.MapRoute(
			//	name: "Default",
			//	url: "{controller}/{action}/{id}",
			//	defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			//);
			
		}


		private static readonly object _lock = new object();
		private static string OldThemes;
		private static List<string> OldKey = new List<string>();
		public static void CheckRouteTheme()
		{
			var theme = DependencyResolver.Current.GetService<SettingsService>().GetSetting("Theme").ToLower();
			if (theme == OldThemes) return;

			lock (_lock)
			{
				if (theme == OldThemes) return;

				foreach (var it in OldKey)
				{
					RouteConfig.routes.Remove(RouteConfig.routes[it]);
				}
				OldKey.Clear();

				try
				{
					var fp = HostingEnvironment.MapPath(string.Concat("~/Themes/", theme, "/route.json"));
					var str = File.ReadAllText(fp);

					var data = (dynamic)JsonConvert.DeserializeObject(str);

					foreach (var item in data)
					{
						try
						{
							string key = string.Concat(theme, "-", (string)item.Name);

							//var a = new { controller = "Home", action = "Index", id = UrlParameter.Optional };
							//a.i

							RouteConfig.routes.MapRoute(
							  name: key,
							  url: (string)item.Value.Url,
							  defaults: new MapData(item.Value.Defaults)
							);

							OldKey.Add(key);
						}
						catch { }
					}
				}
				catch(Exception ex)
				{
					ServiceFactory.Get<LoggingService>().Error(ex);
				}

				OldThemes = theme;
			}
		}

		public class MapData
		{
			private dynamic data;

			public MapData(dynamic data)
			{
				this.data = data;
			}


			public object controller {
				get
				{

					return data.controller;
				}
				
			}

			public object action
			{
				get
				{
					return data.action;
				}
			}

			public object id
			{
				get
				{
					return data.id;
				}
			}
		}
    }
}
