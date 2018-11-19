using System;

using Unity;
using Unity.Lifetime;
using WebMvc.Application;
using WebMvc.Application.Context;
using WebMvc.Application.Interfaces;
using WebMvc.Services;

namespace WebMvc
{
    public static class UnityExtensions
    {
        public static void BindInRequestScope<T1, T2>(this IUnityContainer container) where T2 : T1
        {
            container.RegisterType<T1, T2>(new HierarchicalLifetimeManager());
        }

        public static void BindInRequestScope<T1>(this IUnityContainer container)
        {
            container.RegisterType<T1>(new HierarchicalLifetimeManager());
        }

    }

    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            container.BindInRequestScope<WebMvcContext>();
            container.BindInRequestScope<IUnitOfWorkManager, UnitOfWorkManager>();
            
            //Bind the various domain model services and repositories that e.g. our controllers require         
            container.BindInRequestScope<LoggingService>();
            container.BindInRequestScope<CacheService>();
            container.BindInRequestScope<MembershipService>();
            container.BindInRequestScope<SettingsService>();
            container.BindInRequestScope<ConfigService>();
            container.BindInRequestScope<LocalizationService>();
            container.BindInRequestScope<CategoryService>();
            container.BindInRequestScope<TopicService>();
            container.BindInRequestScope<PostSevice>();
            container.BindInRequestScope<ProductPostSevice>();
            container.BindInRequestScope<RoleSevice>();
            container.BindInRequestScope<PermissionService>();
            container.BindInRequestScope<ContactService>();
            container.BindInRequestScope<BookingSevice>();
            container.BindInRequestScope<TypeRoomSevice>();
            container.BindInRequestScope<ProductSevice>();
            container.BindInRequestScope<EmployeesService>();
            container.BindInRequestScope<EmployeesRoleService>();
            container.BindInRequestScope<ShoppingCartService>();
            container.BindInRequestScope<ShoppingCartProductService>();
            container.BindInRequestScope<MenuService>();
            container.BindInRequestScope<CarouselService>();
            container.BindInRequestScope<Login>();
            
        }
    }


}