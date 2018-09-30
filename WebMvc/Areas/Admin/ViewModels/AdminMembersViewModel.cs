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
    public class AdminListMembersViewModel
    {
        public List<MembershipUser> ListMembers { get; set; }
        public AdminPagingViewModel Paging { get; set; }
    }

    public class AdminCreateMemberViewModel
    {
        [DisplayName("Tên tài khoản")]
        public string UserName { get; set; }
        [DisplayName("Mật khẩu")]
        public string Password { get; set; }
        [DisplayName("Nhập lại mật khẩu")]
        public string Password2 { get; set; }
        [DisplayName("Câu hỏi bí mật")]
        public string PasswordQuestion { get; set; }
        [DisplayName("Câu trả lời bí mật")]
        public string PasswordAnswer { get; set; }
        [DisplayName("Kích hoạt tài khoản")]
        public bool IsApproved { get; set; }
        [DisplayName("Khóa tài khoản")]
        public bool IsLockedOut { get; set; }
        [DisplayName("Cấm tài khoản")]
        public bool IsBanned { get; set; }
        [DisplayName("Ảnh đại diện")]
        public string Avatar { get; set; }
        [DisplayName("Hòm thư")]
        public string Email { get; set; }
    }

    public class AdminChangePass
    {
        [DisplayName("Tài khoản")]
        public string UserName { get; set; }
        [DisplayName("Mật khẩu cũ")]
        [Required]
        public string OldPassword { get; set; }
        [DisplayName("Mật khẩu mới")]
        [Required]
        public string NewPassword { get; set; }
        [DisplayName("Nhập lại mật khẩu mới")]
        [Required]
        public string NewPassword2 { get; set; }
    }

    public class AdminNewPassViewModel
    {
        [DisplayName("Tài khoản")]
        public string UserName { get; set; }
        [DisplayName("Mật khẩu mới")]
        [Required]
        public string NewPassword { get; set; }
        [DisplayName("Nhập lại mật khẩu mới")]
        [Required]
        public string NewPassword2 { get; set; }
    }

    public class AdminChangeInfo
    {
        [DisplayName("Tài khoản")]
        public string UserName { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }
        
    }
}   