using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebMvc.Application.Interfaces;
using WebMvc.Services;

namespace WebMvc.Application.Context
{
    class UnitOfWork : IUnitOfWork
    {
        private readonly WebMvcContext context;
        private readonly SqlTransaction transaction;

        public UnitOfWork(WebMvcContext _context)
        {
            context = _context as WebMvcContext;

            transaction = context.BeginTransaction();
        }

        public void Commit()
        {
            transaction.Commit();
        }

        public void Commit(List<string> cacheStartsWithToClear, CacheService cacheService)
        {
            Commit();
            cacheService.ClearStartsWith(cacheStartsWithToClear);
        }
        

        public void Dispose()
        {
            context.Dispose();
        }

        public void Rollback()
        {
            transaction.Rollback();
        }
    }
}