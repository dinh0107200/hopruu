using System.ComponentModel.DataAnnotations;

namespace banruou.Models
{
    public class ConfigSite
    {
        public int Id { get; set; }
        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn Facebook"),
         Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string Facebook { get; set; }
        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn Youtube"),
         Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string Youtube { get; set; }
        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn Twitter"),
        Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string Twitter { get; set; }
        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn Instagram"),
        Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string Instagram { get; set; }
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), Display(Name = "Đường dẫn Zalo"), UIHint("TextBox")]
        public string Zalo { get; set; }
        [StringLength(4000, ErrorMessage = "Tối đa 4000 ký tự"), Display(Name = "Mã nhúng Live chat"),
        UIHint("TextArea")]
        public string LiveChat { get; set; }
        [Display(Name = "Logo"), StringLength(500)]
        public string Image { get; set; }
        [Display(Name = "Favicon"), StringLength(500)]
        public string Favicon { get; set; }
        [StringLength(4000, ErrorMessage = "Tối đa 4000 ký tự"), Display(Name = "Mã Google Map"), UIHint("TextArea"), DataType(DataType.MultilineText)]
        public string GoogleMap { get; set; }
        [StringLength(4000, ErrorMessage = "Tối đa 4000 ký tự"), Display(Name = "Mã Google Analytics"), UIHint("TextArea"), DataType(DataType.MultilineText)]
        public string GoogleAnalytics { get; set; }
        [Display(Name = "Thẻ title"), StringLength(200, ErrorMessage = "Tối đa 200 ký tự"), UIHint("TextBox")]
        public string Title { get; set; }
        [Display(Name = "Thẻ description"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea"), DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Display(Name = "Ảnh giới thiệu"), StringLength(500)]
        public string AboutImage { get; set; }
        [Display(Name = "Thông tin bài giới thiệu"), UIHint("EditorBox"), DataType(DataType.MultilineText)]
        public string AboutText { get; set; }
        [Display(Name = "Đường dẫn Bài giới thiệu"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextBox")]
        public string AboutUrl { get; set; }
        [Display(Name = "Hotline"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), UIHint("TextBox")]
        public string Hotline { get; set; }
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), Display(Name = "Email"), UIHint("TextBox")]
        public string Email { get; set; }
        [Display(Name = "Thông tin liên hệ"), UIHint("EditorBox"),]
        public string InfoContact { get; set; }
        [Display(Name = "Thông tin chân trang"), UIHint("EditorBox"), DataType(DataType.MultilineText)]
        public string InfoFooter { get; set; }
        [Display(Name = "Hotline 2"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), UIHint("TextBox")]
        public string Hotline2 { get; set; }
        [Display(Name = "Hotline 3"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), UIHint("TextBox")]
        public string Hotline3 { get; set; }
        [Display(Name = "Hotline 4"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), UIHint("TextBox")]
        public string Hotline4 { get; set; }
        [Display(Name = "Địa chỉ"), StringLength(500, ErrorMessage = "Tối đa 50 ký tự"), UIHint("TextBox")]
        public string Address { get; set; }
        [Display(Name = "Thông tin CSKH"), UIHint("EditorBox")]
        public string ProductNote { get; set; }
        [Display(Name = "Thông tin liên hệ bài viết"), UIHint("EditorBox")]
        public string Introduce { get; set; }
    }
}