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
    public class AdminTopicListViewModel
    {
        public string Seach;
        public Category Cat;
        public List<Topic> ListTopic;
        public AdminPagingViewModel Paging;
		public List<SelectListItem> AllCategories { get; set; }
	}

    public class AdminCreateEditTopicViewModel
    {
        [Required]
        [StringLength(100)]
        [DisplayName("Tiêu đề")]
        public string Name { get; set; }

        [UIHint(AppConstants.EditorType), AllowHtml]
        public string Content { get; set; }

        [DisplayName("Chú ý")]
        public bool IsSticky { get; set; }

        [DisplayName("Khóa bài viết")]
        public bool IsLocked { get; set; }

        [Required]
        [DisplayName("Chọn danh mục")]
        public Guid? Category { get; set; }

        public string Tags { get; set; }

        [DisplayName("Topic.Label.PollCloseAfterDays")]
        public int PollCloseAfterDays { get; set; }

        public List<SelectListItem> Categories { get; set; }

        public IList<PollAnswer> PollAnswers { get; set; }

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
        


    }

}