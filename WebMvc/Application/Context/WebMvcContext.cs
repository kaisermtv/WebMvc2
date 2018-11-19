using DataLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebMvc.Services;

namespace WebMvc.Application.Context
{
    public partial class WebMvcContext
    {
        private SQLTrans transaction;
        private List<string> cacheStartsWithToClear;
        private readonly CacheService _cacheService;

        public WebMvcContext(CacheService cacheService)
        {
            _cacheService = cacheService;
            //constr = ConfigurationManager.ConnectionStrings["WebMvcContext"].ToString();
        }

        public SQLTrans BeginTransaction()
        {
            if (transaction == null)
            {
                transaction = DataManage.GetSQLTrans();
                cacheStartsWithToClear = new List<string>();
            }
            return transaction;
        }

        public SQLCon CreateCommand()
        {
            if (transaction != null) return transaction.GetSQLCon();
            return DataManage.GetSQLCon();
        }

        public void AddCacheStartsWithToClear(string _cachekey)
        {
            if (transaction != null)
            {
                cacheStartsWithToClear.Add(_cachekey);
            } else
            {
                _cacheService.ClearStartsWith(_cachekey);
            }
        }

        public void Dispose()
        {
            if (transaction != null)
            {
                transaction.Dispose();
                transaction = null;
                _cacheService.ClearStartsWith(cacheStartsWithToClear);
            }

        }
    }

    public static class Extension
    {
        public static void cacheStartsWithToClear(this SQLCon con, string _cachekey)
        {
            ServiceFactory.Get<WebMvcContext>().AddCacheStartsWithToClear(_cachekey);
        }
    }
}