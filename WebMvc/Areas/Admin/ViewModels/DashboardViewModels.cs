using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMvc.Application;
using WebMvc.Application.Entities;

namespace WebMvc.Areas.Admin.ViewModels
{
    public class MainDashboardNavViewModel
    {
        public int PrivateMessageCount { get; set; }
        public int ModerateCount { get; set; }
    }
}