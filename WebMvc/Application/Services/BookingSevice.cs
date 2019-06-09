using System;
using System.Collections.Generic;
using System.Data;
using WebMvc.Application;
using WebMvc.Application.Context;
using WebMvc.Application.Entities;
using WebMvc.Application.Lib;

namespace WebMvc.Services
{
    public partial class BookingSevice
    {
        private readonly WebMvcContext _context;
        private readonly CacheService _cacheService;

        public BookingSevice(WebMvcContext context, CacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
        }

        public void Add(Booking booking)
        {
            //string cachekey = string.Concat(CacheKeys.Category.StartsWith, "getSetting-", key);

            using (var Cmd = _context.CreateCommand())
            {
                bool rt = Cmd.Add<Booking>(booking) > 0;

                Cmd.cacheStartsWithToClear(CacheKeys.Booking.StartsWith);

                if (!rt) throw new Exception("Add Booking false");
            }

            //    var Cmd = _context.CreateCommand();

            //booking.CreateDate = DateTime.UtcNow;

            //Cmd.CommandText = "INSERT INTO [Booking]([Id],[NamePartner],[CheckIn],[CheckOut],[Adukts],[Adolescent],[Children],[TypeRoom_Id],[Phone],[Email],[IsCheck],[Note],[CreateDate])"
            //    + " VALUES(@Id,@NamePartner,@CheckIn,@CheckOut,@Adukts,@Adolescent,@Children,@TypeRoom_Id,@Phone,@Email,@IsCheck,@Note,@CreateDate)";

            //Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = booking.Id;
            //Cmd.AddParameters("NamePartner", booking.NamePartner);
            //Cmd.AddParameters("CheckIn", booking.CheckIn);
            //Cmd.AddParameters("CheckOut", booking.CheckOut);
            //Cmd.AddParameters("Adukts", booking.Adukts);
            //Cmd.AddParameters("Adolescent", booking.Adolescent);
            //Cmd.AddParameters("Children", booking.Children);
            //Cmd.AddParameters("TypeRoom_Id", booking.TypeRoom_Id);
            //Cmd.AddParameters("IsCheck", booking.IsCheck);
            //Cmd.AddParameters("Phone", booking.Phone);
            //Cmd.AddParameters("Note", booking.Note);
            //Cmd.AddParameters("Email", booking.Email);
            //Cmd.AddParameters("CreateDate", booking.CreateDate);

            //bool rt = Cmd.command.ExecuteNonQuery() > 0;
            //Cmd.cacheStartsWithToClear(CacheKeys.Booking.StartsWith);
            //Cmd.Close();

            //if (!rt) throw new Exception("Add Booking false");
        }


        public Booking Get(Guid Id)
        {
            using (var Cmd = _context.CreateCommand())
            {
                Cmd.CommandText = "SELECT * FROM [Booking] WHERE Id = @Id";

                Cmd.AddParameters("Id", Id);

                return Cmd.FindFirst<Booking>();
            }
        }


        public void Del(Booking emp)
        {
            var Cmd = _context.CreateCommand();
            Cmd.CommandText = "DELETE FROM [Booking] WHERE Id = @Id";

            Cmd.AddParameters("Id", emp.Id);
            //Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = emp.Id;

            Cmd.command.ExecuteNonQuery();
            Cmd.cacheStartsWithToClear(CacheKeys.Booking.StartsWith);
            Cmd.Close();

        }

        public List<Booking> GetList(int limit = 10, int page = 1)
        {
            if (page == 0) page = 1;

            using (var Cmd = _context.CreateCommand())
            {
                Cmd.CommandText = "SELECT TOP " + limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY CreateDate DESC)) AS RowNum FROM  [Booking]) AS MyDerivedTable WHERE RowNum > @Offset";

                //Cmd.Parameters.Add("limit", SqlDbType.Int).Value = limit;
                //Cmd.Parameters.Add("Offset", SqlDbType.Int).Value = (page - 1) * limit;
                Cmd.AddParameters("Offset", (page - 1) * limit);


                return Cmd.FindAll<Booking>();
            }
        }
    }
}
