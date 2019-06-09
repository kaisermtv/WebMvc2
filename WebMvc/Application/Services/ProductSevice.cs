using DataLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using WebMvc.Application;
using WebMvc.Application.Context;
using WebMvc.Application.Entities;
using WebMvc.Application.Lib;

namespace WebMvc.Services
{
    public partial class ProductSevice 
    {
        private readonly WebMvcContext _context;
        private readonly CacheService _cacheService;

        public ProductSevice(WebMvcContext context, CacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
        }

        #region Slug
        private bool CheckSlug(string slug, DataTable data)
        {
            foreach (DataRow it in data.Rows)
            {
                if (slug == it["Slug"].ToString()) return false;
            }

            return true;
        }

        private bool TestSlug(string slug, string newslug)
        {
            if (slug == null) slug = "";
            return Regex.IsMatch(slug, string.Concat("^", newslug, "(-[\\d]+)?$"));
        }

        private void CreateSlug(Product product)
        {
            var slug = ServiceHelpers.CreateUrl(product.Name);
            if (!TestSlug(product.Slug, slug))
            {
                var tmpSlug = slug;

                var Cmd = _context.CreateCommand();
                Cmd.CommandText = "SELECT Slug FROM [dbo].[Product] WHERE [Slug] LIKE @Slug AND [Id] != @Id ";

                Cmd.AddParameters("Id", product.Id);
                Cmd.AddParameters("Slug", string.Concat(slug, "%"));

                DataTable data = Cmd.FindAll();
                Cmd.Close();

                int i = 0;
                while (!CheckSlug(tmpSlug, data))
                {
                    i++;
                    tmpSlug = string.Concat(slug, "-", i);
                }

                product.Slug = tmpSlug;
            }
        }

		private void CreateSlug(ProductClass product)
		{
			var slug = ServiceHelpers.CreateUrl(product.Name);
			if (!TestSlug(product.Slug, slug))
			{
				var tmpSlug = slug;

				var Cmd = _context.CreateCommand();
				Cmd.CommandText = "SELECT Slug FROM [dbo].[ProductClass] WHERE [Slug] LIKE @Slug AND [Id] != @Id ";

                Cmd.AddParameters("Id", product.Id);
                Cmd.AddParameters("Slug", string.Concat(slug, "%"));

                DataTable data = Cmd.FindAll();
				Cmd.Close();

				int i = 0;
				while (!CheckSlug(tmpSlug, data))
				{
					i++;
					tmpSlug = string.Concat(slug, "-", i);
				}

				product.Slug = tmpSlug;
			}
		}
		#endregion

		#region Product

		public void Add(Product topic)
        {
            topic.CreateDate = DateTime.UtcNow;
            CreateSlug(topic);

            using (var Cmd = _context.CreateCommand())
            {
                bool ret = Cmd.Add<Product>(topic) > 0;
                Cmd.cacheStartsWithToClear(CacheKeys.Product.StartsWith);
                if (!ret) throw new Exception("Add Product false");
            }
        }

        public void Update(Product topic)
        {
            CreateSlug(topic);

            var Cmd = _context.CreateCommand();

            //Cmd.CommandText = "IF NOT EXISTS (SELECT * FROM [Topic] WHERE [Id] = @Id)";
            Cmd.CommandText = "UPDATE [dbo].[Product] SET [ProductClassId] = @ProductClassId, [Name] = @Name, [ShotContent] = @ShotContent,[Image] = @Image,[isAutoShotContent] = @isAutoShotContent, [CreateDate] = @CreateDate,"
                            + " [Slug] = @Slug, [Views] = @Views, [IsLocked] = @IsLocked, [Category_Id] = @Category_Id, [ProductPost_Id] = @ProductPost_Id, [MembershipUser_Id] = @MembershipUser_Id"
                            + " WHERE [Id] = @Id";

            Cmd.AddParameters("Id", topic.Id);
            Cmd.AddParameters("ProductClassId", topic.ProductClassId);
            Cmd.AddParameters("Name", topic.Name);
            Cmd.AddParameters("ShotContent", topic.ShotContent);
            Cmd.AddParameters("Image", topic.Image);
            Cmd.AddParameters("isAutoShotContent", topic.isAutoShotContent);

            Cmd.AddParameters("CreateDate", topic.CreateDate);
            //Cmd.AddParameters("SolvedReminderSent", SqlDbType.Bit).Value = topic.SolvedReminderSent;
            Cmd.AddParameters("Slug", topic.Slug);
            Cmd.AddParameters("Views", topic.Views);
            Cmd.AddParameters("IsLocked", topic.IsLocked);
            //Cmd.AddParameters("Pending", topic.Pending);
            Cmd.AddParameters("Category_Id", topic.Category_Id);
            Cmd.AddParameters("ProductPost_Id", topic.ProductPost_Id);
            Cmd.AddParameters("MembershipUser_Id", topic.MembershipUser_Id);

            bool ret = Cmd.command.ExecuteNonQuery() > 0;
			Cmd.cacheStartsWithToClear(CacheKeys.Product.StartsWith);

			Cmd.Close();

            if (!ret) throw new Exception("Update Product false");
        }


