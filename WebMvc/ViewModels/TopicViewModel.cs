﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMvc.Application;
using WebMvc.Application.Annotation;
using WebMvc.Application.Entities;

namespace WebMvc.ViewModels
{
    public class CreateEditTopicViewModel
    {
        [Required]
        [StringLength(100)]
        [WebMvcResourceDisplayName("Topic.Label.TopicTitle")]
        public string Name { get; set; }

        [UIHint(AppConstants.EditorType), AllowHtml]
        public string Content { get; set; }

        [WebMvcResourceDisplayName("Post.Label.IsStickyTopic")]
        public bool IsSticky { get; set; }

        [WebMvcResourceDisplayName("Post.Label.LockTopic")]
        public bool IsLocked { get; set; }

        [Required]
        [WebMvcResourceDisplayName("Topic.Label.Category")]
        public Guid? Category { get; set; }

        public string Tags { get; set; }

        [WebMvcResourceDisplayName("Topic.Label.PollCloseAfterDays")]
        public int PollCloseAfterDays { get; set; }

        public List<SelectListItem> Categories { get; set; }

        public IList<PollAnswer> PollAnswers { get; set; }

        [WebMvcResourceDisplayName("Topic.Label.SubscribeToTopic")]
        public bool SubscribeToTopic { get; set; }

        [WebMvcResourceDisplayName("Topic.Label.UploadFiles")]
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

    public class TopicListViewModel
    {
        public Category Cat;
        public List<Topic> ListTopic;
        public PagingViewModel Paging;
    }
    public class TopicViewModel
    {
        public Topic topic;
        public Category Cat;
        public Post post;
    }


}