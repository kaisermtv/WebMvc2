using System;
using WebMvc.Application.Lib;

namespace WebMvc.Application.Entities
{
    public partial class ProductAttribute : Entity
    {
        public ProductAttribute()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string LangName { get; set; }
        public int ValueType { get; set; }
		public string ValueOption { get; set; }
		public string ValueFindter { get; set; }
		public bool IsNull { get; set; }
		public bool IsShowFindter { get; set; }
		public bool IsLock { get; set; }
    }
}
