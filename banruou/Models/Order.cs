using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace banruou.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Display(Name = "Họ và tên"), Required(ErrorMessage = "Chưa nhập họ tên"), StringLength(60, ErrorMessage = "Nhập tối đa 60 ký tự"), UIHint("TextBox")]
        public string Name { get; set; }
        [Display(Name = "Địa chỉ"), Required(ErrorMessage = "Chưa nhập địa chỉ"), StringLength(200, ErrorMessage = "Nhập tối đa 200 ký tự")]
        public string Address {get; set;}

        [Display(Name = "Số điện thoại"), RegularExpression(@"^\(?(09|03|07|08|05)\)?[-. ]?([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng!"),
         Required(ErrorMessage = "Hãy nhập số điện thoại"), StringLength(10, ErrorMessage = "Tối đa 20 ký tự"), UIHint("TextBox")]
        public string Phone { get; set; }
        [Display(Name = "Email"), Required(ErrorMessage = "Hãy nhập Email"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), EmailAddress(ErrorMessage = "Email không hợp lệ"), UIHint("TextBox")]
        public string Email { get; set; }
        [Display(Name = "Nội dung"), DataType(DataType.MultilineText), StringLength(4000)]
        public string Discription { get; set; }
        public string NameProduct { get; set; }
        public DateTime CreateDate { get; set; }
    }
}