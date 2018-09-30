using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMvc.Application.Context;

namespace WebMvc.Application.Interfaces
{
    public partial interface IUnitOfWorkManager : IDisposable
    {
        IUnitOfWork NewUnitOfWork();
    }
}