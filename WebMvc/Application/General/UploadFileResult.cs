﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMvc.Application.General
{
    public partial class UploadFileResult
    {
        public bool UploadSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public string UploadedFileName { get; set; }
        public string UploadedFileUrl { get; set; }
    }
}