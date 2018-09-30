﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Application.Lib;

namespace WebMvc.Application.Entities
{
    public partial class Category : Entity
    {
        public Category()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsLocked { get; set; }
        public bool? ModerateTopics { get; set; }
        public bool? ModeratePosts { get; set; }
        public int SortOrder { get; set; }
        public DateTime DateCreated { get; set; }
        public string Slug { get; set; }
        public string PageTitle { get; set; }
        public string Path { get; set; }
        public string MetaDescription { get; set; }
        public string Colour { get; set; }
        public string Image { get; set; }

        public bool IsProduct { get; set; }

        public Guid? Category_Id { get; set; }

        public int Level { get; set; }
    }
}
