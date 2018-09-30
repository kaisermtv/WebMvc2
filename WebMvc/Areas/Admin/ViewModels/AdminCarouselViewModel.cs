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
    public class AdminCarouselsViewModel
    {
        public List<Carousel> Carousels { get; set; }
    }

    public class AdminCarouselEditViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [DisplayName("Tên Carousel")]
        [StringLength(600)]
        public string Name { get; set; }


        [DisplayName("Mô tả")]
        [DataType(DataType.MultilineText)]
        [UIHint(AppConstants.EditorType), AllowHtml]
        public string Description { get; set; }

        [DisplayName("Ảnh đại diện")]
        public string Image { get; set; }
        
        [DisplayName("Số thứ tự")]
        [Range(0, int.MaxValue)]
        public int SortOrder { get; set; }


        [DisplayName("Carousel cha")]
        public Guid? ParentCarousel { get; set; }
        public List<SelectListItem> AllCarousels { get; set; }

        [DisplayName("link liên kết")]
        public string Link { get; set; }
    }
}