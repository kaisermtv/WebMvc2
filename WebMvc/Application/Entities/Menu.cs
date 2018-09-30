using System;
using WebMvc.Application.Lib;

namespace WebMvc.Application.Entities
{
    public partial class Menu :Entity
    {
        public Menu()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }
        public Guid? Menu_Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Colour { get; set; }
        public int iType { get; set; }
        public string Link { get; set; }
        public int SortOrder { get; set; }

        public int Level { get; set; }
    }
}
