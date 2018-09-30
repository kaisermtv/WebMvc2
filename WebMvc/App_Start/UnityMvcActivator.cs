using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unity;
using Unity.AspNet.Mvc;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(WebMvc.UnityMvcActivator), nameof(WebMvc.UnityMvcActivator.Start))]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(WebMvc.UnityMvcActivator), nameof(WebMvc.UnityMvcActivator.Shutdown))]

namespace WebMvc
{
    /// <summary>
    /// Provides the bootstrapping for integrating Unity with ASP.NET MVC.
    /// </summary>
    public static class UnityMvcActivator
    {
        /// <summary>
        /// Integrates Unity when the application starts.
        /// </summary>
        public static void Start() 
        {
            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(UnityConfig.Container));

            DependencyResolver.SetResolver(new UnityDependencyResolver(UnityConfig.Container));

            // TODO: Uncomment if you want to use PerRequestLifetimeManager
            Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(UnityPerRequestHttpModule));
        }

        /// <summary>
        /// Disposes the Unity container when the application is shut down.
        /// </summary>
        public static void Shutdown()
        {
            UnityConfig.Container.Dispose();
        }


        public class UnityDependencyResolver : IDependencyResolver
        {
            private const string HttpContextKey = "perRequestContainer";

            private readonly IUnityContainer _container;

            public UnityDependencyResolver(IUnityContainer container)
            {
                _container = container;
            }

            public object GetService(Type serviceType)
            {
                if (typeof(IController).IsAssignableFrom(serviceType))
                {
                    return ChildContainer.Resolve(serviceType);
                }

                return IsRegistered(serviceType) ? ChildContainer.Resolve(serviceType) : null;
            }

            public IEnumerable<object> GetServices(Type serviceType)
            {
                if (IsRegistered(serviceType))
                {
                    yield return ChildContainer.Resolve(serviceType);
                }

                foreach (var service in ChildContainer.ResolveAll(serviceType))
                {
                    yield return service;
                }
            }

            protected IUnityContainer ChildContainer
            {
                get
                {
                    var childContainer = HttpContext.Current.Items[HttpContextKey] as IUnityContainer;

                    if (childContainer == null)
                    {
                        HttpContext.Current.Items[HttpContextKey] = childContainer = _container.CreateChildContainer();
                    }

                    return childContainer;
                }
            }

            public static void DisposeOfChildContainer()
            {
                var childContainer = HttpContext.Current.Items[HttpContextKey] as IUnityContainer;

                if (childContainer != null)
                {
                    childContainer.Dispose();
                }
            }

            private bool IsRegistered(Type typeToCheck)
            {
                var isRegistered = true;

                if (typeToCheck.IsInterface || typeToCheck.IsAbstract)
                {
                    isRegistered = ChildContainer.IsRegistered(typeToCheck);

                    if (!isRegistered && typeToCheck.IsGenericType)
                    {
                        var openGenericType = typeToCheck.GetGenericTypeDefinition();

                        isRegistered = ChildContainer.IsRegistered(openGenericType);
                    }
                }

                return isRegistered;
            }
        }
    }
}