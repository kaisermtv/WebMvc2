using System;
using WebMvc.Application.Lib;

namespace WebMvc.Application.Entities
{
    public partial class TypeRoom : Entity
    {
        public TypeRoom()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsShow { get; set; }
        public string Note { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
