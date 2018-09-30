using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMvc.Application.Lib;

namespace WebMvc.Application.Entities
{
    public partial class Carousel : Entity
    {
        public Carousel()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }
        public Guid? Carousel_Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public int SortOrder { get; set; }

        public int Level { get; set; }
    }
}