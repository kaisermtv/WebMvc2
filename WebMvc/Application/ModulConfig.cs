using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMvc.Application.Lib;

namespace WebMvc.Application
{
    public static class ModulConfig
    {
        public static string[] Moduls = new string[]
        {
            "",

        };



        public static AdminMenuItem[] AdminMenu { get; private set; } = new AdminMenuItem[]{
            new AdminMenuItem{
                MenuName = "Trang chủ",
                IconMenu = "fa-arrow-left",

                UrlString =  "~/",
            },
            new AdminMenuItem{
                MenuName = "Bảng điều khiển",
                IconMenu = "fa-dashboard",

                ActionName = "Index",
                ControllerName = "Admin",
                RouterValues = new { area = "Admin"},
            },
            new AdminMenuItem{
                MenuName = "Tổng quan",
                IconMenu = "fa-cogs",

                SubMenu = new AdminMenuItem[]
                {
                    new AdminMenuItem{
                        MenuName = "Thông tin website",

                        ActionName = "General",
                        ControllerName = "Overview",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem{
                        MenuName = "Thông tin doanh nghiệp",

                        ActionName = "Business",
                        ControllerName = "Overview",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem{
                        MenuName = "Điều khoản và điều kiện",

                        ActionName = "TermsConditions",
                        ControllerName = "Overview",
                        RouterValues = new { area = "Admin"},
                    },
                }
            },
            new AdminMenuItem{
                MenuName = "Quản lý Menus",
                IconMenu = "fa-bars",

                ActionName = "Index",
                ControllerName = "AdminMenu",
                RouterValues = new { area = "Admin"},
            },
            new AdminMenuItem{
                MenuName = "Quản lý bài viết",
                IconMenu = "fa-file-text-o",

                SubMenu = new AdminMenuItem[]
                {
                    new AdminMenuItem
                    {
                        MenuName = "Danh sách bài viết",

                        ActionName = "Index",
                        ControllerName = "AdminTopic",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        isHidden = true,
                        MenuName = "Đăng bài",

                        ActionName = "Create",
                        ControllerName = "AdminTopic",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        isHidden = true,
                        MenuName = "Sửa bài",

                        ActionName = "Edit",
                        ControllerName = "AdminTopic",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        isHidden = true,
                        MenuName = "Xóa bài viết",

                        ActionName = "Delete",
                        ControllerName = "AdminTopic",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        MenuName = "Quản lý nhóm bài viết",

                        ActionName = "News",
                        ControllerName = "AdminCategory",
                        RouterValues = new { area = "Admin"},
                    },
                    
                    new AdminMenuItem
                    {
                        isHidden = true,
                        MenuName = "Thêm nhóm bài viết",

                        ActionName = "NewCatNews",
                        ControllerName = "AdminCategory",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        isHidden = true,
                        MenuName = "Sửa nhóm bài viết",

                        ActionName = "EditCatNews",
                        ControllerName = "AdminCategory",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        isHidden = true,
                        MenuName = "Xóa nhóm bài viết",

                        ActionName = "DelCatNews",
                        ControllerName = "AdminCategory",
                        RouterValues = new { area = "Admin"},
                    },
                }
            },
            new AdminMenuItem{
                MenuName = "Quản lý sản phẩm",
                IconMenu = "fa-file-text-o",

                SubMenu = new AdminMenuItem[]
                {
                    new AdminMenuItem
                    {
                        MenuName = "Danh sách sản phẩm",

                        ActionName = "Product",
                        ControllerName = "AdminProduct",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        isHidden = true,
                        MenuName = "Thêm sản phẩm",

                        ActionName = "CreateProduct",
                        ControllerName = "AdminProduct",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        isHidden = true,
                        MenuName = "Chỉnh sửa sản phẩm",

                        ActionName = "EditProduct",
                        ControllerName = "AdminProduct",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        isHidden = true,
                        MenuName = "Xóa sản phẩm",

                        ActionName = "DelProduct",
                        ControllerName = "AdminProduct",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        MenuName = "Quản lý loại sản phẩm",

                        ActionName = "Index",
                        ControllerName = "AdminProduct",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        isHidden = true,
                        MenuName = "Thêm loại sản phẩm",

                        ActionName = "Create",
                        ControllerName = "AdminProduct",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        isHidden = true,
                        MenuName = "Chỉnh sửa loại sản phẩm",

                        ActionName = "Edit",
                        ControllerName = "AdminProduct",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        isHidden = true,
                        MenuName = "Xóa loại sản phẩm",

                        ActionName = "Del",
                        ControllerName = "AdminProduct",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        MenuName = "Quản lý nhóm sản phẩm",

                        ActionName = "Product",
                        ControllerName = "AdminCategory",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        isHidden = true,
                        MenuName = "Thêm nhóm sản phẩm",

                        ActionName = "NewCatProduct",
                        ControllerName = "AdminCategory",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        isHidden = true,
                        MenuName = "Sửa nhóm sản phẩm",

                        ActionName = "EditCatProduct",
                        ControllerName = "AdminCategory",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        isHidden = true,
                        MenuName = "Xóa nhóm sản phẩm",

                        ActionName = "DelCatProduct",
                        ControllerName = "AdminCategory",
                        RouterValues = new { area = "Admin"},
                    },
                }
            },
            new AdminMenuItem{
                MenuName = "Đa ngôn ngữ",
                IconMenu = "fa-language",

                SubMenu = new AdminMenuItem[]
                {
                    new AdminMenuItem
                    {
                        MenuName = "Thêm ngôn ngữ",

                        ActionName = "Create",
                        ControllerName = "AdminLanguage",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        MenuName = "Chỉnh sửa ngôn ngữ",

                        ActionName = "Index",
                        ControllerName = "AdminLanguage",
                        RouterValues = new { area = "Admin"},

                        SubMenu = new AdminMenuItem[]
                        {
                            new AdminMenuItem
                            {
                                ActionName = "managelanguageresourcevalues",
                                ControllerName = "AdminLanguage",
                            }
                        }
                    },
                    new AdminMenuItem
                    {
                        MenuName = "Import / Export",

                        ActionName = "ImportExport",
                        ControllerName = "AdminLanguage",
                        RouterValues = new { area = "Admin"},
                    },
                }
            },
            new AdminMenuItem{
                MenuName = "Log",
                IconMenu = "fa-file-text-o",

                ActionName = "Index",
                ControllerName = "Log",
                RouterValues = new { area = "Admin"},
            },
        };

        public static AdminMenuItem[] NewsManage = new AdminMenuItem[]
        {
                    new AdminMenuItem
                    {
                        MenuName = "Danh sách bài viết",

                        ActionName = "Index",
                        ControllerName = "AdminTopic",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        MenuName = "Đăng bài",

                        ActionName = "Create",
                        ControllerName = "AdminTopic",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        MenuName = "Quản lý nhóm bài viết",

                        ActionName = "News",
                        ControllerName = "AdminCategory",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        MenuName = "Thêm nhóm bài viết",

                        ActionName = "NewCatNews",
                        ControllerName = "AdminCategory",
                        RouterValues = new { area = "Admin"},
                    },
        };

        public static AdminMenuItem[] ProductManage = new AdminMenuItem[]
        {
                    new AdminMenuItem
                    {
                        MenuName = "Danh sách sản phẩm",

                        ActionName = "Index",
                        ControllerName = "AdminProduct",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        MenuName = "Quản lý loại sản phẩm",

                        ActionName = "Index",
                        ControllerName = "AdminProduct",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        MenuName = "Thêm loại sản phẩm",

                        ActionName = "Create",
                        ControllerName = "AdminProduct",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        MenuName = "Quản lý nhóm sản phẩm",

                        ActionName = "Product",
                        ControllerName = "AdminCategory",
                        RouterValues = new { area = "Admin"},
                    },
                    new AdminMenuItem
                    {
                        MenuName = "Thêm nhóm sản phẩm",

                        ActionName = "NewCatProduct",
                        ControllerName = "AdminCategory",
                        RouterValues = new { area = "Admin"},
                    },
        };

        #region Declare
        public static bool IsActive(this AdminMenuItem menu, string controllerName, string actionName)
        {
            if (!menu.ActionName.IsNullEmpty())
            {
                if (actionName == menu.ActionName.ToLower() && controllerName == menu.ControllerName.ToLower())
                {
                    return true;
                }
            }

            if (menu.SubMenu != null)
            {
                foreach (var it in menu.SubMenu)
                {
                    if (it.IsActive(controllerName, actionName))
                    {
                        return true;
                    }
                }
            }


            return false;
        }

        public static string Content(this UrlHelper Url, AdminMenuItem menu)
        {
            string urlString = "#";
            if (menu.ActionName.IsNullEmpty())
            {
                if (!menu.UrlString.IsNullEmpty())
                {
                    urlString = Url.Content(menu.UrlString);
                }
            }
            else
            {
                urlString = Url.Action(menu.ActionName, menu.ControllerName, menu.RouterValues);
            }

            return urlString;
        }

        public class AdminMenuItem
        {
            public bool isHidden { get; set; }
            public string MenuName { get; set; }
            public string IconMenu { get; set; }
            public AdminMenuItem[] SubMenu { get; set; }

            public string ControllerName { get; set; }
            public string ActionName { get; set; }
            public object RouterValues { get; set; }

            public string UrlString { get; set; }
        }

        #endregion
    }
}