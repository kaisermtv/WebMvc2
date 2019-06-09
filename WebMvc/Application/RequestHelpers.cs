using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMvc.Application
{
    public class RequestHelpers
    {
        #region Request Utils
        private UrlHelper _Url;
        public UrlHelper Url
        {
            get
            {
                if (_Url == null) _Url = new UrlHelper(Request.RequestContext);
                return _Url;
            }
        }

        private HttpRequest _Request;
        public HttpRequest Request
        {
            get
            {
                if (_Request == null) _Request = HttpContext.Current.Request;
                return _Request;
            }
        }
        #endregion

        #region Cache
        private Dictionary<string, object> _Cache = new Dictionary<string, object>();

        public T Cache<T>(string cacheKey, Func<T> getCacheItem)
        {
            if (!_Cache.ContainsKey(cacheKey))
            {
                var result = getCacheItem();
                if (result != null)
                {
                    SetCache(cacheKey, result);
                    return result;
                }
                return default(T);
            }
            return (T)_Cache[cacheKey];
        }

        public void SetCache(string cacheKey, object objectToCache)
        {
            _Cache.Add(cacheKey, objectToCache);
        }
        #endregion
    }
}