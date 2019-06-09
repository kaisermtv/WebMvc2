using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMvc.Application;
using WebMvc.Application.Interfaces;
using WebMvc.Services;
using WebMvc.ViewModels;
using WebMvc.Application.Lib;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using WebMvc.Application.Attribute;

namespace WebMvc.Controllers
{
    [Login]
    public class FileManagerController : BaseController
    {
        public FileManagerController(LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, CacheService cacheService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, cacheService, localizationService)
        {

        }

        // GET: FileManager
        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult DirList(string type)
        {
            try
            {
                return Json(GetFolder(type));
            }catch(Exception ex)
            {
                return Json(GetErrorRes(ex.Message));
            }
        }

        public ActionResult FileList(string d, string type)
        {
            try
            {
                return Json(ListFiles(d, type));
            }
            catch (Exception ex)
            {
                return Json(GetErrorRes(ex.Message));
            }
            
        }

        public string GenerateThumb(int width,int height,string f)
        {
            try
            {
                ShowThumbnail(f, width, height);
                return "";
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(GetErrorRes(ex.Message));
            }
            
        }
        
        public ActionResult CopyFile(string f, string n)
        {
            CheckPath(f);
            CheckPath(n);
            FileInfo file = new FileInfo(FixPath(f));
            n = FixPath(n);
            if (!file.Exists)
                return Json(GetErrorRes(LangRes("E_CopyFileInvalisPath")));
            else
            {
                string newName = MakeUniqueFilename(n, file.Name);
                try
                {
                    System.IO.File.Copy(file.FullName, Path.Combine(n, newName));

                    return Json(GetSuccessRes());
                }
                catch (Exception ex)
                {
                    LoggingService.Error(ex.Message);
                    return Json(GetErrorRes(LangRes("E_CopyFile")));
                }
            }
        }

        public ActionResult CopyDir(string d, string n)
        {
            CheckPath(d);
            CheckPath(n);
            DirectoryInfo dir = new DirectoryInfo(FixPath(d));
            DirectoryInfo newDir = new DirectoryInfo(FixPath(n + "/" + dir.Name));

            if (!dir.Exists)
            {
                return Json(GetErrorRes(LangRes("E_CopyDirInvalidPath")));
            }
            else if (newDir.Exists)
            {
                return Json(GetErrorRes(LangRes("E_DirAlreadyExists")));
            }
            else
            {
                _copyDir(dir.FullName, newDir.FullName);
            }
            return Json(GetSuccessRes());
        }

        public ActionResult CreateDir(string d, string n)
        {
            CheckPath(d);
            d = FixPath(d);
            if (!Directory.Exists(d))
                return Json(GetErrorRes(LangRes("E_CreateDirInvalidPath")));
            else if (d == FixPath("/Content/UserFile"))
                return Json(GetErrorRes(LangRes("E_CannotCreateRoot")));
            else
            {
                try
                {
                    d = Path.Combine(d, n);
                    if (!Directory.Exists(d))
                        Directory.CreateDirectory(d);
                }
                catch (Exception ex)
                {
                    LoggingService.Error(ex.Message);
                    return Json(GetErrorRes(LangRes("E_CreateDirFailed")));
                }
            }

            return Json(GetSuccessRes());
        }
        
        public ActionResult DeleteDir(string d)
        {
            CheckPath(d);
            d = FixPath(d);
            if (!Directory.Exists(d))
                return Json(GetErrorRes(LangRes("E_DeleteDirInvalidPath")));
            else if (d == FixPath("/Content/Images"))
                return Json(GetErrorRes(LangRes("E_CannotDeleteRoot")));
            else if (d == FixPath("/Content/UserFile"))
                return Json(GetErrorRes(LangRes("E_CannotDeleteRoot")));
            else if (d == FixPath("/Content/UserFile/" + LoggedOnReadOnlyUser.Id + "/"))
                return Json(GetErrorRes(LangRes("E_CannotDeleteRoot")));
            else if (Directory.GetDirectories(d).Length > 0 || Directory.GetFiles(d).Length > 0)
                return Json(GetErrorRes(LangRes("E_DeleteNonEmpty")));
            else
            {
                try
                {
                    Directory.Delete(d);
                }
                catch (Exception ex)
                {
                    LoggingService.Error(ex.Message);
                    return Json(GetErrorRes(LangRes("E_CannotDeleteDir")));
                }
            }

            return Json(GetSuccessRes());
        }
        
