using banruou.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace banruou.ViewModel
{
    public class BannerViewModel
    {
        public Banner Banner { get; set; }
        public SelectList SelectGroup { get; set; }
        public BannerViewModel()
        {
            var listgroup = new Dictionary<int, string>
            {
                { 1, "Banner chính" },
                { 2 , "Banner Khuyến mại" },
                { 3, "Lý do lựa chọn" },
                { 4, "Ảnh chính phần Dịch vụ" }
            };
            SelectGroup = new SelectList(listgroup, "Key", "Value");
        }
    }
    public class ListBannerViewModel
    {
        public PagedList.IPagedList<Banner> Banners { get; set; }
        public int? GroupId { get; set; }
        public SelectList SelectGroup { get; set; }
        public ListBannerViewModel()
        {
            var listgroup = new Dictionary<int, string>
            {
                { 1, "Banner video" },
                { 2 , "Banner Khuyến mại" },
                { 3, "Lý do lựa chọn" },
                { 4, "Ảnh chính phần Dịch vụ" }
            };
            SelectGroup = new SelectList(listgroup, "Key", "Value");
        }
    }
}