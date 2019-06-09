using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using WebMvc.Application;
using WebMvc.Application.Attribute;
using WebMvc.Application.Context;
using WebMvc.Application.Entities;
using WebMvc.Application.Interfaces;
using WebMvc.Application.Lib;
using WebMvc.Areas.Admin.ViewModels;
using WebMvc.Services;

namespace WebMvc.Areas.Admin.Controllers
{
    public class AdminMembersController : BaseAdminController
    {
        protected readonly PermissionService _permissionService;
        protected readonly RoleSevice _roleSevice;

        public AdminMembersController(RoleSevice roleSevice,PermissionService permissionService,LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {
            _permissionService = permissionService;
            _roleSevice = roleSevice;
        }

        // GET: Admin/AdminMembership
        public ActionResult Index(int? p)
        {
            int limit = 10;
            var count = MembershipService.GetCount();

            var Paging = CalcPaging(limit, p, count);

            var viewModel = new AdminListMembersViewModel
            {
                Paging = Paging,
                ListMembers = MembershipService.GetList(limit, Paging.Page)
            };

            return View(viewModel);
        }

        #region Create Accout

        public ActionResult Create()
        {
            var viewModel = new AdminCreateMemberViewModel
            {
                Roles = GetRolesEdit(),
            };

            // See if a return url is present or not and add it
            var returnUrl = Request["ReturnUrl"];
            if (!string.IsNullOrEmpty(returnUrl))
            {
                viewModel.ReturnUrl = returnUrl;
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdminCreateMemberViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Check User exits
                if (MembershipService.GetUser(viewModel.UserName) != null)
                {
                    ModelState.AddModelError("UserName", "Tên tài khoản đã tồn tại! Hãy chọn tên khác");
                    return View(viewModel);
                }

                // Check password is match
                if (viewModel.Password != viewModel.Password2)
                {
                    ModelState.AddModelError("Password2", "Nhập lại mật khẩu mới không khớp");
                    return View(viewModel);
                }

                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var user = new MembershipUser
                        {
                            UserName = viewModel.UserName,
                            Password = viewModel.Password,
                            Email = viewModel.Email,
                            Avatar = viewModel.Avatar,
                            IsApproved = true,
                        };

                        //var Roles = GetRolesEdit();


                        var createStatus = MembershipService.NewUser(user);
                        if (createStatus != MembershipCreateStatus.Success)
                        {
                            ModelState.AddModelError(string.Empty, MembershipService.ErrorCodeToString(createStatus));
                            return View(viewModel);
                        }

                        if (viewModel.Roles != null)
                        {
                            foreach (var it in viewModel.Roles)
                            {
                                if (!it.Check) continue;

                                it.RoleName = _roleSevice.GetRoleNameById(it.RoleId);
                                if (it.RoleName == AppConstants.AdminRoleName && !LoginRequest.IsSuperAccount()) continue;
                                if (it.RoleName == AppConstants.GuestRoleName || it.RoleName == AppConstants.StandardRoleName) continue;


                                MembershipService.AddRoleByUser(user.Id,it.RoleId);
                            }
                        }
                        

                        unitOfWork.Commit();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Tạo mới tài khoản thành công."),
                            MessageType = GenericMessages.success
                        };

                        if (Url.IsLocalUrl(viewModel.ReturnUrl) && viewModel.ReturnUrl.Length > 1 && viewModel.ReturnUrl.StartsWith("/")
                            && !viewModel.ReturnUrl.StartsWith("//") && !viewModel.ReturnUrl.StartsWith("/\\"))
                        {
                            return Redirect(viewModel.ReturnUrl);
                        }
                        return RedirectToAction("Index");


                    }
                    catch(Exception ex)
                    {
                        LoggingService.Error(ex);
                        unitOfWork.Rollback();

                        ModelState.AddModelError("", "Có lỗi không mong muốn xảy ra! Xin thử lại.");
                    }
                }



            }
           
