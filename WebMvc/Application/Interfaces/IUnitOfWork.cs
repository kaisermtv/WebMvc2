using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMvc.Services;

namespace WebMvc.Application.Interfaces
{
    public partial interface IUnitOfWork : IDisposable
    {
        void Commit();
        void Rollback();
    }
}