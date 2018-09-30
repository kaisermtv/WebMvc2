using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using WebMvc.Services;

namespace WebMvc.Application.Annotation
{
    public class WebMvcResourceDisplayName : DisplayNameAttribute
    {
        private string _resourceValue = string.Empty;
        private readonly LocalizationService _localizationService;

        public WebMvcResourceDisplayName(string resourceKey)
            : base(resourceKey)
        {
            ResourceKey = resourceKey;
            _localizationService = ServiceFactory.Get<LocalizationService>();
        }

        public string ResourceKey { get; set; }

        public override string DisplayName
        {
            get
            {
                _resourceValue = _localizationService.GetResourceString(ResourceKey.Trim());

                return _resourceValue;
            }
        }

        public string Name
        {
            get { return "WMVCResourceDisplayName"; }
        }
    }
}