using System;
using WebMvc.Application.Lib;

namespace WebMvc.Application.Entities
{
    public partial class Permission : Entity
    {
        public Permission()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public Guid? PermissionId { get; set; }
        public string Name { get; set; }
        public bool IsGlobal { get; set; }
        public bool Lock { get; set; }
    }
}
