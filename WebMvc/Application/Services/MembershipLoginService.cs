using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WebMvc.Application.Context;
using WebMvc.Application.Entities;
using WebMvc.Services;

namespace WebMvc.Services
{
    public class MembershipLoginService
    {
        private readonly WebMvcContext _context;
        private readonly CacheService _cacheService;
        private readonly LocalizationService _localizationService;

        public MembershipLoginService(WebMvcContext context, CacheService cacheService, LocalizationService localizationService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
            _localizationService = localizationService;
        }

        public MembershipLogin Get(Guid Id)
        {
            using(var Cmd = _context.CreateCommand())
            {
                Cmd.CommandText = "SELECT * FROM [dbo].[MembershipLogin] WHERE [Id] = @Id";
                Cmd.AddParameters("Id", Id);

                return Cmd.FindFirst<MembershipLogin>();
            }
        }

        public void Add(MembershipLogin login)
        {
            using (var Cmd = _context.CreateCommand())
            {
                Cmd.CommandText = "INSERT INTO [dbo].[MembershipLogin]([Id],[UserId],[Password],[TypeLogin],[LoginDate],[OnlineDate],[Remember])" +
                    "VALUES (@Id,@UserId,@Password,@TypeLogin,@LoginDate,@OnlineDate,@Remember)";

                Cmd.AddParameters("Id", login.Id);
                Cmd.AddParameters("UserId", login.UserId);
                Cmd.AddParameters("Password", login.Password);
                Cmd.AddParameters("TypeLogin", login.TypeLogin);
                Cmd.AddParameters("LoginDate", login.LoginDate);
                Cmd.AddParameters("OnlineDate", login.OnlineDate);
                Cmd.AddParameters("Remember", login.Remember);

                Cmd.command.ExecuteNonQuery();
            }
        }

        public void Del(Guid Id)
        {
            using(var Cmd = _context.CreateCommand())
            {
                Cmd.CommandText = "DELETE FROM [dbo].[MembershipLogin] WHERE Id = @Id";

                Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = Id;

                Cmd.command.ExecuteNonQuery();
            }
        }

        //public void Clear()
        //{
        //    using (var Cmd = _context.CreateCommand())
        //    {
        //        Cmd.CommandText = "DELETE FROM [dbo].[MembershipLogin] WHERE OnlineDate - @OnlineDate > @TimeOut";

        //        Cmd.Parameters.Add("Id", SqlDbType.UniqueIdentifier).Value = Id;

        //        Cmd.command.ExecuteNonQuery();
        //    }
        //}

    }
}