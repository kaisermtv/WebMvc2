using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using WebMvc.Application;
using WebMvc.Application.Context;
using WebMvc.Application.Entities;
using WebMvc.Application.Lib;


namespace WebMvc.Services
{
    public partial class PermissionService
    {
        private readonly WebMvcContext _context;
        private readonly CacheService _cacheService;

        public PermissionService(WebMvcContext context, CacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
        }

        public List<Permission> GetPermissions(List<MembershipRole> roles)
        {
            if(roles == null || roles.Count == 0)
            {
                return new List<Permission>();
            }
            roles.Sort();
            string InRoles = "(";
            for (int i = 0; i < roles.Count;i++)
            {
                if (i == 0) InRoles += roles[i].Id.ToString();
                else InRoles += "," + roles[i].Id.ToString();
            }
            InRoles += ")";

            string cachekey = string.Concat(CacheKeys.Permission.StartsWith, "GetPermissions-", InRoles);
            var cachedSettings = _cacheService.Get<List<Permission>>(cachekey);
            if (cachedSettings == null)
            {
                using (var Cmd = _context.CreateCommand())
                {
                    Cmd.CommandText = "SELECT P.* FROM [dbo].[Permission] AS P INNER JOIN [dbo].[PermissionsInRoles] AS K ON P.[Id] = K.[PermissionId]  WHERE K.[RoleId] IN " + InRoles;

                    cachedSettings = Cmd.FindAll<Permission>();
                }

                _cacheService.Set(cachekey, cachedSettings, CacheTimes.OneDay);
            }

            return cachedSettings;
        }

        public List<Permission> GetPermissions(MembershipRole roles)
        {
            string cachekey = string.Concat(CacheKeys.Permission.StartsWith, "GetPermissions-", roles.Id);
            var cachedSettings = _cacheService.Get<List<Permission>>(cachekey);
            if (cachedSettings == null)
            {
                using (var Cmd = _context.CreateCommand())
                {
                    Cmd.CommandText = "SELECT P.* FROM [dbo].[Permission] AS P INNER JOIN [dbo].[PermissionsInRoles] AS K ON P.[Id] = K.[PermissionId]  WHERE K.[RoleId] = @RoleId ";
                    Cmd.AddParameters("RoleId", roles.Id);
                    cachedSettings = Cmd.FindAll<Permission>();
                }

                _cacheService.Set(cachekey, cachedSettings, CacheTimes.OneDay);
            }

            return cachedSettings;
        }
        
        public void ClearPermissionsInRole(MembershipRole role)
        {
            using (var Cmd = _context.CreateCommand())
            {
                Cmd.cacheStartsWithToClear(CacheKeys.Role.StartsWith);

                Cmd.CommandText = "DELETE FROM [PermissionsInRoles] WHERE [RoleId] = @Id";

                Cmd.AddParameters("Id", role.Id);

                Cmd.command.ExecuteNonQuery();
                Cmd.cacheStartsWithToClear(CacheKeys.Permission.StartsWith);
            }
        }

        public void AddPermissionInRole(Guid PermissionId,Guid RoleId)
        {
            using (var Cmd = _context.CreateCommand())
            {
                Cmd.CommandText += "INSERT INTO [dbo].[PermissionsInRoles]([Id],[PermissionId],[RoleId])";
                Cmd.CommandText += " VALUES(@Id,@PermissionId,@RoleId)";

                Cmd.AddParameters("Id", GuidComb.GenerateComb());
                Cmd.AddParameters("PermissionId", PermissionId);
                Cmd.AddParameters("RoleId", RoleId);

                bool ret = Cmd.command.ExecuteNonQuery() > 0;
                if (!ret) throw new Exception("Add PermissionsInRoles false");
                Cmd.cacheStartsWithToClear(CacheKeys.Permission.StartsWith);
            }
        }

        #region MembershipRole Action
        public void Add(Permission role)
        {
            using (var Cmd = _context.CreateCommand())
            {
                Cmd.CommandText = "IF NOT EXISTS (SELECT * FROM [Permission] WHERE [Id] = @Id OR [Name] = @Name)";
                Cmd.CommandText += " BEGIN INSERT INTO [Permission]([Id],[PermissionId],[Name],[IsGlobal])";
                Cmd.CommandText += " VALUES(@Id,@PermissionId,@Name,@IsGlobal) END ";

                Cmd.AddParameters("Id", role.Id);
                Cmd.AddParameters("PermissionId", role.PermissionId);
                Cmd.AddParameters("Name", role.Name);
                Cmd.AddParameters("IsGlobal", role.IsGlobal);

                bool ret = Cmd.command.ExecuteNonQuery() > 0;
                if (!ret) throw new Exception("Add Permission false");
                Cmd.cacheStartsWithToClear(CacheKeys.Permission.StartsWith);
            }
        }

        public List<Permission> GetAll()
        {
            string cachekey = string.Concat(CacheKeys.Permission.StartsWith, "GetAll");
            var cachedSettings = _cacheService.Get<List<Permission>>(cachekey);
            if (cachedSettings == null)
            {
                using (var Cmd = _context.CreateCommand())
                {
                    Cmd.CommandText = "SELECT * FROM [dbo].[Permission]";

                    cachedSettings = Cmd.FindAll<Permission>();
                }
                   
                _cacheService.Set(cachekey, cachedSettings, CacheTimes.OneDay);
            }

            return cachedSettings;
        }

