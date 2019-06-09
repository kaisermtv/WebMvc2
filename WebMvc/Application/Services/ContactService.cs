using System;
using System.Collections.Generic;
using System.Data;
using WebMvc.Application;
using WebMvc.Application.Context;
using WebMvc.Application.Entities;
using WebMvc.Application.Lib;

namespace WebMvc.Services
{
    public partial class ContactService
    {
        private readonly WebMvcContext _context;
        private readonly CacheService _cacheService;

        public ContactService(WebMvcContext context, CacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
        }

        #region DataRowToEntity
        private Contact DataRowToContact(DataRow data)
        {
            if (data == null) return null;

            Contact contact = new Contact();

            contact.Id = new Guid(data["Id"].ToString());
            contact.Name = data["Name"].ToString();
            contact.Email = data["Email"].ToString();
            contact.Content = data["Content"].ToString();
            contact.IsCheck = (bool)data["IsCheck"];
            contact.CreateDate = (DateTime)data["CreateDate"];
            contact.Note = data["Note"].ToString();
            
            return contact;
        }
        #endregion

        public void Add(Contact contact)
        {
            //string cachekey = string.Concat(CacheKeys.Category.StartsWith, "getSetting-", key);

            var Cmd = _context.CreateCommand();

            contact.CreateDate = DateTime.UtcNow;
            
            Cmd.CommandText = "INSERT INTO [Contact]([Id],[Name],[Email],[Content],[IsCheck],[Note],[CreateDate])"
                + " VALUES(@Id,@Name,@Email,@Content,@IsCheck,@Note,@CreateDate)";

            Cmd.AddParameters("Id", contact.Id);
            Cmd.AddParameters("Name", contact.Name);
            Cmd.AddParameters("Email", contact.Email);
            Cmd.AddParameters("Content", contact.Content);
            Cmd.AddParameters("IsCheck", contact.IsCheck);
            Cmd.AddParameters("Note", contact.Note);
            Cmd.AddParameters("CreateDate", contact.CreateDate);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Contact.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Add Contact false");
        }

        public Contact Get(Guid Id)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "SELECT * FROM [Contact] WHERE Id = @Id";

            Cmd.AddParameters("Id", Id);

            DataRow data = Cmd.FindFirst();
            if (data == null) return null;

            return DataRowToContact(data);
        }

        public void Update(Contact contact)
        {
            var Cmd = _context.CreateCommand();

            Cmd.CommandText = "UPDATE [Contact] SET [Name] = @Name,[Email] = @Email,[Content] = @Content,[IsCheck] = @IsCheck, [Note] = @Note,[CreateDate] = @CreateDate WHERE Id = @Id";

            Cmd.AddParameters("Id", contact.Id);
            Cmd.AddParameters("Name", contact.Name);
            Cmd.AddParameters("Email", contact.Email);
            Cmd.AddParameters("Content", contact.Content);
            Cmd.AddParameters("IsCheck", contact.IsCheck);
            Cmd.AddParameters("Note", contact.Note);
            Cmd.AddParameters("CreateDate", contact.CreateDate);

            bool rt = Cmd.command.ExecuteNonQuery() > 0;
            Cmd.cacheStartsWithToClear(CacheKeys.Contact.StartsWith);
            Cmd.Close();

            if (!rt) throw new Exception("Update Contact false");
        }


        public void Del(Contact emp)
        {
            var Cmd = _context.CreateCommand();
            Cmd.CommandText = "DELETE FROM [Contact] WHERE Id = @Id";


            Cmd.command.ExecuteNonQuery();
            Cmd.cacheStartsWithToClear(CacheKeys.Contact.StartsWith);
            Cmd.Close();

        }


        public int GetCount()
        {
            string cachekey = string.Concat(CacheKeys.Contact.StartsWith, "GetCount");
            var count = _cacheService.Get<int?>(cachekey);
            if (count == null)
            {
                var Cmd = _context.CreateCommand();

                Cmd.CommandText = "SELECT COUNT(*) FROM  [dbo].[Contact]";

                count = (int)Cmd.command.ExecuteScalar();
                Cmd.Close();


                _cacheService.Set(cachekey, count, CacheTimes.OneDay);
            }
            return (int)count;
        }
        public List<Contact> GetList(int limit = 10, int page = 1)
        {
            var Cmd = _context.CreateCommand();

            if (page == 0) page = 1;

            Cmd.CommandText = "SELECT TOP " + limit + " * FROM ( SELECT *,(ROW_NUMBER() OVER(ORDER BY CreateDate DESC)) AS RowNum FROM  [Contact]) AS MyDerivedTable WHERE RowNum > @Offset";

            //Cmd.Parameters.Add("limit", SqlDbType.Int).Value = limit;
            Cmd.AddParameters("Offset", (page - 1) * limit);

            DataTable data = Cmd.FindAll();
            Cmd.Close();

            if (data == null) return null;

            List<Contact> rt = new List<Contact>();
            foreach (DataRow it in data.Rows)
            {
                rt.Add(DataRowToContact(it));
            }

            return rt;
        }
    }
}
