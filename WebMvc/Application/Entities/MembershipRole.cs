using System;
using WebMvc.Application.Lib;

namespace WebMvc.Application.Entities
{
    public partial class MembershipRole :Entity
    {
        public MembershipRole()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool Lock { get; set; }
    }
}