        public Permission Get(Guid Id,bool isCache = true )
        {
            if (isCache)
            {
                string cachekey = string.Concat(CacheKeys.Permission.StartsWith, "Get-", Id);
                var cachedSettings = _cacheService.Get<Permission>(cachekey);
                if (cachedSettings == null)
                {

                    var data = GetAll();

                    foreach (var it in data)
                    {
                        if (it.Id == Id)
                        {
                            cachedSettings = it;
                            break;
                        }
                    }
                    if (cachedSettings == null) return null;

                    _cacheService.Set(cachekey, cachedSettings, CacheTimes.OneDay);
                }

                return cachedSettings;
            }
            else
            {
                using(var Cmd = _context.CreateCommand())
                {
                    Cmd.CommandText = "SELECT * FROM [dbo].[Permission] WHERE [Id] = @Id";
                    Cmd.AddParameters("Id", Id);

                    return Cmd.FindFirst<Permission>();
                }
            }
            
        }

        public string GetName(Guid Id)
        {
            var data = Get(Id);

            if (data == null) return null;
            return data.Name;
        }

        public void Update(Permission role)
        {
            var Cmd = _context.CreateCommand();
            Cmd.cacheStartsWithToClear(CacheKeys.Permission.StartsWith);

            Cmd.CommandText = "UPDATE [Permission] SET [Name] = @Name, [IsGlobal] = @IsGlobal WHERE [Id] = @Id";

            Cmd.AddParameters("Id", role.Id);
            Cmd.AddParameters("Name", role.Name);
            Cmd.AddParameters("IsGlobal", role.IsGlobal);

            bool ret = Cmd.command.ExecuteNonQuery() > 0;

            Cmd.Close();

            if (!ret) throw new Exception("Update Permission false");
        }

        public void Delete(Guid Id)
        {
            using (var Cmd = _context.CreateCommand())
            {
                Cmd.cacheStartsWithToClear(CacheKeys.Role.StartsWith);

                Cmd.CommandText = "DELETE FROM [Permission] WHERE [Id] = @Id";

            Cmd.AddParameters("Id", Id);

                bool ret = Cmd.command.ExecuteNonQuery() > 0;
                Cmd.cacheStartsWithToClear(CacheKeys.Permission.StartsWith);

                if (!ret) throw new Exception("Delete Permission false");
            }

            using (var Cmd = _context.CreateCommand())
            {
                Cmd.cacheStartsWithToClear(CacheKeys.Role.StartsWith);

                Cmd.CommandText = "DELETE FROM [PermissionsInRoles] WHERE [PermissionId] = @Id";

                Cmd.AddParameters("Id", Id);

                bool ret = Cmd.command.ExecuteNonQuery() > 0;
                Cmd.cacheStartsWithToClear(CacheKeys.Permission.StartsWith);

                if (!ret) throw new Exception("Delete Permission false");
            }
        }

        #endregion


        public List<SelectListItem> GetBaseSelectListPermissions(List<Permission> allowedPermissions)
        {
            var cacheKey = string.Concat(CacheKeys.Permission.StartsWith, "GetBaseSelectListMenus-", allowedPermissions.GetHashCode());
            var list = _cacheService.Get<List<SelectListItem>>(cacheKey);
            if (list == null)
            {
                list = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
                foreach (var it in allowedPermissions)
                {
                    list.Add(new SelectListItem { Text = it.Name, Value = it.Id.ToString() });
                }

                _cacheService.Set(cacheKey, list, CacheTimes.OneDay);
            }
            return list;
        }

        public List<Permission> GetAllSubPermissions(Permission permission, List<Permission> allowedPermissions)
        {
            var cacheKey = string.Concat(CacheKeys.Permission.StartsWith, "GetAllSubPermissions", "-", permission.Id, "-", allowedPermissions.GetHashCode());
            var list = _cacheService.Get<List<Permission>>(cacheKey);
            if (list == null)
            {
                list = new List<Permission>();

                int i = 0, x = 0;
                while (true)
                {
                    if (allowedPermissions[i].PermissionId == permission.Id)
                    {
                        list.Add(allowedPermissions[i]);
                    }

                    i++;
                    if (i >= allowedPermissions.Count)
                    {
                        if (x >= list.Count) break;
                        permission = list[x];
                        x++;
                        i = 0;
                    }
                }

                _cacheService.Set(cacheKey, list, CacheTimes.OneHour);
            }

            return list;
        }

        public List<Permission> GetPermissionsParenPermission(Permission permission)
        {
            var cacheKey = string.Concat(CacheKeys.Permission.StartsWith, "GetPermissionsParenPermission", "-", permission.Id);
            var list = _cacheService.Get<List<Permission>>(cacheKey);
            if (list == null)
            {
                var permissions = GetAll();
                list = new List<Permission>(permissions);
                list.Remove(permission);

                var sublist = GetAllSubPermissions(permission, permissions);

                foreach (Permission it in sublist)
                {
                    list.Remove(it);
                }

                _cacheService.Set(cacheKey, list, CacheTimes.OneHour);
            }
            return list;
        }
    }
}
