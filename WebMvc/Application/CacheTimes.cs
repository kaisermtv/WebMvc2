using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMvc.Application
{
    public enum CacheTimes
    {
        OneMinute = 1,
        OneHour = 60,
        TwoHours = 120,
        SixHours = 360,
        TwelveHours = 720,
        OneDay = 1440
    }
}