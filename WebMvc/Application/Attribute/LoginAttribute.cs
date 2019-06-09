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
        protected RedirectToRouteResult LoginPage => new RedirectToRouteResult(new RouteValueDictionary(new { action = "Login", controller = "Members", area = "", ReturnUrl = HttpContext.Current.Request.RawUrl }));
        protected RedirectToRouteResult HomePage => new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Home", area = "" }));
        protected RedirectToRouteResult AdminLoginPage => new RedirectToRouteResult(new RouteValueDictionary(new { action = "Login", controller = "AdminMembers", area = "Admin", ReturnUrl = HttpContext.Current.Request.RawUrl }));
        protected RedirectToRouteResult AdminHomePage => new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Admin", area = "Admin" }));
        protected RedirectToRouteResult NotPermisionPage => new RedirectToRouteResult(new RouteValueDictionary(new { action = "Login", controller = "Members", area = "", ReturnUrl = HttpContext.Current.Request.RawUrl }));
        protected RedirectToRouteResult NotRolePage => new RedirectToRouteResult(new RouteValueDictionary(new { action = "Login", controller = "Members", area = "", ReturnUrl = HttpContext.Current.Request.RawUrl }));


        protected Login Login => ServiceFactory.Get<Login>();

        protected LoginOption Opt;
        protected string[] StrOpt;

        public string[] Users;
        public string[] Roles;
        public string[] Permisions;

        public LoginAttribute()
        {
            Opt = LoginOption.Login;
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
                case LoginOption.Permission:

                    break;
            }
        }

    }

    public class AdminLoginAttribute : LoginAttribute
    {
        public AdminLoginAttribute()
        {
            Opt = LoginOption.AdminLogin;
        }
    }

    public class AdminNotLoginAttribute : LoginAttribute
    {
        public AdminNotLoginAttribute()
        {
            Opt = LoginOption.NotAdminLogin;
        }
    }

    public class AllowLoginAttribute : LoginAttribute
    {
        public AllowLoginAttribute()
        {
            Opt = LoginOption.Allow;
        }
    }

    public class NotLoginAttribute : LoginAttribute
    {
        public NotLoginAttribute()
        {
            Opt = LoginOption.NotLogin;
        }
    }

    public class RoleAttribute : LoginAttribute
    {
        public RoleAttribute(params string[] roles)
        {
            Opt = LoginOption.Role;
            Roles = roles;
        }
    }

    public class PermissionAttribute : LoginAttribute
    {
        public PermissionAttribute(params string[] permissions)
        {
            Opt = LoginOption.Permission;
            Permisions = permissions;
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
        Permission,
        AdminLogin,
        SuperLogin
    }
}