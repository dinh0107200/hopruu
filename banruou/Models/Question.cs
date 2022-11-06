using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace banruou.Models
{
    public class Question
    {
        public int Id { get; set; }
        [Display(Name = "Câu hỏi", Description = "Câu hỏi dài tối đa 150 ký tự"),
         Required(ErrorMessage = "Hãy nhập câu hỏi"), StringLength(150, ErrorMessage = "Tối đa 150 ký tự"),
         UIHint("TextBox")]
        public string QuestionName { get; set; }
        [Display(Name = "Đáp"), UIHint("EditorBox")]
        public string Body { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập số thứ tự"),
         RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Sort { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        [StringLength(300)]
        public string Url { get; set; }
        public Question()
        {
            Active = true;
        }
    }
}