            return View(viewModel);
        }
        #endregion

        #region Edit info Accout
        public ActionResult Edit(Guid Id)
        {
            var user = MembershipService.Get(Id, true);
            if (LoginUser.UserName == user.UserName) return RedirectToAction("ChangeInfo");

            if (!LoginRequest.IsSuperAccount())
            {
                if (MembershipService.UserInRole(user, AppConstants.AdminRoleName))
                {
                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "Bạn không có quyện hạn thực hiện thao tác này!",
                        MessageType = GenericMessages.warning
                    };

                    return RedirectToAction("Index");
                }
            }

            AdminEditMemberViewModel viewModel = new AdminEditMemberViewModel {
                Id = Id,
                UserName = user.UserName,
                Email = user.Email,
                Avatar = user.Avatar,

                Roles = GetRolesEdit(user),
            };


            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminEditMemberViewModel viewModel)
        {
            var user = MembershipService.Get(viewModel.Id, true);
            if (LoginUser.UserName == user.UserName) return RedirectToAction("ChangeInfo");

            if (!LoginRequest.IsSuperAccount())
            {
                if (MembershipService.UserInRole(user, AppConstants.AdminRoleName))
                {
                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "Bạn không có quyện hạn thực hiện thao tác này!",
                        MessageType = GenericMessages.warning
                    };

                    return RedirectToAction("Index");
                }
            }

            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        user.Email = viewModel.Email;
                        user.Avatar = viewModel.Avatar;

                        MembershipService.ClearRolesByUser(user);
                        if (viewModel.Roles != null)
                        {
                            foreach (var it in viewModel.Roles)
                            {
                                if (!it.Check) continue;
                                it.RoleName = _roleSevice.GetRoleNameById(it.RoleId);
                                if (it.RoleName == AppConstants.AdminRoleName && !LoginRequest.IsSuperAccount()) continue;
                                if (it.RoleName == AppConstants.GuestRoleName || it.RoleName == AppConstants.StandardRoleName) continue;


                                MembershipService.AddRoleByUser(user.Id, it.RoleId);
                            }
                        }


                        unitOfWork.Commit();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Cập nhật tài khoản thành công."),
                            MessageType = GenericMessages.success
                        };
                        

                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error(ex);
                        unitOfWork.Rollback();

                        ModelState.AddModelError("", "Có lỗi không mong muốn xảy ra! Xin thử lại.");
                    }
                }



            }

            viewModel.UserName = user.UserName;
            return View(viewModel);
        }
        #endregion

        #region New pass
        public ActionResult NewPass(Guid id)
        {
            if (LoginUser.Id == id) return RedirectToAction("ChangePass");
            var user = MembershipService.Get(id,true);
            if(user == null) return RedirectToAction("index");
            if(user.UserName == AppConstants.AdminUsername || (LoginUser.UserName != AppConstants.AdminUsername && MembershipService.UserInRole(user, AppConstants.AdminRoleName)))
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Bạn không đủ quyền hạn để thực hiện thao tác này!",
                    MessageType = GenericMessages.warning
                };

                return RedirectToAction("index");
            }

            var viewModel = new AdminNewPassViewModel
            {
                Id = id,
                UserName = user.UserName,
            };


            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPass(AdminNewPassViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (LoginUser.Id == viewModel.Id) return RedirectToAction("ChangePass");
                var user = MembershipService.Get(viewModel.Id, true);
                if (user == null) return RedirectToAction("index");
                if (user.UserName == AppConstants.AdminUsername || (LoginUser.UserName != AppConstants.AdminUsername && MembershipService.UserInRole(user,AppConstants.AdminRoleName)) )
                {
                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "Bạn không đủ quyền hạn để thực hiện thao tác này!",
                        MessageType = GenericMessages.warning
                    };

                    return RedirectToAction("index");
                }

                // Check password is match
                if (viewModel.NewPassword != viewModel.NewPassword2)
                {
                    ModelState.AddModelError("NewPassword2", "Nhập lại mật khẩu mới không khớp");
                    return View(viewModel);
                }

                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var salt = user.PasswordSalt;
                        var newhash = StringUtils.GenerateSaltedHash(viewModel.NewPassword, salt);

                        user.Password = newhash;
                        user.LastPasswordChangedDate = DateTime.UtcNow;
                        MembershipService.Update(user);

                        unitOfWork.Commit();
                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Đổi mật khẩu thành công!",
                            MessageType = GenericMessages.success
                        };
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError("", "Có lỗi xảy ra khi đổi mật khẩu!");
                    }

                }
                    

            }
            
            return View(viewModel);
        }
        #endregion

        #region change info
        public ActionResult ChangeInfo()
        {
            var viewModel = new AdminChangeInfo
            {
                UserName = LoginUser.UserName,
                Email = LoginUser.Email,
            };


            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeInfo(AdminChangeInfo viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var user = MembershipService.GetUser(LoginUser.UserName);
                        user.Email = viewModel.Email;


                        MembershipService.Update(user);

                        unitOfWork.Commit();
                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Đổi thông tin thành công!",
                            MessageType = GenericMessages.success
                        };

                        return RedirectToAction("index", "Admin");
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError("", "Có lỗi xảy ra khi đổi thông tin tài khoản!");
                    }
                }
            }

            return View(viewModel);
        }
        #endregion

        #region change pass
        public ActionResult ChangePass()
        {
            var viewModel = new AdminChangePass
            {
                UserName = LoginUser.UserName,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePass(AdminChangePass viewModel)
        {
            if (ModelState.IsValid)
            {
                // Check password is match
                if (viewModel.NewPassword != viewModel.NewPassword2)
                {
                    ModelState.AddModelError("NewPassword2", "Nhập lại mật khẩu mới không khớp");
                    return View(viewModel);
                }

                if (viewModel.OldPassword == viewModel.NewPassword)
                {
                    ModelState.AddModelError("NewPassword", "Mật khẩu mới không được trùng với mật khẩu cũ");
                    return View(viewModel);
                }

                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var ischange = MembershipService.ChangePassword(LoginUser.UserName, viewModel.OldPassword, viewModel.NewPassword);
                        if (!ischange)
                        {
                            // Rollback transaction 
                            unitOfWork.Rollback();

                            ModelState.AddModelError("OldPassword", "Sai mật khẩu");
                            return View(viewModel);
                        }

                        unitOfWork.Commit();
                        // We use temp data because we are doing a redirect
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Đổi mật khẩu thành công!",
                            MessageType = GenericMessages.success
                        };

                        return RedirectToAction("index", "Admin");
                    }
                    catch(Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError("", "Có lỗi xảy ra khi đổi mật khẩu!");
                    }
                }

            }

                return View(viewModel);
        }
        #endregion

        #region Login & Logout
        public ActionResult Logout()
        {
            LoginRequest.LogOut();

            return RedirectToAction("Login");
        }

        [AdminNotLogin]
        public ActionResult Login(string ReturnUrl = "")
        {
            AdminLogOnViewModel viewModel = new AdminLogOnViewModel
            {
                ReturnUrl = ReturnUrl
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminNotLogin]
        public ActionResult Login(AdminLogOnViewModel model)
        {
            if (ModelState.IsValid)
            {
                switch (LoginRequest.ValidateUser(model.UserName, model.Password,false,Application.Login.TypeLogin.AdminLogin))
                {
                    case LoginAttemptStatus.LoginSuccessful:
                        if (Url.IsLocalUrl(model.ReturnUrl) && model.ReturnUrl.Length > 1 && model.ReturnUrl.StartsWith("/")
                                        && !model.ReturnUrl.StartsWith("//") && !model.ReturnUrl.StartsWith("/\\"))
                        {
                            return Redirect(model.ReturnUrl);
                        }

                        return RedirectToAction("Index", "Admin");
                    case LoginAttemptStatus.UserNotFound:
                    case LoginAttemptStatus.PasswordIncorrect:
                        ModelState.AddModelError(string.Empty,"Tài khoản hoặc mật khẩu không chính xác!");
                        break;

                    case LoginAttemptStatus.PasswordAttemptsExceeded:
                        ModelState.AddModelError("Password","Vượt quá số lần thử mật khẩu!");
                        break;

                    case LoginAttemptStatus.UserLockedOut:
                        ModelState.AddModelError("UserName", "Tài khoản này đã bị khóa!");
                        break;

                    case LoginAttemptStatus.Banned:
                        ModelState.AddModelError("UserName", "Tài khoản bị cấm!");
                        break;

                    case LoginAttemptStatus.UserNotApproved:
                        ModelState.AddModelError("UserName", "Tài khoản người dùng chưa được kích hoạt!");
                        //user = MembershipService.GetUser(username);
                        //SendEmailConfirmationEmail(user);
                        break;

                    default:
                        ModelState.AddModelError(string.Empty,"Lỗi đăng nhập không xác định!");
                        break;
                }
            }
            return View(model);
        }
        #endregion

        #region Roles
        public ActionResult Roles(int? p)
        {
            int limit = AppConstants.AdminNumberInPage;

            var count = MembershipService.GetCount();

            var Paging = CalcPaging(limit, p, count);

            var viewModel = new AdminListRolesViewModel
            {
                Paging = Paging,
                ListRoles = MembershipService.GetAllRoles(limit, Paging.Page)
            };

            return View(viewModel);
        }

        private List<AdminEditRolePermissionsViewModel> GetListRolePermissions(MembershipRole Role = null)
        {
            var allPermissions = _permissionService.GetAll();
            List<Permission> SelectPermissions = null;
            if(Role != null)
            {
                SelectPermissions = _permissionService.GetPermissions(Role);
            }

            var Permissions = new List<AdminEditRolePermissionsViewModel>();

            foreach (var it in allPermissions)
            {
                var rl = new AdminEditRolePermissionsViewModel {
                    Id = it.Id,
                    PermissionName = it.Name,
                    PermissionId = it.PermissionId
                };

                if(SelectPermissions != null)
                {
                    foreach (var itt in SelectPermissions)
                    {
                        if(itt.Id == it.Id)
                        {
                            rl.Check = true;
                            break;
                        }
                    }
                }


                Permissions.Add(rl);
            }

            return Permissions;
        }

        public ActionResult AddRole()
        {
            var viewModel = new AdminEditRoleViewModel
            {
                AllPermissions = GetListRolePermissions()
            };


            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRole(AdminEditRoleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var role = new MembershipRole
                        {
                            RoleName = viewModel.RoleName,
                        };
                        
                        MembershipService.Add(role);

                        //_permissionService.ClearPermissionsInRole(role);

                        if(viewModel.AllPermissions != null)
                        {
                            foreach (var it in viewModel.AllPermissions)
                            {
                                if (it.Check && it.Id != null)
                                {
                                    _permissionService.AddPermissionInRole((Guid)it.Id, role.Id);
                                }
                            }
                        }
                        
                        unitOfWork.Commit();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Thêm chức vụ thành công.",
                            MessageType = GenericMessages.success
                        };
                        return RedirectToAction("Roles");
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error(ex.Message);
                        unitOfWork.Rollback();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Có lỗi xảy ra khi thêm chức vụ!",
                            MessageType = GenericMessages.warning
                        };
                    }
                }
                
            }

            return View(viewModel);
        }

        public ActionResult EditRole(Guid id)
        {
            var role = MembershipService.GetRole(id);
            if (role == null) return RedirectToAction("Roles");

            var model = new AdminEditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.RoleName,
                Lock = role.Lock,
                AllPermissions = GetListRolePermissions(role)
            };


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRole(AdminEditRoleViewModel viewModel)
        {
            var role = MembershipService.GetRole(viewModel.Id, false);
            if (role == null) return RedirectToAction("Roles");

            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        if (!role.Lock)
                        {
                            role.RoleName = viewModel.RoleName;
                            MembershipService.Update(role);
                        }
                        
                        _permissionService.ClearPermissionsInRole(role);

                        if (viewModel.AllPermissions != null)
                        {
                            foreach (var it in viewModel.AllPermissions)
                            {
                                if (it.Check && it.Id != null)
                                {
                                    _permissionService.AddPermissionInRole((Guid)it.Id, role.Id);
                                }
                            }
                        }

                        unitOfWork.Commit();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Sửa thông tin chức vụ thành công.",
                            MessageType = GenericMessages.success
                        };
                        //return RedirectToAction("Roles");
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error(ex.Message);
                        unitOfWork.Rollback();

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Có lỗi xảy ra khi sưa thông tin chức vụ!",
                            MessageType = GenericMessages.warning
                        };
                    }
                }

            }

            return View(viewModel);
        }
        #endregion

        #region Private Function
        private List<AdminEditUserRoleViewModel> GetRolesEdit(MembershipUser user = null)
        {
            var roles = new List<AdminEditUserRoleViewModel>();

            var allrole = MembershipService.GetAllRoles();
            List<MembershipRole> userrole;
            if (user != null)
            {
                userrole = MembershipService.GetRoles(user);
            } else
            {
                userrole = new List<MembershipRole>();
            }

            foreach (var it in allrole)
            {
                if (it.RoleName == AppConstants.AdminRoleName && !LoginRequest.IsSuperAccount()) continue;
                if (it.RoleName == AppConstants.GuestRoleName || it.RoleName == AppConstants.StandardRoleName) continue;

                var check = false;
                if(user != null)
                {
                    foreach (var rl in userrole)
                    {
                        if(it.Id == rl.Id)
                        {
                            check = true;
                            break;
                        }
                    }
                }


                roles.Add(new AdminEditUserRoleViewModel
                {
                    Check = check,
                    RoleId = it.Id,
                    RoleName = it.RoleName,
                });
            }



            return roles;
        }
        #endregion
    }
}