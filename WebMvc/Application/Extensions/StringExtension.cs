using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMvc.Application.Extensions
{
    public static class StringExtension
    {
        public static bool ToBool(this string text)
        {
            try
            {
                return bool.Parse(text);
            }
            catch
            {
                return false;
            }
        }
    }
}