using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace WebMvc.Application.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class LoginAttribute : System.Attribute
    {
        private RedirectToRouteResult LoginPage => new RedirectToRouteResult(new RouteValueDictionary(new { action = "Login", controller = "Members", area = "", ReturnUrl = HttpContext.Current.Request.RawUrl }));
        private RedirectToRouteResult HomePage => new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Home", area = "" }));
        private RedirectToRouteResult AdminLoginPage => new RedirectToRouteResult(new RouteValueDictionary(new { action = "Login", controller = "AdminMembers", area = "Admin", ReturnUrl = HttpContext.Current.Request.RawUrl }));
        private RedirectToRouteResult AdminHomePage => new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Admin", area = "Admin" }));
        private RedirectToRouteResult NotPermisionPage => new RedirectToRouteResult(new RouteValueDictionary(new { action = "Login", controller = "Members", area = "", ReturnUrl = HttpContext.Current.Request.RawUrl }));
        private RedirectToRouteResult NotRolePage => new RedirectToRouteResult(new RouteValueDictionary(new { action = "Login", controller = "Members", area = "", ReturnUrl = HttpContext.Current.Request.RawUrl }));

        
        private Login Login => ServiceFactory.Get<Login>();

        private LoginOption Opt;
        private string[] StrOpt;

        public string[] Users;
        public string[] Roles;
        public string[] Permisions;

        public LoginAttribute()
        {
            Opt = LoginOption.Allow;
        }

        public LoginAttribute(LoginOption opt)
        {
            Opt = opt;
        }

        public LoginAttribute(LoginOption opt,params string[] stropt)
        {
            Opt = opt;
            StrOpt = stropt;
        }

        public void OnAuthorization(AuthenticationContext filterContext)
        {
            switch (Opt)
            {
                case LoginOption.NotLogin:
                    if (Login.Type >= Login.TypeLogin.UserLogin)
                    {
                        filterContext.Result = HomePage;
                    }
                    break;
                case LoginOption.NotAdminLogin:
                    if (Login.Type >= Login.TypeLogin.AdminLogin)
                    {
                        filterContext.Result = AdminHomePage;
                    }
                    break;
                case LoginOption.Allow:
                    break;
                case LoginOption.Login:
                    if (Login.Type == Login.TypeLogin.NotLogin)
                    {
                        filterContext.Result = LoginPage;
                    }
                    break;
                case LoginOption.AdminLogin:
                    if (Login.Type < Login.TypeLogin.AdminLogin)
                    {
                        filterContext.Result = AdminLoginPage;
                    }
                    break;
                case LoginOption.User:
                    
                    break;
                case LoginOption.Role:

                    break;
                case LoginOption.Permision:

                    break;
            }
        }

    }

    public enum LoginOption
    {
        Allow,
        NotLogin,
        NotAdminLogin,
        Login,
        User,
        Role,
        Permision,
        AdminLogin,
        SuperLogin
    }
}