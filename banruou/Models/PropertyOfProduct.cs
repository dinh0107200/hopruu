using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace banruou.Models
{
    public class PropertyOfProduct
    {
        public int Id { get; set; }
        [StringLength(100), Display(Name = "Tên thuộc tính"), Required(ErrorMessage = "Bạn chưa nhập tên thuộc tính"), UIHint("TextBox")]
        public string Name { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập số thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Sort { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }

        public virtual ICollection<GroupPropertyOfProduct> GroupPropertyOfProducts { get; set; }
    }

    public class GroupPropertyOfProduct
    {
        public int Id { get; set; }
        [StringLength(100), Display(Name = "Tên nhóm thuộc tính"), Required(ErrorMessage = "Bạn chưa nhập tên nhóm thuộc tính"), UIHint("TextBox")]
        public string Name { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập số thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Sort { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }

        public virtual ICollection<PropertyOfProduct> PropertyOfProducts { get; set; }
    }

    public class PropertyAndProductDetail
    {
        [Key, Column(Order = 1)]
        public int ProductId { get; set; }
        [Key, Column(Order = 2)]
        public int PropertyOfProductId { get; set; }
        [Display(Name = "Nội dung"), StringLength(500), UIHint("TextBox")]
        public string Name { get; set; }

        public virtual PropertyOfProduct PropertyOfProduct { get; set; }
    }
}