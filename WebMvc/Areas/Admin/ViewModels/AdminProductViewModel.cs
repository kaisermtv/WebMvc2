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
    public class AdminAttributeViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }
        public Guid AttriId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int ValueType { get; set; }
        public bool IsNull { get; set; }
		public bool IsShowFindter { get; set; }
		public List<string> ValueOptions { get; set; }
		public List<AdminAttributeNumberFindter> FindterNums { get; set; }
	}

    public class AdminEditProductViewModel
    {
        public Guid ProductClass { get; set; }
        public IList<AdminAttributeViewModel> AllAttribute { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Tên sản phẩm")]
        public string Name { get; set; }

        [DisplayName("Nội dung")]
        [UIHint(AppConstants.EditorType), AllowHtml]
        public string Content { get; set; }
        
        [DisplayName("Khóa comment")]
        public bool IsLocked { get; set; }

        [Required]
        [DisplayName("Danh mục")]
        public Guid? Category { get; set; }
        
        public List<SelectListItem> Categories { get; set; }

        [DisplayName("Topic.Label.SubscribeToTopic")]
        public bool SubscribeToTopic { get; set; }

        [DisplayName("Ảnh đại diện")]
        public string Image { get; set; }

        // Permissions stuff
        //public CheckCreateTopicPermissions OptionalPermissions { get; set; }

        // Edit Properties
        [HiddenInput]
        public Guid Id { get; set; }

        [HiddenInput]
        public Guid TopicId { get; set; }

        public bool IsTopicStarter { get; set; }

        //public TopicAttributeViewModel TopicAttribute { get; set; }

    }

    public class AdminProductViewModel
    {
        public string Seach { get; set; }
        public Category Cat;
        public ProductClass ProductClass { get; set; }
        public List<Product> ListProduct { get; set; }
        public AdminPagingViewModel Paging { get; set; }
    }

    public class AdminProductClassViewModel
    {
        public List<ProductClass> ListProductClass { get; set; }
    }

    public class AdminEditProductClassAttributeViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        public bool IsCheck { get; set; }
        public bool IsShow { get; set; }
    }

    public class AdminEditProductClassViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [DisplayName("Tên nhóm sản phẩm")]
        public string Name { get; set; }

        [DisplayName("Mô tả")]
        [DataType(DataType.MultilineText)]
		[AllowHtml]
        public string Description { get; set; }

        [DisplayName("Khóa nhóm sản phẩm")]
        public bool IsLocked { get; set; }

        [DisplayName("Màu hiển thị")]
		[UIHint(AppConstants.EditorTemplateColourPicker), AllowHtml]
		public string Colour { get; set; }

        [DisplayName("Hình đại diện")]
        public string Image { get; set; }

        public IList<AdminEditProductClassAttributeViewModel> AllAttribute { get; set; }
    }

    public class AdminProductAttributeViewModel
    {
        public List<ProductAttribute> ListProductAttribute { get; set; }
    }

    public class AdminCreateProductAttributeViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [DisplayName("Tên thuộc tính (Tiếng anh)")]
        public string LangName { get; set; }
        
        [DisplayName("Kiểu dữ liệu")]
        public int ValueType { get; set; }

        [DisplayName("Cho phép null")]
        public bool IsNull { get; set; }
		
		[DisplayName("Hiển thị bộ lọc")]
		public bool IsShowFindter { get; set; }

		public List<SelectListItem> AllValueType { get; set; }

		public List<string> ValueOptions { get; set; }
		
		public List<AdminAttributeNumberFindter> FindterNums { get; set; }
	}

	public class AdminAttributeNumberFindter { 
		public string Name { get; set; }
		public double MaxValue { get; set; }
		public double MinValue { get; set; }
	}
}