using DataLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebMvc.Application;
using WebMvc.Application.Lib;
using WebMvc.Application.ViewEngine;
using WebMvc.Services;

namespace WebMvc
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public LoggingService LoggingService => ServiceFactory.Get<LoggingService>();

        protected void Application_Start()
        {
            LoggingService.Initialise(ConfigUtils.GetAppSettingInt32("LogFileMaxSizeBytes", 10000));
            LoggingService.Error("START APP");

            DataManage.SettingConfig(new DbConfig());


            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            // Set the view engine
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new WebMvcViewEngine());

            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;

            
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
			//RouteConfig.CheckRouteTheme();


			HttpContext.Current.Response.AddHeader("X-Frame-Options", "SAMEORIGIN");
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpContext.Current.User = ServiceFactory.Get<LoginPrincipal>() ;
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            //var entityContext = HttpContext.Current.Items[SiteConstants.Instance.MvcForumContext] as IDatabase;
            //entityContext?.Dispose();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var lastError = Server.GetLastError();
            // Don't flag missing pages or changed urls, as just clogs up the log
            if (!lastError.Message.Contains("was not found or does not implement IController"))
            {
                LoggingService.Error(lastError);
            }
        }
    }
}
