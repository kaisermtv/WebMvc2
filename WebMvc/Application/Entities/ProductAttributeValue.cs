using System;
using WebMvc.Application.Lib;

namespace WebMvc.Application.Entities
{
    public partial class ProductAttributeValue : Entity
    {
        public ProductAttributeValue()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductAttributeId { get; set; }
        public string Value { get; set; }
    }
}
