using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMvc.Application;

namespace WebMvc.Areas.Admin.ViewModels
{
    public class CustomCodeViewModels
    {
        [AllowHtml]
        [DisplayName("Mã tiêu đề tùy chỉnh")]
        public string CustomHeaderCode { get; set; }

        [AllowHtml]
        [DisplayName("Mã chân trang tùy chỉnh")]
        public string CustomFooterCode { get; set; }
    }

    public class AdminGeneralSettingViewModel
    {
        [DisplayName("Tên trang web")]
        //[Required]
        [StringLength(200)]
        public string WebsiteName { get; set; }

        [DisplayName("Tên miền trang web")]
        //[Required]
        [StringLength(200)]
        public string WebsiteUrl { get; set; }

        [DisplayName("Hình ảnh")]
        [Description("Hình ảnh hiển thị mặc định khi chia sẻ liên kết")]
        public string WebSiteImage { get; set; }

        [DisplayName("Keyword")]
        [Description("Các từ khóa tìm kiếm")]
        public string Keyword { get; set; }

        [DisplayName("Page Title")]
        [MaxLength(80)]
        public string PageTitle { get; set; }

        [DisplayName("Meta Desc")]
        [MaxLength(200)]
        public string MetaDesc { get; set; }

        [DisplayName("Website Footer")]
        [UIHint(AppConstants.EditorType), AllowHtml]
        public string WebsiteFooter { get; set; }


        [DisplayName("Facebook")]
        public string Facebook { get; set; }

        [DisplayName("Twister")]
        public string Twister { get; set; }

        [DisplayName("Instagram")]
        public string Instagram { get; set; }

        [DisplayName("Linker")]
        public string Linker { get; set; }

        [DisplayName("Skype")]
        public string Skype { get; set; }

        [DisplayName("YouTube")]
        public string YouTube { get; set; }

        [DisplayName("Google+")]
        public string Google { get; set; }


        [DisplayName("Đóng trang web")]
        [Description("Đóng trang web để bảo trì")]
        public bool IsClosed { get; set; }

        [DisplayName("Cho phép nguồn cấp dữ liệu Rss")]
        [Description("Hiển thị biểu tượng nguồn cấp dữ liệu RSS cho Chủ đề và Danh mục")]
        public bool EnableRSSFeeds { get; set; }

        [DisplayName("Hiển thị chỉnh sửa theo chi tiết về bài viết")]
        public bool DisplayEditedBy { get; set; }

        [DisplayName("Cho phép tệp đính kèm trên bài đăng")]
        public bool EnablePostFileAttachments { get; set; }

        [DisplayName("Cho phép bài viết được đánh dấu là giải pháp")]
        public bool EnableMarkAsSolution { get; set; }

        [DisplayName("Khung thời gian trong ngày chờ trước khi email nhắc nhở được gửi đến người tạo chủ đề, cho tất cả các chủ đề chưa được đánh dấu là giải pháp - Đặt thành 0 để tắt")]
        public int MarkAsSolutionReminderTimeFrame { get; set; }

        [DisplayName("Bật báo cáo spam")]
        public bool EnableSpamReporting { get; set; }

        [DisplayName("Bật biểu tượng cảm xúc (Smilies)")]
        public bool EnableEmoticons { get; set; }

        [DisplayName("Cho phép thành viên báo cáo thành viên khác")]
        public bool EnableMemberReporting { get; set; }

        [DisplayName("Cho phép đăng ký email")]
        public bool EnableEmailSubscriptions { get; set; }

        [DisplayName("Thành viên mới phải xác nhận tài khoản của họ thông qua một liên kết được gửi trong email - Sẽ không hoạt động với tài khoản Twitter!")]
        public bool NewMemberEmailConfirmation { get; set; }

        [DisplayName("Cho phép thành viên mới")]
        public bool ManuallyAuthoriseNewMembers { get; set; }

        [DisplayName("Quản trị email khi đăng ký thành viên mới")]
        public bool EmailAdminOnNewMemberSignUp { get; set; }

        [DisplayName("Số lượng chủ đề trên mỗi trang")]
        public int TopicsPerPage { get; set; }

        [DisplayName("Số bài viết trên mỗi trang")]
        public int PostsPerPage { get; set; }

        [DisplayName("Số hoạt động trên mỗi trang")]
        public int ActivitiesPerPage { get; set; }

        [DisplayName("Cho phép tin nhắn riêng tư")]
        public bool EnablePrivateMessages { get; set; }
        
        [DisplayName("Vô hiệu hóa đăng ký chuẩn")]
        public bool DisableStandardRegistration { get; set; }
        
    }

    public class AdminContactInformationSettingViewModel
    {
        [DisplayName("Địa chỉ chính")]
        public string MainAddress { get; set; }

        [DisplayName("Hotline")]
        public string Hotline { get; set; }
    }

    public class AdminLanguageSettingViewModel
    {
        [DisplayName("Language Default")]
        public Guid? LanguageDefault { get; set; }
        public List<SelectListItem> AllLanguage { get; set; }
    }

    public class AdminTermsConditionsSettingViewModel
    {
        [DisplayName("Điều khoản điều kiện")]
        [UIHint(AppConstants.EditorType), AllowHtml]
        public string TermsConditions { get; set; }
    }
    public class AdminEmailSettingViewModel
    {
        [DisplayName("Hòm thư nhận")]
        public string InEmail { get; set; }
    }

    public class AdminRegistrationSettingViewModel
    {
        [DisplayName("Khóa chức năng đăng ký tài khoản.")]
        public bool LockRegister { get; set; }
    }

    public class AdminBusinessSettingViewModel
    {
        [DisplayName("Tên doanh nghiệp")]
        public string BusinessName { get; set; }
        [DisplayName("Người đại diên")]
        public string RepresentAtive { get; set; }
        [DisplayName("Chức vụ")]
        public string RepresentPosition { get; set; }

        [DisplayName("Giới thiệu")]
        [UIHint(AppConstants.EditorType), AllowHtml]
        public string Introduce { get; set; }
        


        [DisplayName("Lời chào")]
        public string Greeting { get; set; }
        

        [DisplayName("Hotline")]
        public string Hotline { get; set; }
        [DisplayName("Hotline Image")]
        public HttpPostedFileBase[] Files { get; set; }
        public string HotlineImg { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Fanpage")]
        public string Fanpage { get; set; }
        public List<AdminShowroomSettingViewModel> Addrens { get; set; }

        [DisplayName("Fancebook Chat")]
        public string FanChat { get; set; }
        

        [DisplayName("Tài khoản ngân hàng")]
        public string BankID { get; set; }
        [DisplayName("Chủ tài khoản ngân hàng")]
        public string BankUser { get; set; }
        [DisplayName("Tên ngân hàng")]
        public string BankName { get; set; }
        [DisplayName("Loại hình thanh toán")]
        public string BankPay { get; set; }

    }

    public class AdminShowroomSettingViewModel
    {
        public string Addren { get; set; }
        public string Name { get; set; }
        public string Tel { get; set; }
        public string Hotline { get; set; }

		[AllowHtml]
        public string iFrameMap { get; set; }
    }


}