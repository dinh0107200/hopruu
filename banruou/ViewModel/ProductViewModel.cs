using banruou.Models;
using System.Collections.Generic;
using System.Web.Mvc;
namespace banruou.ViewModel
{
    public class ListProjectViewModel
    {
        public PagedList.IPagedList<Product> Products { get; set; }
        public SelectList SelectCategories { get; set; }
        public SelectList ChildCategoryList { get; set; }
        public int? ParentId { get; set; }
        public int? CatId { get; set; }
        public string Name { get; set; }
        public string Sort { get; set; }

        public ListProjectViewModel()
        {
            ChildCategoryList = new SelectList(new List<ProductCategory>(), "Id", "CategoryName");
        }
    }
    public class InsertProjectViewModel
    {
        public Product Product { get; set; }
        //[Display(Name = "Danh mục dự án con"), Required(ErrorMessage = "Hãy chọn danh mục dự án")]
        //public int ParentId { get; set; }
        //[Display(Name = "Danh mục dự án")]
        //public int? CategoryId { get; set; }
        public IEnumerable<ProductCategory> Categories { get; set; }
        public IEnumerable<GroupPropertyOfProduct> GroupPropertyOfProducts { get; set; }

        public InsertProjectViewModel()
        {
            //ChildCategoryList = new SelectList(new List<ProductCategory>(), "Id", "CategoryName");
        }
    }

    public class InsertProductCategoryViewModel
    {
        public ProductCategory Category { get; set; }
        public IEnumerable<GroupPropertyOfProduct> GroupPropertyOfProducts { get; set; }
    }

    public class AddOrUpdateGroupPropertyViewModel
    {
        public GroupPropertyOfProduct GroupPropertyOfProduct { get; set; }
        public IEnumerable<PropertyOfProduct> PropertyOfProducts { get; set; }
    }

}