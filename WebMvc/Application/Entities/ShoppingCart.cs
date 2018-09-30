﻿using System;
using WebMvc.Application.Lib;

namespace WebMvc.Application.Entities
{
    public partial class ShoppingCart : Entity
    {
        public ShoppingCart()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Addren { get; set; }
        public string ShipName { get; set; }
        public string ShipPhone { get; set; }
        public string ShipAddren { get; set; }
        public string ShipNote { get; set; }
        public string TotalMoney { get; set; }
        public string Note { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
