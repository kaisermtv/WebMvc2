using System;
using WebMvc.Application.Lib;

namespace WebMvc.Application.Entities
{
    public partial class ProductClassAttribute : Entity
    {
        public ProductClassAttribute()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public Guid ProductClassId { get; set; }
        public Guid ProductAttributeId { get; set; }
        public bool IsShow { get; set; }
    }
}
