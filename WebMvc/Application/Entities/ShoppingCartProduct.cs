using System;
using WebMvc.Application.Lib;

namespace WebMvc.Application.Entities
{
    public partial class ShoppingCartProduct : Entity
    {
        public ShoppingCartProduct()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public Guid ShoppingCartId { get; set; }
        public Guid ProductId { get; set; }
        public string Price { get; set; }
        public int CountProduct { get; set; }
        
    }
}