        public ActionResult DeleteFile(string f)
        {
            CheckPath(f);
            f = FixPath(f);
            if (!System.IO.File.Exists(f))
                return Json(GetErrorRes(LangRes("E_DeleteFileInvalidPath")));
            else
            {
                try
                {
                    System.IO.File.Delete(f);
                }
                catch (Exception ex)
                {
                    LoggingService.Error(ex.Message);
                    return Json(GetErrorRes(LangRes("E_DeletеFile")));
                }
            }

            return Json(GetSuccessRes());
        }

        public void Download(string f)
        {
            CheckPath(f);
            try
            {
                FileInfo file = new FileInfo(FixPath(f));
                if (file.Exists)
                {
                    Response.Clear();
                    Response.Headers.Add("Content-Disposition", "attachment; filename=\"" + file.Name + "\"");
                    Response.ContentType = "application/force-download";
                    Response.TransmitFile(file.FullName);
                    Response.Flush();
                    Response.End();
                }
            }
            catch(Exception ex)
            {
                Response.Write(JsonConvert.SerializeObject( GetErrorRes(ex.Message)));
                Response.Flush();
                Response.End();
            }
            
        }

        public void DownloadDir(string d)
        {
            try
            {
                d = FixPath(d);
                if (!Directory.Exists(d))
                    throw new Exception(LangRes("E_CreateArchive"));
                string dirName = new FileInfo(d).Name;
                string tmpZip = Server.MapPath("../tmp/" + dirName + ".zip");
                if (System.IO.File.Exists(tmpZip))
                    System.IO.File.Delete(tmpZip);
                ZipFile.CreateFromDirectory(d, tmpZip, CompressionLevel.Fastest, true);
                Response.Clear();
                Response.Headers.Add("Content-Disposition", "attachment; filename=\"" + dirName + ".zip\"");
                Response.ContentType = "application/force-download";
                Response.TransmitFile(tmpZip);
                Response.Flush();
                System.IO.File.Delete(tmpZip);
                Response.End();
            }
            catch(Exception ex)
            {
                Response.Write(JsonConvert.SerializeObject(GetErrorRes(ex.Message)));
                Response.Flush();
                Response.End();
            }
           
        }
        
        public ActionResult MoveDir(string d,string n)
        {
            CheckPath(d);
            CheckPath(n);
            DirectoryInfo source = new DirectoryInfo(FixPath(d));
            DirectoryInfo dest = new DirectoryInfo(FixPath(Path.Combine(n, source.Name)));
            if (dest.FullName.IndexOf(source.FullName) == 0)
                return Json(GetErrorRes(LangRes("E_CannotMoveDirToChild")));
            else if (!source.Exists)
                return Json(GetErrorRes(LangRes("E_MoveDirInvalisPath")));
            else if (dest.Exists)
                return Json(GetErrorRes(LangRes("E_DirAlreadyExists")));
            else if (n == "/Content/UserFile")
                return Json(GetErrorRes(LangRes("E_CannotMoveRoot")));
            else
            {
                try
                {
                    source.MoveTo(dest.FullName);
                }
                catch (Exception ex)
                {
                    LoggingService.Error(ex.Message);
                    return Json(GetErrorRes(LangRes("E_MoveDir") + " \"" + d + "\""));
                }
            }

            return Json(GetSuccessRes());
        }

