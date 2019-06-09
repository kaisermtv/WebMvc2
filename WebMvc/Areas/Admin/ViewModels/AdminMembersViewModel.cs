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
    public class AdminLogOnViewModel
    {
        public string ReturnUrl { get; set; }

        [Required(ErrorMessage="Bạn cần nhập trường tài khoản!")]
        [DisplayName("Tài khoản")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Bạn cần nhập trường mật khẩu!")]
        [DataType(DataType.Password)]
        [DisplayName("Mật khẩu")]
        public string Password { get; set; }

        [DisplayName("Nhớ tôi")]
        public bool RememberMe { get; set; }
    }

    public class AdminListMembersViewModel
    {
        public List<MembershipUser> ListMembers { get; set; }
        public AdminPagingViewModel Paging { get; set; }
    }

    public class AdminCreateMemberViewModel
    {
        public string ReturnUrl { get; set; }

        [DisplayName("Tên tài khoản")]
        [Required]
        public string UserName { get; set; }
        [DisplayName("Mật khẩu")]
        [Required]
        public string Password { get; set; }
        [DisplayName("Nhập lại mật khẩu")]
        [Required]
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
        [Required]
        public string Email { get; set; }

        public List<AdminEditUserRoleViewModel> Roles { get; set; }
    }

    public class AdminEditMemberViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }
        [DisplayName("Tên tài khoản")]
        [ReadOnly(true)]
        public string UserName { get; set; }
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

        public List<AdminEditUserRoleViewModel> Roles { get; set; }
    }

    public class AdminEditUserRoleViewModel {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public bool Check { get; set; }
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
        public Guid Id { get; set; }
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

    public class AdminListRolesViewModel
    {
        public List<MembershipRole> ListRoles { get; set; }
        public AdminPagingViewModel Paging { get; set; }
    }

    public class AdminEditRoleViewModel
    {
        public Guid Id { get; set; }
        [DisplayName("Tên chức vụ")]
        public string RoleName { get; set; }

        public bool Lock { get; set; }

        public List<AdminEditRolePermissionsViewModel> AllPermissions { get; set; }
    }

    public class AdminEditRolePermissionsViewModel
    {
        public Guid Id { get; set; }
        public Guid? PermissionId { get; set; }
        public string PermissionName { get; set; }
        public bool Check { get; set; }
    }
}   