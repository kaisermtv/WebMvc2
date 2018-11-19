using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebMvc.Application;
using WebMvc.Application.Context;
using WebMvc.Application.Entities;

namespace WebMvc.Services
{
    public partial class CarouselService
    {
        private readonly WebMvcContext _context;
        private readonly CacheService _cacheService;
        private readonly LocalizationService _localizationService;

        public CarouselService(WebMvcContext context, CacheService cacheService, LocalizationService localizationService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
            _localizationService = localizationService;
        }



        #region DataRowToMenu
        private Carousel DataRowToMenu(DataRow data)
        {
            if (data == null) return null;

            var menu = new Carousel();

            menu.Id = new Guid(data["Id"].ToString());
            menu.Name = data["Name"].ToString();
            menu.Description = data["Description"].ToString();
            menu.Image = data["Image"].ToString();
            menu.Link = data["Link"].ToString();
            menu.Image = data["Image"].ToString();

            if (data["Carousel_Id"] != DBNull.Value) menu.Carousel_Id = new Guid(data["Carousel_Id"].ToString());

            return menu;
        }
        #endregion


        public void Add(Carousel menu)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "INSERT INTO [dbo].[Carousel]([Id],[Carousel_Id],[Name],[Description],[Link],[Image],[SortOrder])"
                + " VALUES(@Id,@Carousel_Id,@Name,@Description,@Link,@Image,@SortOrder)";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = menu.Id;
            Cmd.AddParameters("Name", menu.Name);
            Cmd.AddParameters("Description", menu.Description);
            Cmd.AddParameters("Link", menu.Link);
            Cmd.AddParameters("Image", menu.Image);
            Cmd.AddParameters("SortOrder", menu.SortOrder);
            Cmd.AddParameters("Carousel_Id", menu.Carousel_Id);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Carousel.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Add Carousel false");
        }