        public ActionResult MoveFile(string f, string n)
        {
            CheckPath(f);
            CheckPath(n);
            FileInfo source = new FileInfo(FixPath(f));
            FileInfo dest = new FileInfo(FixPath(n));

            if (!source.Exists)
                return Json(GetErrorRes(LangRes("E_MoveFileInvalisPath")));
            else if (dest.Exists)
                return Json(GetErrorRes(LangRes("E_MoveFileAlreadyExists")));
            else if (!CanHandleFile(dest.Name))
                return Json(GetErrorRes(LangRes("E_FileExtensionForbidden")));
            else
            {
                try
                {
                    source.MoveTo(dest.FullName);
                }
                catch (Exception ex)
                {
                    LoggingService.Error(ex.Message);
                    return Json(GetErrorRes(LangRes("E_MoveFile") + " \"" + f + "\""));
                }
            }

            return Json(GetSuccessRes());
        }

        public ActionResult RenameDir(string d, string n)
        {
            CheckPath(d);
            DirectoryInfo source = new DirectoryInfo(FixPath(d));
            DirectoryInfo dest = new DirectoryInfo(Path.Combine(source.Parent.FullName, n));
            if (d == FixPath("/Content/Images"))
                return Json(GetErrorRes(LangRes("E_CannotRenameRoot")));
            else if (d == FixPath("/Content/UserFile/"))
                return Json(GetErrorRes(LangRes("E_CannotRenameRoot")));
            else if (d == FixPath("/Content/UserFile/" + LoggedOnReadOnlyUser.Id + "/"))
                return Json(GetErrorRes(LangRes("E_CannotRenameRoot")));
            else if (!source.Exists)
                return Json(GetErrorRes(LangRes("E_RenameDirInvalidPath")));
            else if (dest.Exists)
                return Json(GetErrorRes(LangRes("E_DirAlreadyExists")));
            else
            {
                try
                {
                    source.MoveTo(dest.FullName);
                }
                catch (Exception ex)
                {
                    LoggingService.Error(ex.Message);
                    return Json(GetErrorRes(LangRes("E_RenameDir") + " \"" + d + "\""));
                }
            }

            return Json(GetSuccessRes());
        }

        public ActionResult RenameFile(string f, string n)
        {
            CheckPath(f);
            FileInfo source = new FileInfo(FixPath(f));
            FileInfo dest = new FileInfo(Path.Combine(source.Directory.FullName, n));
            if (!source.Exists)
                return Json(GetErrorRes(LangRes("E_RenameFileInvalidPath")));
            else if (!CanHandleFile(n))
                return Json(GetErrorRes(LangRes("E_FileExtensionForbidden")));
            else
            {
                try
                {
                    source.MoveTo(dest.FullName);
                }
                catch (Exception ex)
                {
                    return Json(GetErrorRes(ex.Message + "; " + LangRes("E_RenameFile") + " \"" + f + "\""));
                }
            }

            return Json(GetSuccessRes());
        }

