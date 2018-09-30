using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMvc.Application.Interfaces;

namespace WebMvc.Application.Context
{
    public partial class UnitOfWorkManager : IUnitOfWorkManager
    {
        private bool _isDisposed;
        private readonly WebMvcContext context;
        public UnitOfWorkManager(WebMvcContext _context)
        {
            context = _context;
        }

        public IUnitOfWork NewUnitOfWork()
        {
            return new UnitOfWork(context);
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                context.Dispose();
                _isDisposed = true;
            }
        }
    }
}