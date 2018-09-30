using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Application.Lib;

namespace WebMvc.Application.Entities
{
    public partial class Language : Entity 
    {
        public Language()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LanguageCulture { get; set; }
        public string FlagImageFileName { get; set; }
        public bool RightToLeft { get; set; }
    }
}
