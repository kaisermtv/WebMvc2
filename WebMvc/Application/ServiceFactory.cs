using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMvc.Application
{
    public class ServiceFactory
    {
        /// <summary>
        /// ServiceFactory is to replace the API, and an efficient way to get access to any interfaced service
        /// </summary>
        public static THelper Get<THelper>()
        {
            if (HttpContext.Current != null)
            {
                var key = string.Concat("ServiceFactory-", typeof(THelper).Name);
                if (!HttpContext.Current.Items.Contains(key))
                {
                    var resolvedService = DependencyResolver.Current.GetService<THelper>();
                    if (!HttpContext.Current.Items.Contains(key))
                        HttpContext.Current.Items.Add(key, resolvedService);
                }
                return (THelper)HttpContext.Current.Items[key];
            }
            return DependencyResolver.Current.GetService<THelper>();
        }
    }
}