using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMvc.ViewModels
{
    public class FileManagerViewModel
    {
    }

    public class FileManagerForderInfoViewModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public int CountFile { get; set; }
        public int CountFoder { get; set; }
        public bool AllowUpload { get; set; }
    }

    public class FileManagerFileInfoViewModel
    {
        public string p { get; set; }
        public double t { get; set; }
        public long s { get; set; }
        public int w { get; set; }
        public int h { get; set; }
    }
    public class FileManagerSuccessViewModel
    {
        public string res { get; set; }
        public string msg { get; set; }
    }
    
}