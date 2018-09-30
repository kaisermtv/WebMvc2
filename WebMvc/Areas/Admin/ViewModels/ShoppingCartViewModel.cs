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
    public class ShoppingCartViewModel
    {
        public List<ShoppingCart> shoppingCarts;
        public AdminPagingViewModel Paging;
    }

    public class ShoppingCartEditViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }
        [DisplayName("Tên đầy đủ")]
        public string Name { get; set; }
        [DisplayName("Hòm thư")]
        public string Email { get; set; }
        [DisplayName("Số điện thoại")]
        public string Phone { get; set; }
        [DisplayName("Địa chỉ")]
        public string Addren { get; set; }
        [DisplayName("Người nhận")]
        public string ShipName { get; set; }
        [DisplayName("Điện thoại người nhận")]
        public string ShipPhone { get; set; }
        [DisplayName("Địa chỉ người nhận")]
        public string ShipAddren { get; set; }
        [DisplayName("Chú thích người nhận")]
        public string ShipNote { get; set; }
        [DisplayName("Tổng giá tiền")]
        public string TotalMoney { get; set; }
        [DisplayName("Chú thích")]
        [AllowHtml]
        public string Note { get; set; }
        [DisplayName("Trạng thái")]
        public int Status { get; set; }
        [DisplayName("Ngày đăng")]
        public DateTime CreateDate { get; set; }

        public List<ShoppingCartProduct> products;
    }
}