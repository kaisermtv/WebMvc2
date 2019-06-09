using System.Web.Mvc;

namespace WebMvc.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin_login",
                "Admin/login",
                new { controller = "AdminMembers", action = "Login" }
            );

            context.MapRoute(
                "Admin_logout",
                "Admin/logout",
                new { controller = "AdminMembers", action = "Logout" }
            );

            context.MapRoute(
                "Admin_Menu",
                "Admin/Menu/{action}/{id}",
                new { controller = "AdminMenu", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_Category",
                "Admin/Category/{action}/{id}",
                new { controller = "AdminCategory", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_Carousel",
                "Admin/Carousel/{action}/{id}",
                new { controller = "AdminCarousel", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_Booking",
                "Admin/Booking/{action}/{id}",
                new { controller = "AdminBooking", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_Contact",
                "Admin/Contact/{action}/{id}",
                new { controller = "AdminContact", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_Employees",
                "Admin/Employees/{action}/{id}",
                new { controller = "AdminEmployees", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_Language",
                "Admin/Language/{action}/{id}",
                new { controller = "AdminLanguage", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_Members",
                "Admin/Members/{action}/{id}",
                new { controller = "AdminMembers", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_Product",
                "Admin/Product/{action}/{id}",
                new { controller = "AdminProduct", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_Topic",
                "Admin/Topic/{action}/{id}",
                new { controller = "AdminTopic", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}