        public ActionResult Upload(string d)
        {
            CheckPath(d);
            d = FixPath(d);
            var res = GetSuccessRes();
            bool hasErrors = false;
            try
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    if (CanHandleFile(Request.Files[i].FileName))
                    {
                        FileInfo f = new FileInfo(Request.Files[i].FileName);
                        string filename = MakeUniqueFilename(d, f.Name);
                        string dest = Path.Combine(d, filename);
                        Request.Files[i].SaveAs(dest);
                        if (GetFileType(new FileInfo(filename).Extension) == "image")
                        {
                            int w = 0;
                            int h = 0;
                            int.TryParse(GetSetting("MAX_IMAGE_WIDTH"), out w);
                            int.TryParse(GetSetting("MAX_IMAGE_HEIGHT"), out h);
                            ImageResize(dest, dest, w, h);
                        }
                    }
                    else
                    {
                        hasErrors = true;
                        res = GetSuccessRes(LangRes("E_UploadNotAll"));
                    }
                }
            }
            catch (Exception ex)
            {
                res = GetErrorRes(ex.Message);
            }
            if (IsAjaxUpload())
            {
                if (hasErrors)
                    res = GetErrorRes(LangRes("E_UploadNotAll"));

                return Json(res);
            }
            else
            {
                return View(JsonConvert.SerializeObject(res));
            }
        }


        #region Private
        protected bool IsAjaxUpload()
        {
            return (Request["method"] != null && Request["method"].ToString() == "ajax");
        }

        protected FileManagerSuccessViewModel GetErrorRes(string msg)
        {
            return new FileManagerSuccessViewModel {
                res = "error",
                msg = msg
            };
        }

        protected void ImageResize(string path, string dest, int width, int height)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            Image img = Image.FromStream(fs);
            fs.Close();
            fs.Dispose();
            float ratio = (float)img.Width / (float)img.Height;
            if ((img.Width <= width && img.Height <= height) || (width == 0 && height == 0))
                return;

            int newWidth = width;
            int newHeight = Convert.ToInt16(Math.Floor((float)newWidth / ratio));
            if ((height > 0 && newHeight > height) || (width == 0))
            {
                newHeight = height;
                newWidth = Convert.ToInt16(Math.Floor((float)newHeight * ratio));
            }
            Bitmap newImg = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage((Image)newImg);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(img, 0, 0, newWidth, newHeight);
            img.Dispose();
            g.Dispose();
            if (dest != "")
            {
                newImg.Save(dest, GetImageFormat(dest));
            }
            newImg.Dispose();
        }

        private ImageFormat GetImageFormat(string filename)
        {
            ImageFormat ret = ImageFormat.Jpeg;
            switch (new FileInfo(filename).Extension.ToLower())
            {
                case ".png": ret = ImageFormat.Png; break;
                case ".gif": ret = ImageFormat.Gif; break;
            }
            return ret;
        }


        protected bool CanHandleFile(string filename)
        {
            bool ret = false;
            FileInfo file = new FileInfo(filename);
            string ext = file.Extension.Replace(".", "").ToLower();
            string setting = GetSetting("FORBIDDEN_UPLOADS").Trim().ToLower();
            if (setting != "")
            {
                ArrayList tmp = new ArrayList();
                tmp.AddRange(Regex.Split(setting, "\\s+"));
                if (!tmp.Contains(ext))
                    ret = true;
            }
            setting = GetSetting("ALLOWED_UPLOADS").Trim().ToLower();
            if (setting != "")
            {
                ArrayList tmp = new ArrayList();
                tmp.AddRange(Regex.Split(setting, "\\s+"));
                if (!tmp.Contains(ext))
                    ret = false;
            }

            return ret;
        }

        protected string MakeUniqueFilename(string dir, string filename)
        {
            string ret = filename;
            int i = 0;
            while (System.IO.File.Exists(Path.Combine(dir, ret)))
            {
                i++;
                ret = Path.GetFileNameWithoutExtension(filename) + " - Copy " + i.ToString() + Path.GetExtension(filename);
            }
            return ret;
        }

        protected FileManagerSuccessViewModel GetSuccessRes()
        {
            return GetSuccessRes("");
        }

        protected FileManagerSuccessViewModel GetSuccessRes(string msg)
        {
            return new FileManagerSuccessViewModel{
                res = "ok",
                msg = msg
            };
        }

        private void _copyDir(string path, string dest)
        {
            if (!Directory.Exists(dest))
                Directory.CreateDirectory(dest);
            foreach (string f in Directory.GetFiles(path))
            {
                FileInfo file = new FileInfo(f);
                if (!System.IO.File.Exists(Path.Combine(dest, file.Name)))
                {
                    System.IO.File.Copy(f, Path.Combine(dest, file.Name));
                }
            }
            foreach (string d in Directory.GetDirectories(path))
            {
                DirectoryInfo dir = new DirectoryInfo(d);
                _copyDir(d, Path.Combine(dest, dir.Name));
            }
        }


        private void ShowThumbnail(string path, int width, int height)
        {
            CheckPath(path);
            FileStream fs = new FileStream(FixPath(path), FileMode.Open, FileAccess.Read);
            Bitmap img = new Bitmap(Bitmap.FromStream(fs));
            fs.Close();
            fs.Dispose();
            int cropWidth = img.Width, cropHeight = img.Height;
            int cropX = 0, cropY = 0;

            double imgRatio = (double)img.Width / (double)img.Height;

            if (height == 0)
                height = Convert.ToInt32(Math.Floor((double)width / imgRatio));

            if (width > img.Width)
                width = img.Width;
            if (height > img.Height)
                height = img.Height;

            double cropRatio = (double)width / (double)height;
            cropWidth = Convert.ToInt32(Math.Floor((double)img.Height * cropRatio));
            cropHeight = Convert.ToInt32(Math.Floor((double)cropWidth / cropRatio));
            if (cropWidth > img.Width)
            {
                cropWidth = img.Width;
                cropHeight = Convert.ToInt32(Math.Floor((double)cropWidth / cropRatio));
            }
            if (cropHeight > img.Height)
            {
                cropHeight = img.Height;
                cropWidth = Convert.ToInt32(Math.Floor((double)cropHeight * cropRatio));
            }
            if (cropWidth < img.Width)
            {
                cropX = Convert.ToInt32(Math.Floor((double)(img.Width - cropWidth) / 2));
            }
            if (cropHeight < img.Height)
            {
                cropY = Convert.ToInt32(Math.Floor((double)(img.Height - cropHeight) / 2));
            }

            Rectangle area = new Rectangle(cropX, cropY, cropWidth, cropHeight);
            Bitmap cropImg = img.Clone(area, System.Drawing.Imaging.PixelFormat.DontCare);
            img.Dispose();
            Image.GetThumbnailImageAbort imgCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
            
            Response.AddHeader("Content-Type", "image/png");
            cropImg.GetThumbnailImage(width, height, imgCallback, IntPtr.Zero).Save(Response.OutputStream, ImageFormat.Png);
            Response.OutputStream.Close();
            cropImg.Dispose();
        }

        public bool ThumbnailCallback()
        {
            return false;
        }

        protected void CheckPath(string path, bool all = false)
        {
            string pth = FixPath(path);

            if (IsRole(AppConstants.AdminRoleName) || all)
            {
                if (pth.IndexOf(FixPath("/Content/UserFile")) == 0)
                {
                    return;
                }

                if (pth.IndexOf(FixPath("/Content/Images")) == 0)
                {
                    return;
                }
            }
            else
            {
                if (pth.IndexOf(FixPath(string.Concat("/Content/UserFile/", LoggedOnReadOnlyUser.Id))) == 0)
                {
                    return;
                }
            }


            throw new Exception("Access to " + path + " is denied");
        }

        private double LinuxTimestamp(DateTime d)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime();
            TimeSpan timeSpan = (d.ToLocalTime() - epoch);

            return timeSpan.TotalSeconds;

        }

        private ArrayList ListFiles(string path, string type)
        {
            CheckPath(path);
            string pth = FixPath(path);

            var lst = new ArrayList();

            List<string> files = GetFiles(pth, type);

            foreach(string it in files)
            {
                FileInfo f = new FileInfo(it);
                int w = 0, h = 0;

                if (GetFileType(f.Extension) == "image")
                {
                    try
                    {
                        FileStream fs = new FileStream(f.FullName, FileMode.Open, FileAccess.Read);
                        Image img = Image.FromStream(fs);
                        w = img.Width;
                        h = img.Height;
                        fs.Close();
                        fs.Dispose();
                        img.Dispose();
                    }
                    catch (Exception ex) { throw ex; }
                }

                lst.Add(new FileManagerFileInfoViewModel
                {
                    p = string.Concat(path,"/",f.Name),
                    t = Math.Ceiling(LinuxTimestamp(f.LastWriteTime)),
                    s = f.Length,
                    h = h,
                    w = w,
                });

            }

            return lst;
        }

        private string GetFileType(string ext)
        {
            string ret = "file";
            ext = ext.ToLower();
            if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif")
                ret = "image";
            else if (ext == ".swf" || ext == ".flv")
                ret = "flash";
            return ret;
        }

        private List<string> GetFiles(string path, string type)
        {
            List<string> ret = new List<string>();
            if (type == "#")
                type = "";
            string[] files = Directory.GetFiles(path);
            foreach (string f in files)
            {
                if ((GetFileType(new FileInfo(f).Extension) == type) || (type == ""))
                    ret.Add(f);
            }
            return ret;
        }

        private string FixPath(string path)
        {
            if (!path.StartsWith("~"))
            {
                if (!path.StartsWith("/"))
                    path = "/" + path;
                path = "~" + path;
            }
            if (path.EndsWith("/")) path = path.Substring(0, path.Length - 1);
            return Server.MapPath(path);
        }

        private ArrayList GetListFolder(string name,string path, string type,bool allowUpload = false)
        {
            var lst = new ArrayList();

            string fixPath = FixPath(path);

            DirectoryInfo d = new DirectoryInfo(fixPath);
            if (!d.Exists)
            {
                d = Directory.CreateDirectory(fixPath);
            }

            string[] dirs = Directory.GetDirectories(fixPath);

            var dirv = new FileManagerForderInfoViewModel {
                Name = name,
                Path = path,
                CountFile = GetFiles(fixPath, type).Count,
                CountFoder = dirs.Length,
                AllowUpload = allowUpload
            };

            lst.Add(dirv);

            string localPath = Server.MapPath("~/");
            localPath = localPath.Substring(0, localPath.Length - 1);

            foreach (var dir in dirs)
            {
                var pth = dir.Replace(localPath, "").Replace("\\", "/");

                lst.AddRange(GetListFolder("",pth, type, allowUpload));
            }

            return lst;
        }

        private ArrayList GetListFolderUser(string path, string type, bool allowUpload = false)
        {
            var lst = new ArrayList();

            string fixPath = FixPath(path);

            DirectoryInfo d = new DirectoryInfo(fixPath);
            if (!d.Exists)
            {
                d = Directory.CreateDirectory(fixPath);
            }

            string[] dirs = Directory.GetDirectories(fixPath);

            var dirv = new FileManagerForderInfoViewModel
            {
                Path = path,
                CountFile = GetFiles(fixPath, type).Count,
                CountFoder = dirs.Length,
                AllowUpload = allowUpload
            };

            lst.Add(dirv);

            string localPath = Server.MapPath("~/");
            localPath = localPath.Substring(0, localPath.Length - 1);

            foreach (var dir in dirs)
            {
                var pth = dir.Replace(localPath, "").Replace("\\", "/");

                var name = pth.Substring(pth.LastIndexOf("/")+1);

                try
                {
                    var guid = new Guid(name);
                    if (LoggedOnReadOnlyUser.Id == guid) continue;

                    var mem = MembershipService.Get(guid);
                    if (mem != null) name = mem.UserName;
                }
                catch { }
                

                lst.AddRange(GetListFolder(name, pth, type, allowUpload));
            }

            return lst;
        }

        private ArrayList GetFolder(string type)
        {
            var lst = new ArrayList();


            lst.AddRange(GetListFolder("My file",string.Concat("/Content/UserFile/", LoginUser.Id), type, true));


            bool uploadRoler = false;

            if (IsRole(AppConstants.AdminRoleName))
            {
                uploadRoler = true;
            }
            lst.AddRange(GetListFolder("","/Content/Images", type, uploadRoler));

            if (IsRole(AppConstants.AdminRoleName))
            {
                lst.AddRange(GetListFolderUser("/Content/UserFile", type, true));
            }

            return lst;
        }
        #endregion

        #region Private static
        private static JsonConfig cacheLang;

        private static string LangRes(string name)
        {
            if(cacheLang == null)
            {
                cacheLang = new JsonConfig("~/Scripts/fileman/lang/vi.json");
            }

            string ret = (string)cacheLang.GetValue(name);

            if (ret.IsNullEmpty()) return name;
            return ret;
        }

        private static JsonConfig cacheSetting;

        private static string GetSetting(string name)
        {
            if (cacheSetting == null)
            {
                cacheSetting = new JsonConfig("~/Scripts/fileman/conf.json");
            }

            return (string)cacheSetting.GetValue(name);
        }
        
        #endregion
    }
}