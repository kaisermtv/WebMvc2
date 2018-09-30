using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace WebMvc.Application
{
    public class JsonConfig
    {
        #region Cache
        private Hashtable cache = new Hashtable();

        public void ClearCahe()
        {
            cache.Clear();
        }
        #endregion
        
        public JsonConfig(string mappath)
        {
            var fp = HostingEnvironment.MapPath(mappath);
            var str = "";
            try
            {
                str = File.ReadAllText(fp);
            }
            catch
            {

            }
            
            foreach (var item in (dynamic)JsonConvert.DeserializeObject(str))
            {
                cache.Add((string)item.Name, item.Value.Value);
            }
        }

        public object GetValue(string key)
        {
            return cache[key];
        }

        public object GetValue(string key,object defaultvalue)
        {
            if (cache[key] == null) return defaultvalue;
            return cache[key];
        }
    }
}