        public void Del(Product product)
        {
            var Cmd = _context.CreateCommand();
            Cmd.CommandText = "DELETE FROM [Product] WHERE Id = @Id";

            Cmd.AddParameters("Id", product.Id);

            Cmd.command.ExecuteNonQuery();
            Cmd.cacheStartsWithToClear(CacheKeys.Product.StartsWith);
            Cmd.Close();

        }

        public Product Get(Guid Id)
        {
            using (var Cmd = _context.CreateCommand())
            {
                Cmd.CommandText = "SELECT * FROM [Product] WHERE Id = @Id";

                Cmd.AddParameters("Id", Id);

                return Cmd.FindFirst<Product>();
            }
        }

        public Product GetBySlug(string Slug)
        {
            string cachekey = string.Concat(CacheKeys.Product.StartsWith, "GetBySlug-", Slug);
            var topic = _cacheService.Get<Product>(cachekey);
            if (topic == null)
            {
                using (var Cmd = _context.CreateCommand())
                {
                    Cmd.CommandText = "SELECT * FROM [dbo].[Product] WHERE [Slug] = @Slug";

                    Cmd.AddParameters("Slug", Slug);

                    topic = Cmd.FindFirst<Product>();
                    if (topic == null) return null;
                }

                _cacheService.Set(cachekey, topic, CacheTimes.OneDay);
            }
            return topic;
        }

        
        public List<Product> GetList(Guid Id, int limit = 10, int page = 1)
        {
            if (page == 0) page = 1;
            using (var Cmd = _context.CreateCommand())
            {
                Cmd.CommandText = "SELECT TOP " + limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY CreateDate DESC)) AS RowNum FROM  [Product] WHERE [ProductClassId] = @ProductClassId) AS MyDerivedTable WHERE RowNum > @Offset";

                Cmd.AddParameters("ProductClassId", Id);
                //Cmd.AddParameters("limit", SqlDbType.Int).Value = limit;
                Cmd.AddParameters("Offset", (page - 1) * limit);