        public void Update(Carousel menu)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "UPDATE [dbo].[Carousel] SET [Carousel_Id] = @Carousel_Id,[Name] = @Name,[Description] = @Description,"
                                + "[Link] = @Link,[Image] = @Image,[SortOrder] = @SortOrder WHERE [Id] = @Id";


            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = menu.Id;
            Cmd.AddParameters("Name", menu.Name);
            Cmd.AddParameters("Description", menu.Description);
            Cmd.AddParameters("Link", menu.Link);
            Cmd.AddParameters("Image", menu.Image);
            Cmd.AddParameters("SortOrder", menu.SortOrder);
            Cmd.AddParameters("Carousel_Id", menu.Carousel_Id);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Carousel.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Update Carousel false");
        }

        public void Del(Carousel menu)
        {
            var Cmd = _context.CreateCommand();
            Cmd.CommandText = "DELETE FROM [dbo].[Carousel] WHERE Id = @Id";

            Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = menu.Id;

            Cmd.command.ExecuteNonQuery();
            Cmd.cacheStartsWithToClear(CacheKeys.Carousel.StartsWith);
            Cmd.Close();
        }

        public Carousel Get(string id)
        {
            return Get(new Guid(id));
        }

        public Carousel Get(Guid id)
        {
            string cachekey = string.Concat(CacheKeys.Carousel.StartsWith, "Get-", id);

            var cat = _cacheService.Get<Carousel>(cachekey);
            if (cat == null)
            {
                var allcat = GetAll();
                if (allcat == null) return null;

                foreach (Carousel it in allcat)
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


        public List<Carousel> GetSubCarousel(Carousel cat)
        {
            var cacheKey = string.Concat(CacheKeys.Menu.StartsWith, "GetSubCarousel", "-", cat);
            var list = _cacheService.Get<List<Carousel>>(cacheKey);
            if (list == null)
            {
                var cats = GetAll();
                list = new List<Carousel>();

                int i = 0, x = 0;
                while (true)
                {
                    if (cats[i].Carousel_Id == cat.Id)
                    {
                        list.Add(cats[i]);
                    }

                    i++;
                    if (i >= cats.Count)
                    {
                        if (x >= list.Count) break;
                        cat = list[x];
                        x++;
                        i = 0;
                    }
                }

                _cacheService.Set(cacheKey, list, CacheTimes.OneMinute);
            }

            return list;
        }

        public List<Carousel> GetAll()
        {
            string cachekey = string.Concat(CacheKeys.Carousel.StartsWith, "GetAll");

            var allCat = _cacheService.Get<List<Carousel>>(cachekey);
            if (allCat == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT * FROM  [dbo].[Carousel] ORDER BY SortOrder ASC";

                DataTable data = Cmd.FindAll();
                Cmd.Close();

                if (data == null) return null;

                allCat = new List<Carousel>();

                foreach (DataRow it in data.Rows)
                {
                    allCat.Add(DataRowToMenu(it));
                }


                foreach (var it in allCat)
                {
                    if (it.Carousel_Id == null)
                    {
                        SetLiveMenu(it, allCat);
                    }
                }

                _cacheService.Set(cachekey, allCat, CacheTimes.OneDay);
            }
            return allCat;
        }


        private void SetLiveMenu(Carousel cat, List<Carousel> allcat, int leve = 2)
        {
            foreach (var it in allcat)
            {
                if (cat.Id == it.Carousel_Id)
                {
                    it.Level = leve;
                    SetLiveMenu(it, allcat, leve + 1);
                }
            }
        }


        public List<SelectListItem> GetBaseSelectListCarousel(List<Carousel> allowedCategories)
        {
            var cacheKey = string.Concat(CacheKeys.Menu.StartsWith, "GetBaseSelectListCarousel-", allowedCategories.GetHashCode());
            var list = _cacheService.Get<List<SelectListItem>>(cacheKey);
            if (list == null)
            {
                list = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
                foreach (var cat in allowedCategories)
                {
                    var catName = string.Concat(LevelDashes(cat.Level), cat.Level > 1 ? " " : "", cat.Name);
                    list.Add(new SelectListItem { Text = catName, Value = cat.Id.ToString() });
                }
				
                _cacheService.Set(cacheKey, list, CacheTimes.OneDay);
            }
            return list;
        }


        public List<Carousel> GetAllSubCarousels(Carousel cat, List<Carousel> allowedCategories)
        {
            var cacheKey = string.Concat(CacheKeys.Menu.StartsWith, "GetAllSubCarousel", "-", cat, "-", allowedCategories.GetHashCode());
            var list = _cacheService.Get<List<Carousel>>(cacheKey);
            if (list == null)
            {
                list = new List<Carousel>();

                int i = 0, x = 0;
                while (true)
                {
                    if (allowedCategories[i].Carousel_Id == cat.Id)
                    {
                        list.Add(allowedCategories[i]);
                    }

                    i++;
                    if (i >= allowedCategories.Count)
                    {
                        if (x >= list.Count) break;
                        cat = list[x];
                        x++;
                        i = 0;
                    }
                }

                _cacheService.Set(cacheKey, list, CacheTimes.OneDay);
            }

            return list;
        }

        #region GetMenusParenMenu

        public List<Carousel> GetCarouselsParenCarousel(Carousel cat)
        {
            var cacheKey = string.Concat(CacheKeys.Carousel.StartsWith, "GetCarouselsParenCarousel", "-", cat);
            var list = _cacheService.Get<List<Carousel>>(cacheKey);
            if (list == null)
            {
                var cats = GetAll();
                list = new List<Carousel>(cats);
                list.Remove(cat);

                var sublist = GetAllSubCarousels(cat, cats);

                foreach (Carousel it in sublist)
                {
                    list.Remove(it);
                }

                _cacheService.Set(cacheKey, list, CacheTimes.OneDay);
            }
            return list;
        }
        #endregion




        private static string LevelDashes(int level)
        {
            if (level > 1)
            {
                var sb = new StringBuilder();
                for (var i = 0; i < level - 1; i++)
                {
                    sb.Append("-");
                }
                return sb.ToString();
            }
            return string.Empty;
        }
    }
}