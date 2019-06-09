using DataLib;
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
        private readonly SQLTrans transaction;

        public UnitOfWork(WebMvcContext _context)
        {
            context = _context as WebMvcContext;

            transaction = context.BeginTransaction();
        }

        public void Commit()
        {
            transaction.Commit();
            context.ClearCache();
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