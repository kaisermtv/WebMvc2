using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMvc.Application;
using WebMvc.Application.Annotation;
using WebMvc.Application.Entities;

namespace WebMvc.Areas.Admin.ViewModels
{
    public class AdminContactViewModel
    {
        public List<Contact> ListContact { get; set; }
    }

    public class AdminContactEditViewModel
    {
        [Required]
        [HiddenInput]
        public Guid Id { get; set; }

        [Required]
        [WebMvcResourceDisplayName("Contact.Name")]
        public string Name { get; set; }

        [Required]
        [WebMvcResourceDisplayName("Contact.Email")]
        public string Email { get; set; }

        [Required]
        [WebMvcResourceDisplayName("Contact.Content")]
        public string Content { get; set; }

        [WebMvcResourceDisplayName("Contact.IsCheck")]
        public bool IsCheck { get; set; }

        [WebMvcResourceDisplayName("Contact.Note")]
        public string Note { get; set; }
    }
}