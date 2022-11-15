using System;
using System.ComponentModel.DataAnnotations;

namespace banruou.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Display(Name = "Họ và tên"), UIHint("TextBox"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        public string Fullname { get; set; }

        [Display(Name = "Địa chỉ"), UIHint("TextBox"), StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Address { get; set; }

        [Display(Name = "Số điện thoại"), Required(ErrorMessage = "Bạn chưa nhập SĐT"), StringLength(20, ErrorMessage = "Tối đa 20 ký tự"), UIHint("TextBox")]
        public string Mobile { get; set; }

        [Display(Name = "Email"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), EmailAddress(ErrorMessage = "Email không hợp lệ"), UIHint("TextBox")]
        public string Email { get; set; }

        [Display(Name = "Nội dung"), DataType(DataType.MultilineText), StringLength(4000)]
        public string Body { get; set; }

        public DateTime CreateDate { get; set; }

        public Contact()
        {
            CreateDate = DateTime.Now;
        }
    }
}