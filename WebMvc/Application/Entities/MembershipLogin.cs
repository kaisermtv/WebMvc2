using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMvc.Application.Lib;

namespace WebMvc.Application.Entities
{
    public class MembershipLogin
    {
        public MembershipLogin()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Password { get; set; }
        public int TypeLogin { get; set; }
        public DateTime LoginDate { get; set; }
        public DateTime OnlineDate { get; set; }
        public bool Remember { get; set; }

    }
}