                return Cmd.FindAll<Product>();
            }
        }

		public int GetCount(List<Category> cats)
		{
			if (cats.Count == 0) return 0;

			string cachekey = string.Concat(CacheKeys.Product.StartsWith, "GetCountForCatergories-", cats.GetHashCode());
			var count = _cacheService.Get<int?>(cachekey);
			if (count == null)
			{
				var Cmd = _context.CreateCommand();

				Cmd.CommandText = "SELECT COUNT(*) FROM  [dbo].[Product] WHERE Category_Id IN ('";
				for(int i = 0; i < cats.Count;i++)
				{
					if(i != 0) Cmd.CommandText += "','";
					Cmd.CommandText += cats[i].Id;
				}

				Cmd.CommandText += "')";
				

				count = (int)Cmd.command.ExecuteScalar();
				Cmd.Close();


				_cacheService.Set(cachekey, count, CacheTimes.OneDay);
			}
			return (int)count;
		}
		public List<Product> GetList(List<Category> cats, int limit = 10, int page = 1)
		{
            if (page == 0) page = 1;

            using (var Cmd = _context.CreateCommand())
            {

                Cmd.CommandText = "SELECT TOP " + limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY CreateDate DESC)) AS RowNum FROM  [Product] WHERE [Category_Id] IN ('";
                for (int i = 0; i < cats.Count; i++)
                {
                    if (i != 0) Cmd.CommandText += "','";
                    Cmd.CommandText += cats[i].Id;
                }
                Cmd.CommandText += "') ) AS MyDerivedTable WHERE RowNum > @Offset";

                Cmd.AddParameters("Offset", (page - 1) * limit);


                return Cmd.FindAll<Product>();
            }
		}

		public int GetCount(List<Category> cats, List<Guid> groups)
		{
			if (cats.Count == 0) return 0;

			var Cmd = _context.CreateCommand();

			Cmd.CommandText = "SELECT COUNT(*) FROM  [dbo].[Product] WHERE Category_Id IN ('";
			for (int i = 0; i < cats.Count; i++)
			{
				if (i != 0) Cmd.CommandText += "','";
				Cmd.CommandText += cats[i].Id;
			}

			if (groups != null && groups.Count > 0)
			{
				Cmd.CommandText += "') AND ProductClassId IN ('";
				for (int i = 0; i < groups.Count; i++)
				{
					if (i != 0) Cmd.CommandText += "','";
					Cmd.CommandText += groups[i];
				}
			}

			Cmd.CommandText += "')";
			int count = (int)Cmd.command.ExecuteScalar();
			Cmd.Close();


			return count;
		}

		public List<Product> GetList(List<Category> cats, List<Guid> groups, int limit = 10, int page = 1)
        {
            if (page == 0) page = 1;
            using (var Cmd = _context.CreateCommand())
            {

                Cmd.CommandText = "SELECT TOP " + limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY CreateDate DESC)) AS RowNum FROM  [Product] WHERE [Category_Id] IN ('";
                for (int i = 0; i < cats.Count; i++)
                {
                    if (i != 0) Cmd.CommandText += "','";
                    Cmd.CommandText += cats[i].Id;
                }

                if (groups != null && groups.Count > 0)
                {
                    Cmd.CommandText += "') AND ProductClassId IN ('";
                    for (int i = 0; i < groups.Count; i++)
                    {
                        if (i != 0) Cmd.CommandText += "','";
                        Cmd.CommandText += groups[i];
                    }
                }

                Cmd.CommandText += "') ) AS MyDerivedTable WHERE RowNum > @Offset";

                Cmd.AddParameters("Offset", (page - 1) * limit);

                return Cmd.FindAll<Product>();
            }
		}


		public int GetCount(Category cat)
        {
            string cachekey = string.Concat(CacheKeys.Product.StartsWith, "GetCountForCatergory-", cat.Id);
            var count = _cacheService.Get<int?>(cachekey);
            if (count == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT COUNT(*) FROM  [dbo].[Product] WHERE Category_Id = @Category_Id";

                Cmd.AddParameters("Category_Id", cat.Id);

                count = (int)Cmd.command.ExecuteScalar();
                Cmd.Close();


                _cacheService.Set(cachekey, count, CacheTimes.OneDay);
            }
            return (int)count;
        }
        public List<Product> GetList(Category cat, int limit = 10, int page = 1)
        {
            if (page == 0) page = 1;
            using (var Cmd = _context.CreateCommand())
            {

                Cmd.CommandText = "SELECT TOP " + limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY CreateDate DESC)) AS RowNum FROM  [Product] WHERE [Category_Id] = @Category_Id) AS MyDerivedTable WHERE RowNum > @Offset";

                Cmd.AddParameters("Category_Id", cat.Id);
                //Cmd.AddParameters("limit", SqlDbType.Int).Value = limit;
                Cmd.AddParameters("Offset", (page - 1) * limit);

                return Cmd.FindAll<Product>();
            }
        }

        public List<Product> GetListForCategory(Guid Id, int limit = 10, int page = 1)
        {
            if (page == 0) page = 1;

            using (var Cmd = _context.CreateCommand())
            {
                Cmd.CommandText = "SELECT TOP " + limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY CreateDate DESC)) FROM  [Product] WHERE Category_Id = @Category_Id) AS MyDerivedTable WHERE RowNum > @Offset";

                Cmd.AddParameters("Category_Id", Id);
                //Cmd.AddParameters("limit", SqlDbType.Int).Value = limit;
                Cmd.AddParameters("Offset", (page - 1) * limit);

                return Cmd.FindAll<Product>();
            }
        }

        public List<Product> GetListForClass(Guid Id, int limit = 10, int page = 1)
        {
            string cachekey = string.Concat(CacheKeys.Product.StartsWith, "GetListForClass-", Id,"-page", page,"-limit", limit);
            var list = _cacheService.Get<List<Product>>(cachekey);
            if (list == null)
            {
                if (page == 0) page = 1;
                using (var Cmd = _context.CreateCommand())
                {

                    Cmd.CommandText = "SELECT TOP " + limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY CreateDate DESC)) AS RowNum FROM  [Product] WHERE ProductClassId = @ProductClassId) AS MyDerivedTable WHERE RowNum > @Offset";

                    Cmd.AddParameters("ProductClassId", Id);
                    //Cmd.AddParameters("limit", SqlDbType.Int).Value = limit;
                    Cmd.AddParameters("Offset", (page - 1) * limit);

                    list = Cmd.FindAll<Product>();
                    if (list == null) return null;
                }

                _cacheService.Set(cachekey, list, CacheTimes.OneHour);
            }
            return list;
        }

        public int GetCount(ProductClass productClass)
        {
            string cachekey = string.Concat(CacheKeys.Product.StartsWith, "GetCountForClass-", productClass.Id);
            var count = _cacheService.Get<int?>(cachekey);
            if (count == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT COUNT(*) FROM  [dbo].[Product] WHERE ProductClassId = @ProductClassId";

                Cmd.AddParameters("ProductClassId", productClass.Id);

                count = (int)Cmd.command.ExecuteScalar();
                Cmd.Close();


                _cacheService.Set(cachekey, count, CacheTimes.OneDay);
            }
            return (int)count;
        }
        public List<Product> GetList(ProductClass productClass, int limit = 10, int page = 1)
        {
            return GetListForClass(productClass.Id, limit, page);
        }

        public int GetCount()
        {
            string cachekey = string.Concat(CacheKeys.Product.StartsWith, "GetCount");
            var count = _cacheService.Get<int?>(cachekey);
            if (count == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT COUNT(*) FROM  [dbo].[Product]";
                
                count = (int)Cmd.command.ExecuteScalar();
                Cmd.Close();


                _cacheService.Set(cachekey, count, CacheTimes.OneDay);
            }
            return (int)count;
        }
        public List<Product> GetList(int limit = 10, int page = 1)
        {
            if (page == 0) page = 1;
            using (var Cmd = _context.CreateCommand())
            {

                Cmd.CommandText = "SELECT TOP " + limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY CreateDate DESC)) AS RowNum FROM  [Product]) AS MyDerivedTable WHERE RowNum > @Offset";

                //Cmd.AddParameters("limit", SqlDbType.Int).Value = limit;
                Cmd.AddParameters("Offset", (page - 1) * limit);

                return Cmd.FindAll<Product>();
            }
        }

        public int GetCount(string seach)
        {
            if (seach.IsNullEmpty()) return GetCount();

            string cachekey = string.Concat(CacheKeys.Product.StartsWith, "GetCountForCatergory-seach-", seach);
            var count = _cacheService.Get<int?>(cachekey);
            if (count == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT COUNT(*) FROM  [dbo].[Product] WHERE [Name] LIKE  N'%'+UPPER(RTRIM(LTRIM(@Seach)))+'%'";
                Cmd.AddParameters("Seach", seach);

                count = (int)Cmd.command.ExecuteScalar();
                Cmd.Close();


                _cacheService.Set(cachekey, count, CacheTimes.OneMinute);
            }
            return (int)count;
        }
        public List<Product> GetList(string seach, int limit = 10, int page = 1)
        {
            if (seach.IsNullEmpty()) return GetList(limit, page);

            if (page == 0) page = 1;
            using (var Cmd = _context.CreateCommand())
            {

                Cmd.CommandText = "SELECT TOP " + limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY CreateDate DESC)) AS RowNum FROM  [Product] WHERE [Name] LIKE  N'%'+UPPER(RTRIM(LTRIM(@Seach)))+'%') AS MyDerivedTable WHERE RowNum > @Offset";

                Cmd.AddParameters("Seach", seach);
                Cmd.AddParameters("Offset", (page - 1) * limit);

                return Cmd.FindAll<Product>();
            }
        }

		public List<ProductClass> GetClassByProducts(List<Category> cats)
		{
			using (var Cmd = _context.CreateCommand())
			{
				Cmd.CommandText = "SELECT * FROM [dbo].[ProductClass] WHERE Id IN (SELECT ProductClassId FROM [dbo].[Product] WHERE [Category_Id] IN ('";
				for (int i = 0; i < cats.Count; i++)
				{
					if (i != 0) Cmd.CommandText += "','";
					Cmd.CommandText += cats[i].Id;
				}
				Cmd.CommandText += "') GROUP BY ProductClassId) ORDER BY [Name]";
				
				return Cmd.FindAll<ProductClass>();
			}
		}

		public List<ProductClass> GetClassByProducts(Category cat)
		{
			using (var Cmd = _context.CreateCommand())
			{
				Cmd.CommandText = "SELECT * FROM [dbo].[ProductClass] WHERE Id IN (SELECT ProductClassId FROM [dbo].[Product] WHERE [Category_Id] = @Category_Id GROUP BY ProductClassId) ORDER BY [Name]";

				Cmd.AddParameters("Category_Id", cat.Id);

				return Cmd.FindAll<ProductClass>();
			}
		}

		public FindterProduct GetFindterProduct()
		{
			return new FindterProduct(_context,_cacheService);
		}

		public class FindterProduct {
			private readonly WebMvcContext _context;
			private readonly CacheService _cacheService;

			public FindterProduct(WebMvcContext context, CacheService cacheService)
			{
				_cacheService = cacheService;
				_context = context as WebMvcContext;
			}

			private List<Guid> Cats = new List<Guid>();
			public FindterProduct AddCat(Category cat)
			{
				if (Cats.IndexOf(cat.Id) < 0)
				{
					Cats.Add(cat.Id);
				}

				return this;
			}

			public FindterProduct AddCat(List<Category> cats)
			{
				foreach (var it in cats)
				{
					AddCat(it);
				}

				return this;
			}

			private List<Guid> Group = new List<Guid>();
			public FindterProduct AddGroupId(Guid id)
			{
				if (Group.IndexOf(id) < 0)
				{
					Group.Add(id);
				}

				return this;
			}

			public FindterProduct AddGroupId(List<Guid> ids)
			{
				foreach (var it in ids)
				{
					AddGroupId(it);
				}

				return this;
			}


			private int AttCout = 0;
			private Hashtable Attributes = new Hashtable();
			private int ValCout = 0;
			public FindterProduct SetAttributeEquas(Guid AttributeId, string val)
			{
				Attribute att = null;
				if (Attributes.ContainsKey(AttributeId))
				{
					att = (Attribute)Attributes[AttributeId];
				} else
				{
					att = new Attribute()
					{
						AttributeId = AttributeId,
						Key = string.Concat("Val", AttCout++)
					};
					Attributes.Add(AttributeId, att);
				}

				var vl = new AttributeValue
				{
					type = Type.Equas,
					Value = val,
					key = string.Concat("Value", ValCout++)
				};

				att.AttrVal.Add(vl);

				return this;
			}

			public FindterProduct SetAttributeBetween(Guid Attribute, double min, double max)
			{


				return this;
			}

			private SQLCon GetQuery(SQLCon Cmd)
			{
				Cmd.CommandText += " FROM [dbo].[Product] AS P";

				if (Attributes.Count > 0)
				{
					foreach (DictionaryEntry it in Attributes)
					{
						var a = (Attribute)it.Value;
						Cmd.CommandText += string.Format(" INNER JOIN [dbo].[ProductAttributeValue] AS {0} ON {0}.[ProductId] = P.[Id] AND {0}.ProductAttributeId = '{1}' ", a.Key, a.AttributeId);
					}
				}

				Cmd.CommandText += " WHERE 1=1 ";
				if (Cats.Count == 1)
				{
					Cmd.CommandText += " AND Category_Id = @Category_Id ";
					Cmd.AddParameters("Category_Id", Cats[0]);
				}
				else if (Cats.Count > 1)
				{
					Cmd.CommandText += " AND Category_Id IN ('";
					for (int i = 0; i < Cats.Count; i++)
					{
						if (i != 0) Cmd.CommandText += "','";
						Cmd.CommandText += Cats[i];
					}

					Cmd.CommandText += "') ";
				}

				if (Group.Count == 1)
				{
					Cmd.CommandText += " AND ProductClassId = @ProductClassId ";
					Cmd.AddParameters("ProductClassId", Group[0]);
				}
				else if (Group.Count > 1)
				{
					Cmd.CommandText += " AND ProductClassId IN ('";
					for (int i = 0; i < Group.Count; i++)
					{
						if (i != 0) Cmd.CommandText += "','";
						Cmd.CommandText += Group[i];
					}

					Cmd.CommandText += "') ";
				}

				if (Attributes.Count > 0)
				{
					foreach (DictionaryEntry it in Attributes)
					{
						var a = (Attribute)it.Value;

						if (a.AttrVal.Count > 0)
						{
							if (a.AttrVal[0].type == Type.Equas)
							{
								Cmd.CommandText += string.Format(" AND ({0}.[Value] = @{1}", a.Key, a.AttrVal[0].key);
								Cmd.AddParameters(a.AttrVal[0].key, a.AttrVal[0].Value);
							} else if (a.AttrVal[0].type == Type.Between)
							{
								Cmd.CommandText += string.Format(" AND ((CONVERT(BIGINT,{0}.[Value]) BETWEEN @{1}Min AND @{1}Max)", a.Key, a.AttrVal[0].key);
								Cmd.AddParameters(a.AttrVal[0].key + "Min", a.AttrVal[0].Min);
								Cmd.AddParameters(a.AttrVal[0].key + "Max", a.AttrVal[0].Max);
							}
						}

						if (a.AttrVal.Count == 1)
						{
							Cmd.CommandText += ")";
						} else if(a.AttrVal.Count > 1)
						{
							for(int i = 1;i < a.AttrVal.Count; i++)
							{
								if (a.AttrVal[i].type == Type.Equas)
								{
									Cmd.CommandText += string.Format(" OR {0}.[Value] = @{1}", a.Key, a.AttrVal[i].key);
									Cmd.AddParameters(a.AttrVal[i].key, a.AttrVal[i].Value);
								}
								else if (a.AttrVal[i].type == Type.Between)
								{
									Cmd.CommandText += string.Format(" OR (CONVERT(BIGINT,{0}.[Value]) BETWEEN @{1}Min AND @{1}Max)", a.Key, a.AttrVal[i].key);
									Cmd.AddParameters(a.AttrVal[i].key + "Min", a.AttrVal[i].Min);
									Cmd.AddParameters(a.AttrVal[i].key + "Max", a.AttrVal[i].Max);
								}
							}

							Cmd.CommandText += ")";
						}

						
					}
				}

				return Cmd;
			}

			public int GetCount()
			{
				using (var Cmd = _context.CreateCommand())
				{
					Cmd.CommandText = "SELECT COUNT(*) ";
					GetQuery(Cmd);
					
					return (int)Cmd.command.ExecuteScalar();
				}
			}

			public List<Product> GetList(int limit = 10, int page = 1)
			{
				using (var Cmd = _context.CreateCommand())
				{
					if (page == 0) page = 1;
					Cmd.CommandText = string.Concat("SELECT TOP ",limit," * FROM ( SELECT P.*,(ROW_NUMBER() OVER(ORDER BY CreateDate DESC)) AS RowNum ");
					GetQuery(Cmd);
					Cmd.CommandText += " ) AS MyDerivedTable WHERE RowNum > @Offset";

					Cmd.AddParameters("Offset", (page - 1) * limit);

					return Cmd.FindAll<Product>();
				}
			}

			private class Attribute
			{
				public Guid AttributeId;
				public string Key;
				public List<AttributeValue> AttrVal = new List<AttributeValue>();
			}

			private class AttributeValue
			{
				public Type type;
				public string key;
				public string Value;
				public double Min;
				public double Max;
			}

			private enum Type
			{
				Equas,
				Between
			}
		}
		#endregion

		#region ProductAttributeValue
		public void Add(ProductAttributeValue cat)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "INSERT INTO [dbo].[ProductAttributeValue]([Id],[ProductId],[ProductAttributeId],[Value])"
                + " VALUES(@Id,@ProductId,@ProductAttributeId,@Value)";

            Cmd.AddParameters("Id", cat.Id);
            Cmd.AddParameters("ProductId", cat.ProductId);
            Cmd.AddParameters("ProductAttributeId", cat.ProductAttributeId);
            Cmd.AddParameters("Value", cat.Value);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(string.Concat(CacheKeys.Product.ProductAttributeValue, "ProductId-", cat.ProductId));
            Cmd.Close();

            if (!rt) throw new Exception("Add ProductAttributeValue false");
        }

        public void Set(Product product,ProductAttribute attribute,string value)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "IF NOT EXISTS (SELECT * FROM [dbo].[ProductAttributeValue] WHERE [ProductId] = @ProductId AND [ProductAttributeId] = @ProductAttributeId)";
            Cmd.CommandText += " BEGIN INSERT INTO [dbo].[ProductAttributeValue]([Id],[ProductId],[ProductAttributeId],[Value]) VALUES(@Id,@ProductId,@ProductAttributeId,@Value) END ";
            Cmd.CommandText += " ELSE BEGIN UPDATE [dbo].[ProductAttributeValue] SET [Value] = @Value WHERE [ProductId] = @ProductId AND [ProductAttributeId] = @ProductAttributeId END";

            Cmd.AddParameters("Id", GuidComb.GenerateComb());
            Cmd.AddParameters("ProductId", product.Id);
            Cmd.AddParameters("ProductAttributeId", attribute.Id);
            Cmd.AddParameters("Value", value);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(string.Concat(CacheKeys.Product.ProductAttributeValue, "ProductId-", product.Id));
            Cmd.Close();

            if (!rt) throw new Exception("Set ProductAttributeValue false");
        }

        public List<ProductAttributeValue> GetAllAttributeValue(Guid productid)
        {
            string cachekey = string.Concat(CacheKeys.Product.ProductAttributeValue, "ProductId-", productid, "-GetAllAttributeValue");
            var cachedSettings = _cacheService.Get<List<ProductAttributeValue>>(cachekey);
            if (cachedSettings == null)
            {
                using (var Cmd = _context.CreateCommand())
                {
                    Cmd.CommandText = "SELECT * FROM  [dbo].[ProductAttributeValue] WHERE [ProductId] = @ProductId ";
                    Cmd.AddParameters("ProductId", productid);

                    cachedSettings = Cmd.FindAll<ProductAttributeValue>();
                    if (cachedSettings == null) return null;

                }
                _cacheService.Set(cachekey, cachedSettings, CacheTimes.OneHour);
            }

            return cachedSettings;
        }

        public List<ProductAttributeValue> GetAllAttributeValue(Product product)
        {
            return GetAllAttributeValue(product.Id);
        }

        public ProductAttributeValue GetAttributeValue(Guid productid, Guid attributeid)
        {
            string cachekey = string.Concat(CacheKeys.Product.ProductAttributeValue, "ProductId-", productid, "-GetAttributeValue-", attributeid);

            var cat = _cacheService.Get<ProductAttributeValue>(cachekey);
            if (cat == null)
            {
                var allcat = GetAllAttributeValue(productid);
                if (allcat == null) return null;

                foreach (ProductAttributeValue it in allcat)
                {
                    if (it.ProductAttributeId == attributeid)
                    {
                        cat = it;
                        break;
                    }
                }

                _cacheService.Set(cachekey, cat, CacheTimes.OneMinute);
            }
            return cat;
        }

        public ProductAttributeValue GetAttributeValue(Product product, string key)
        {
            string cachekey = string.Concat(CacheKeys.Product.ProductAttributeValue, "ProductId-", product.Id, "-GetAttributeValue-key-", key);

            var attrivalue = _cacheService.Get<ProductAttributeValue>(cachekey);
            if (attrivalue == null)
            {
                var attribute = GetAttribute(key);
                attrivalue = GetAttributeValue(product.Id, attribute.Id);

                _cacheService.Set(cachekey, attrivalue, CacheTimes.OneMinute);
            }
            return attrivalue;
        }
        #endregion

        #region ProductClassAttribute
        public void Add(ProductClassAttribute cat)
        {
            var Cmd = _context.CreateCommand();
           
            Cmd.CommandText = "INSERT INTO [dbo].[ProductClassAttribute]([Id],[ProductClassId],[ProductAttributeId],[IsShow])"
                + " VALUES(@Id,@ProductClassId,@ProductAttributeId,@IsShow)";

            Cmd.AddParameters("Id", cat.Id);
            Cmd.AddParameters("ProductClassId", cat.ProductClassId);
            Cmd.AddParameters("ProductAttributeId", cat.ProductAttributeId);
            Cmd.AddParameters("IsShow", cat.IsShow);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(string.Concat(CacheKeys.Product.ProductClassAttribute, "ProductClassId-", cat.ProductClassId));
            Cmd.Close();

            if (!rt) throw new Exception("Add ProductAttribute false");
        }

        public List<ProductClassAttribute> GetListProductClassAttributeForProductClassId(Guid id)
        {
            string cachekey = string.Concat(CacheKeys.Product.ProductClassAttribute, "ProductClassId-", id ,"-GetListProductClassAttributeForProductClassId");
            var cachedSettings = _cacheService.Get<List<ProductClassAttribute>>(cachekey);
            if (cachedSettings == null)
            {
                using(var Cmd = _context.CreateCommand())
                {

                    Cmd.CommandText = "SELECT * FROM  [dbo].[ProductClassAttribute] WHERE [ProductClassId] = @ProductClassId ";

                    Cmd.AddParameters("ProductClassId", id);

                    cachedSettings = Cmd.FindAll<ProductClassAttribute>();
                    if (cachedSettings == null) return null;
                }

                _cacheService.Set(cachekey, cachedSettings, CacheTimes.OneDay);
            }

            return cachedSettings;
        }

        public ProductClassAttribute GetProductClassAttributeForProductClassId(Guid id,Guid ProductClassId)
        {
            string cachekey = string.Concat(CacheKeys.Product.ProductClassAttribute, "ProductClassId-", ProductClassId, "-GetProductClassAttributeForProductClassId-", id);

            var cat = _cacheService.Get<ProductClassAttribute>(cachekey);
            if (cat == null)
            {
                var allcat = GetListProductClassAttributeForProductClassId(ProductClassId);
                if (allcat == null) return null;

                foreach (ProductClassAttribute it in allcat)
                {
                    if (it.Id == id)
                    {
                        cat = it;
                        break;
                    }
                }

                _cacheService.Set(cachekey, cat, CacheTimes.OneHour);
            }
            return cat;
        }

        public void DelAllAttributeForProductClass(Guid guid)
        {
            var Cmd = _context.CreateCommand();
            
            Cmd.CommandText = "DELETE FROM [dbo].[ProductClassAttribute] WHERE [ProductClassId] = @ProductClassId";

            Cmd.AddParameters("ProductClassId", guid);

            Cmd.command.ExecuteNonQuery();
            Cmd.cacheStartsWithToClear(string.Concat(CacheKeys.Product.ProductClassAttribute, "ProductClassId-", guid));
            Cmd.Close();
        }
        #endregion

        #region ProductClass

        public void Add(ProductClass cat)
        {
            cat.DateCreated = DateTime.UtcNow;
            CreateSlug(cat);

            using (var Cmd = _context.CreateCommand())
            {
                bool rt = Cmd.Add<ProductClass>(cat) > 0;
                Cmd.cacheStartsWithToClear(CacheKeys.Product.ProductClass);
                if (!rt) throw new Exception("Add ProductAttribute false");
            }
        }


        public void Update(ProductClass cat)
        {
			CreateSlug(cat);

			var Cmd = _context.CreateCommand();

            //Cmd.CommandText = "IF NOT EXISTS (SELECT * FROM [Topic] WHERE [Id] = @Id)";
            Cmd.CommandText = "UPDATE [dbo].[ProductClass] SET [Name] = @Name,[Description] = @Description,[IsLocked] = @IsLocked,[Slug] = @Slug,[Colour] = @Colour,[Image] = @Image,[DateCreated] = @DateCreated WHERE [Id] = @Id";

            Cmd.AddParameters("Id", cat.Id);
            Cmd.AddParameters("Name", cat.Name);
            Cmd.AddParameters("Description", cat.Description);
            Cmd.AddParameters("IsLocked", cat.IsLocked);
            Cmd.AddParameters("Slug", cat.Slug);
            Cmd.AddParameters("Colour", cat.Colour);
            Cmd.AddParameters("Image", cat.Image);
            Cmd.AddParameters("DateCreated", cat.DateCreated);

            bool ret = Cmd.command.ExecuteNonQuery() > 0;
			Cmd.cacheStartsWithToClear(CacheKeys.Product.ProductClass);
			Cmd.Close();

            if (!ret) throw new Exception("Update ProductClass false");
        }

		public ProductClass GetProductClassBySlug(string Slug)
		{
			string cachekey = string.Concat(CacheKeys.Product.ProductClass, "GetProductClassBySlug-", Slug);
			var topic = _cacheService.Get<ProductClass>(cachekey);
			if (topic == null)
			{
				var allcat = GetAllProductClass();
				if (allcat == null) return null;

				foreach (ProductClass it in allcat)
				{
					if (it.Slug == Slug)
					{
						topic = it;
						break;
					}
				}

				_cacheService.Set(cachekey, topic, CacheTimes.OneDay);
			}
			return topic;
		}

		public ProductClass GetProductClass(Guid Id)
        {
            string cachekey = string.Concat(CacheKeys.Product.ProductClass, "GetProductClass-", Id);
            var topic = _cacheService.Get<ProductClass>(cachekey);
            if (topic == null)
            {
                using (var Cmd = _context.CreateCommand())
                {

                    Cmd.CommandText = "SELECT * FROM [dbo].[ProductClass] WHERE [Id] = @Id";

                    Cmd.AddParameters("Id", Id);

                    topic = Cmd.FindFirst<ProductClass>();
                    if (topic == null) return null;
                }
                _cacheService.Set(cachekey, topic, CacheTimes.OneDay);
            }
            return topic;
        }

        public List<ProductClass> GetAllProductClass()
        {
            string cachekey = string.Concat(CacheKeys.Product.ProductClass, "GetAllProductClass");
            var cachedSettings = _cacheService.Get<List<ProductClass>>(cachekey);
            if (cachedSettings == null)
            {

                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT * FROM  [dbo].[ProductClass] ORDER BY [Name] ";

				cachedSettings = Cmd.FindAll<ProductClass>();
                Cmd.Close();

                if (cachedSettings == null) return null;
				
                _cacheService.Set(cachekey, cachedSettings, CacheTimes.OneDay);
            }

            return cachedSettings;
        }
        #endregion

        #region ProductAttribute
        public void Add(ProductAttribute cat)
        {
            var Cmd = _context.CreateCommand();
            
            Cmd.CommandText = "INSERT INTO [dbo].[ProductAttribute]([Id],[LangName],[ValueType],[IsNull],[IsLock],[ValueOption],[ValueFindter],[IsShowFindter])"
				+ " VALUES(@Id,@LangName,@ValueType,@IsNull,@IsLock,@ValueOption,@ValueFindter,@IsShowFindter)";

            Cmd.AddParameters("Id", cat.Id);
            Cmd.AddParameters("LangName", cat.LangName);
            Cmd.AddParameters("ValueType", cat.ValueType);
            Cmd.AddParameters("IsNull", cat.IsNull);
            Cmd.AddParameters("IsLock", cat.IsLock);
            Cmd.AddParameters("ValueOption", cat.ValueOption);
            Cmd.AddParameters("ValueFindter", cat.ValueFindter);
            Cmd.AddParameters("IsShowFindter", cat.IsShowFindter);

			bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Product.Attribute);
            Cmd.Close();

            if (!rt) throw new Exception("Add ProductAttribute false");
        }

        public void Update(ProductAttribute cat)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "UPDATE [dbo].[ProductAttribute] SET [LangName] = @LangName,[ValueType] = @ValueType,[IsNull] = @IsNull,[IsLock] = @IsLock,[ValueOption] = @ValueOption,[ValueFindter] = @ValueFindter,[IsShowFindter] = @IsShowFindter WHERE Id = @Id";


            Cmd.AddParameters("Id", cat.Id);
            Cmd.AddParameters("LangName", cat.LangName);
            Cmd.AddParameters("ValueType", cat.ValueType);
            Cmd.AddParameters("IsNull", cat.IsNull);
            Cmd.AddParameters("IsLock", cat.IsLock);
			Cmd.AddParameters("ValueOption", cat.ValueOption);
			Cmd.AddParameters("ValueFindter", cat.ValueFindter);
			Cmd.AddParameters("IsShowFindter", cat.IsShowFindter);

			bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Product.Attribute);
            Cmd.Close();

            if (!rt) throw new Exception("Update ProductAttribute false");
        }

        public ProductAttribute GetAttribute(Guid id)
        {
            string cachekey = string.Concat(CacheKeys.Product.Attribute, "GetAttribute-", id);

            var cat = _cacheService.Get<ProductAttribute>(cachekey);
            if (cat == null)
            {
                var allcat = GetAllAttribute();
                if (allcat == null) return null;

                foreach (ProductAttribute it in allcat)
                {
                    if (it.Id == id)
                    {
                        cat = it;
                        break;
                    }
                }

                _cacheService.Set(cachekey, cat, CacheTimes.OneDay);
            }
            return cat;
        }

        public ProductAttribute GetAttribute(string name)
        {
            string cachekey = string.Concat(CacheKeys.Product.Attribute, "GetAttribute-Name-", name);

            var cat = _cacheService.Get<ProductAttribute>(cachekey);
            if (cat == null)
            {
                var allcat = GetAllAttribute();
                if (allcat == null) return null;

                foreach (ProductAttribute it in allcat)
                {
                    if (it.LangName == name)
                    {
                        cat = it;
                        break;
                    }
                }

                _cacheService.Set(cachekey, cat, CacheTimes.OneDay);
            }
            return cat;
        }

        public List<ProductAttribute> GetAllAttribute()
        {
            string cachekey = string.Concat(CacheKeys.Product.Attribute, "GetAllAttribute");
            var cachedSettings = _cacheService.Get<List<ProductAttribute>>(cachekey);
            if (cachedSettings == null)
            {

                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT * FROM  [dbo].[ProductAttribute] ORDER BY [LangName] ";

				cachedSettings = Cmd.FindAll<ProductAttribute>();
                Cmd.Close();

                if (cachedSettings == null) return null;
				
                _cacheService.Set(cachekey, cachedSettings, CacheTimes.OneDay);
            }

            return cachedSettings;
        }


		public List<ProductAttribute> GetAttributeByClass(List<ProductClass> cats,bool IsShowFindter = true)
		{
			if (cats == null || (cats != null && cats.Count == 0)) return new List<ProductAttribute>();

			//string cachekey = string.Concat(CacheKeys.Product.Attribute, "GetAttributeByClass-", cats.GetHashCode());
			//var cachedSettings = _cacheService.Get<List<ProductAttribute>>(cachekey);
			//if (cachedSettings == null)
			//{
				using (var Cmd = _context.CreateCommand())
				{
					Cmd.CommandText = "SELECT * FROM [dbo].[ProductAttribute] WHERE [IsShowFindter] = @IsShowFindter AND Id IN (SELECT ProductAttributeId FROM [dbo].[ProductClassAttribute] WHERE [ProductClassId] IN ('";
					for (int i = 0; i < cats.Count; i++)
					{
						if (i != 0) Cmd.CommandText += "','";
						Cmd.CommandText += cats[i].Id;
					}
					Cmd.CommandText += "') GROUP BY ProductAttributeId) ORDER BY [LangName]";

					Cmd.AddParameters("IsShowFindter", IsShowFindter);

					return Cmd.FindAll<ProductAttribute>();
					//cachedSettings = Cmd.FindAll<ProductAttribute>();
				}

				//_cacheService.Set(cachekey, cachedSettings, CacheTimes.OneDay);
			//}

		}
		#endregion



	}
}
