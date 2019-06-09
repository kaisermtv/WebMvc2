using System;
using System.Collections.Generic;
using System.Data;
using WebMvc.Application;
using WebMvc.Application.Context;
using WebMvc.Application.Entities;
using WebMvc.Application.Lib;

namespace WebMvc.Services
{
    public partial class ShoppingCartProductService
    {
        private readonly WebMvcContext _context;
        private readonly CacheService _cacheService;

        public ShoppingCartProductService(WebMvcContext context, CacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
        }


        #region DataRowToEntity
        private ShoppingCartProduct DataRowToShoppingCartProduct(DataRow data)
        {
            if (data == null) return null;

            var cart = new ShoppingCartProduct();

            cart.Id = new Guid(data["Id"].ToString());
            cart.CountProduct = (int)data["CountProduct"];
            cart.Price = data["Price"].ToString();
            cart.ProductId = new Guid(data["ProductId"].ToString());
            cart.ShoppingCartId = new Guid(data["ShoppingCartId"].ToString());


            return cart;
        }
        #endregion


        public void Add(ShoppingCartProduct cat)
        {
            var Cmd = _context.CreateCommand();
            
            Cmd.CommandText = "INSERT INTO [dbo].[ShoppingCartProduct]([Id],[CountProduct],[Price] ,[ProductId],[ShoppingCartId])"
                + " VALUES(@Id,@CountProduct,@Price,@ProductId,@ShoppingCartId)";

            Cmd.AddParameters("Id", cat.Id);
            Cmd.AddParameters("CountProduct", cat.CountProduct);
            Cmd.AddParameters("Price", cat.Price);
            Cmd.AddParameters("ProductId", cat.ProductId);
            Cmd.AddParameters("ShoppingCartId", cat.ShoppingCartId);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.ShoppingCartProduct.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Add ShoppingCartProduct false");
        }


        public ShoppingCartProduct Get(Guid Id)
        {
            string cachekey = string.Concat(CacheKeys.ShoppingCartProduct.StartsWith, "Get-", Id);
            var cart = _cacheService.Get<ShoppingCartProduct>(cachekey);
            if (cart == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT * FROM  [ShoppingCartProduct] WHERE [Id] = @Id";
                Cmd.AddParameters("Id", Id);

                DataRow data = Cmd.FindFirst();
                if (data == null) return null;

                cart = DataRowToShoppingCartProduct(data);

                Cmd.Close();

                _cacheService.Set(cachekey, cart, CacheTimes.OneHour);
            }
            return cart;
        }

        public void Del(ShoppingCart shoppingCart)
        {
            var Cmd = _context.CreateCommand();
            Cmd.CommandText = "DELETE FROM [ShoppingCartProduct] WHERE ShoppingCartId = @Id";

            Cmd.AddParameters("Id", shoppingCart.Id);

            Cmd.command.ExecuteNonQuery();
            Cmd.cacheStartsWithToClear(CacheKeys.ShoppingCartProduct.StartsWith);
            Cmd.Close();

        }

        public List<ShoppingCartProduct> GetList(ShoppingCart cart)
        {
            string cachekey = string.Concat(CacheKeys.ShoppingCartProduct.StartsWith, "GetList-ShoppingCartId-", cart.Id);
            var list = _cacheService.Get<List<ShoppingCartProduct>>(cachekey);
            if (list == null)
            {
                var Cmd = _context.CreateCommand();
                
                Cmd.CommandText = "SELECT * FROM  [dbo].[ShoppingCartProduct] WHERE [ShoppingCartId] = @ShoppingCartId";
                Cmd.AddParameters("ShoppingCartId", cart.Id);

                DataTable data = Cmd.FindAll();
                Cmd.Close();

                if (data == null) return null;

                list = new List<ShoppingCartProduct>();
                foreach (DataRow it in data.Rows)
                {
                    list.Add(DataRowToShoppingCartProduct(it));
                }

                _cacheService.Set(cachekey, list, CacheTimes.OneDay);
            }
            return list;
        }

    }
}
