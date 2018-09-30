using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMvc.ViewModels
{
    public class AjaxShowroomViewModel
    {
        public List<AjaxShowroomItemViewModel> Showrooms { get; set; }
    }

    public class AjaxShowroomItemViewModel
    {
        public string Name { get; set; }
        public string Tel { get; set; }
        public string Hotline { get; set; }
		public string Addren { get; set; }
        public string iFrameMap { get; set; }
    }
}