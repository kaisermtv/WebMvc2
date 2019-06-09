using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMvc.Application.Entities;

namespace WebMvc.Areas.Admin.ViewModels
{
    public class PermissionsViewModel
    {
        public List<Permission> Permissions { get; set; }
    }

    public class EditPermissionsViewModel
    {
        public Guid Id { get; set; }
        [DisplayName("Phụ thuộc quyền hạn cha")]
        public Guid? PermissionId { get; set; }
        [Required]
        [DisplayName("Tên quyền hạn")]
        public string Name { get; set; }
        public bool IsGlobal { get; set; }
        public bool Lock { get; set; }

        public List<SelectListItem> Permissions { get; set; }
    }
}