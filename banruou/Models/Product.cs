using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace banruou.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Display(Name = "Tên sản phẩm", Description = "Tên dự án dài tối đa 150 ký tự"),
         Required(ErrorMessage = "Hãy nhập tên sản phẩm"), StringLength(150, ErrorMessage = "Tối đa 150 ký tự"),
         UIHint("TextBox")]
        public string Name { get; set; }
        [Display(Name = "Ảnh đại diện" ),StringLength(500 )]
        public string Image { get; set; }
        [Display(Name ="Giá")]
        public int Price { get; set; }
        [Display(Name = "Quốc gia"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        public string Country { get; set; }

        [Display(Name="Dung tích"), StringLength(100 , ErrorMessage ="Tối đa 100 ký tự")]
        public string Capacity { get; set; }
        [Display(Name ="Ứng dụng")]
        public string Application { get; set; }
        [Display(Name ="Chất liệu")]
        public string Material { get; set; }
        [Display(Name = "Mô tả chi tiết"), UIHint("EditorBox")]
        public string Body { get; set; }
        [Display(Name = "Ngày đăng")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập số thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Sort { get; set; }
        [Display(Name = "Danh mục sản phẩm"), Required(ErrorMessage = "Hãy chọn danh mục dự án")]
        public int ProductCategoryId { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        [Display(Name = "Bán chạy")]
        public bool Hot { get; set; }
        [Display(Name = "Hiện trang chủ")]
        public bool Home { get; set; }
        [Display(Name = "Đường dẫn"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string Url { get; set; }
        [Display(Name = "Thẻ tiêu đề"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string TitleMeta { get; set; }
        [Display(Name = "Thẻ mô tả"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string DescriptionMeta { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
        public Product()
        {
            CreateDate = DateTime.Now;
            Active = true;
        }
    }
}