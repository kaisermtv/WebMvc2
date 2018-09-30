using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMvc.Application.Entities;

namespace WebMvc.ViewModels
{
	public class CategoryListViewModel
	{
		public List<Category> AllCategory;
	}

	public class CategoryTopicListViewModel
	{
		public Category Cat;
		public List<Topic> ListTopic;
		public PagingViewModel Paging;
	}

	public class CategoryProductListViewModel
	{
		public ProductClass Group;
		public Category Cat;
		public List<Product> ListProduct;
		public PagingViewModel Paging;
		public List<ProductClass> GroupProducts;
		public List<Guid> GroupSelect;
		public List<CategoryProductFindterViewModel> Attributes;
	}

	public class CategoryProductFindterViewModel {
		public string Name { get; set; }
		public string Title { get; set; }

		public List<CategoryProductFindterOption> Options { get; set; }
	}

	public class CategoryProductFindterOption {
		public string Value { get; set; }
		public string Text { get; set; }
		public bool Check { get; set; }
	}

}