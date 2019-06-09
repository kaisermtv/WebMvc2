using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using WebMvc.Application.Lib;

namespace WebMvc.Application.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class ModulAttribute : System.Attribute
    {
        private HttpNotFoundResult HttpNotFound = new HttpNotFoundResult();

        public ModulAttribute()
        {
            
        }

        private string ModulName;
        public ModulAttribute(string name)
        {
            ModulName = name;
        }


        public void OnAuthorModul(AuthenticationContext filterContext)
        {
            if (ModulName.IsNullEmpty()) return;

            if (!ModulConfig.Moduls.Contains(ModulName))
            {
                filterContext.Result = HttpNotFound;
            }
        }
    }
}