using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Application.Lib;

namespace WebMvc.Application.Entities
{
    public partial class LocaleStringResource : Entity
    {
        public LocaleStringResource()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }
        public string ResourceValue { get; set; }
        public Guid LocaleResourceKey_Id { get; set; }
        public Guid Language_Id { get; set; }

    }
}
