using banruou.DAL;
using banruou.Models;
using banruou.ViewModel;
using Helpers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;

namespace banruou.Controllers
{
    public class HomeController : Controller
    {
        private static string Email => WebConfigurationManager.AppSettings["email"];
        private static string Password => WebConfigurationManager.AppSettings["password"];

        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public ConfigSite ConfigSite => (ConfigSite)HttpContext.Application["ConfigSite"];
        private IEnumerable<ProductCategory> ProductCategories =>
            _unitOfWork.ProductCategoryRepository.Get(a => a.CategoryActive, q => q.OrderBy(a => a.CategorySort));
        private IEnumerable<ArticleCategory> ArticleCategories =>
            _unitOfWork.ArticleCategoryRepository.Get(a => a.CategoryActive, q => q.OrderBy(a => a.CategorySort));
        public IEnumerable<Banner> Banners => _unitOfWork.BannerRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));

        public ActionResult Index()
        {
            var items = _unitOfWork.ProductCategoryRepository
                .GetQuery(a => a.CategoryActive && a.ShowHome, q => q.OrderBy(a => a.CategorySort)).Select(a =>
                    new HomeViewModel.HomeBlock
                    {
                        Category = a,
                        Products = a.Products.Where(c => c.Active && c.Home).OrderBy(c => c.Sort).Take(10)
                    });


            var model = new HomeViewModel
            {
                Categories = ProductCategories,
                Banners = Banners,
                HomeBlocks = items,
                NewArticles = _unitOfWork.ArticleRepository.Get(a => a.Active && a.Home, q => q.OrderByDescending(a => a.CreateDate), 8)
            };
            return View(model);
        }
        [Route("{url}.html", Order = 1)]
        public ActionResult ProductDetail(string url)
        {
            var product = _unitOfWork.ProductRepository.GetQuery(a => a.Url == url && a.Active).FirstOrDefault();
            if (product == null)
            {
                return RedirectToAction("Index");
            }
            var model = new ProductDetailViewModel
            {
                Product = product,
                Products = _unitOfWork.ProductRepository.GetQuery(p =>
                p.Active && (p.ProductCategoryId == product.ProductCategoryId && p.Id != product.Id), q => q.OrderByDescending(a => a.CreateDate), 12)
            };
            return View(model);
        }
        [Route("san-pham")]
        public ActionResult AllProduct(string url, int? Page)
        {
            var pageNumber = Page ?? 1;
            var category = _unitOfWork.ProductCategoryRepository.GetQuery(a => a.Url == url || a.CategoryActive);
            var product = _unitOfWork.ProductRepository.GetQuery(p => p.Active, o => o.OrderByDescending(p => p.CreateDate));
            var model = new AllProductViewModel
            {
                Products = product.ToPagedList(pageNumber, 12),
                ProductCategories = category,
                ProductCount = product.Count(),
                CountCategory = category.Count(),
            };
            return View(model);
        }
        [Route("{url:regex(^(?!.*(vcms|uploader|article|banner|contact)).*$)}", Order = 2)]
        public ActionResult CategoryProduct(string url, int? page)
        {
            var pageNumber = page ?? 1;
            var category = _unitOfWork.ProductCategoryRepository.Get(a => a.Url == url && a.CategoryActive).FirstOrDefault();
            var product = _unitOfWork.ProductRepository.Get(a => a.ProductCategory.CategoryName == category.CategoryName && a.Active);
            var model = new CategoryProduct
            {
                ProductCategory = category,
                Products = product.ToPagedList(pageNumber, 12),
            };
            return View(model);
        }
        public PartialViewResult GetNavMenu(string url)
        {
            var model = new GetNavMenu
            {
                ArticleCategories = ArticleCategories.Where(a => a.CategoryActive),
            };
            return PartialView(model);
        }
        [ChildActionOnly]
        public PartialViewResult GetNavProduct(int catId = 0)
        {
            var model = new GetNavProduct
            {
                CategoryId = catId,
                ProductCategories = ProductCategories.Where(a => a.CategoryActive),
            };
            return PartialView(model);
        }
        [Route("blogs/{url}", Order = 3)]
        public ActionResult ShowArtilceCategory(string url)
        {
            var category = _unitOfWork.ArticleCategoryRepository.Get(a => a.Url == url && a.CategoryActive).FirstOrDefault();
            var articles = _unitOfWork.ArticleRepository.Get(a => a.ArticleCategory.CategoryName == category.CategoryName && a.Active);
            if (category == null)
            {
                return RedirectToAction("Index");
            }
            var model = new ShowArtilceCategory
            {
                ArticleCategory = category,
                Articles = articles,
            };
            return View(model);
        }
        [Route("blog/{url}", Order = 2)]
        public ActionResult ArticleCategory(int? page, string url)
        {
            var category = _unitOfWork.ArticleCategoryRepository.GetQuery(a => a.CategoryActive && a.Url == url).FirstOrDefault();
            if (category == null)
            {
                return RedirectToAction("Index");
            }
            var articles = _unitOfWork.ArticleRepository.GetQuery(
                a => a.Active && (a.ArticleCategoryId == category.Id || a.ArticleCategory.ParentId == category.Id),
                q => q.OrderByDescending(a => a.CreateDate));
            var pageNumber = page ?? 1;
            if (articles.Count() == 1)
            {
                var fi = articles.First();
                return RedirectToAction("ArticleDetail", new { url = fi.Url });
            }
            var model = new ArticleCategoryViewModel
            {
                Category = category,
                Articles = articles.ToPagedList(pageNumber, 12),
                ArticleCount = articles.Count(),
            };

            if (category.ParentId != null)
            {
                model.RootCategory = _unitOfWork.ArticleCategoryRepository.GetById(category.ParentId);
            }
            return View(model);
        }
        public PartialViewResult GetAllProduct(int? page, int? catId, string url)
        {
            var pageNumber = page ?? 1;
            var product = _unitOfWork.ProductRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate)).AsNoTracking();
            if (catId > 0)
            {
                product = product.Where(p => p.ProductCategoryId == catId);
            }
            var model = new GetAllProductViewModel
            {
                Products = product.ToPagedList(pageNumber, 12),
                CatId = catId,
            };
            return PartialView(model);
        }
        [Route("blogs")]
        public ActionResult AllArtilce(int? page)
        {
            var pageNumber = page ?? 1;
            var artilce = _unitOfWork.ArticleRepository.GetQuery(a => a.Active && a.ArticleCategory.TypePost == TypePost.Article, o => o.OrderByDescending(a => a.CreateDate));
            var model = new AllArticleViewModel
            {
                Articles = artilce.ToPagedList(pageNumber, 12),
                ArticleCount = artilce.Count(),
            };
            return View(model);
        }
        [Route("blog/{url}.html", Order = 1)]
        public ActionResult ArticleDetail(string url)
        {
            var artilce = _unitOfWork.ArticleRepository.GetQuery(a => a.Url == url && a.Active).FirstOrDefault();
            if (artilce == null)
            {
                return RedirectToAction("Index");
            }
            var model = new ArticleDetailsViewModel
            {
                Article = artilce,
                Articles = _unitOfWork.ArticleRepository.Get(p => p.Active && p.ArticleCategoryId == artilce.ArticleCategoryId && p.Id != artilce.Id, q => q.OrderByDescending(a => a.CreateDate), 6)
            };
            return View(model);
        }
        public PartialViewResult Order(string url)
        {
            var product = _unitOfWork.ProductRepository.GetQuery(a => a.Url == url).FirstOrDefault();
            var model = new OrderViewModel
            {
                Products = product,
            };
            return PartialView(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public PartialViewResult Order(OrderViewModel model, string url)
        {
            var product = _unitOfWork.ProductRepository.Get(a => a.Url == url).FirstOrDefault();
            if (product == null)
            {
                return null;
            }
            model.Order.NameProduct = product.Name;
            model.Order.CreateDate = DateTime.Now;
            _unitOfWork.OrderRepository.Insert(model.Order);
            _unitOfWork.Save();
            var body = $"<p>Tên người liên hệ: {model.Order.Name},</p>" +
                       $"<p>Số điện thoại: {model.Order.Phone},</p>" +
                       $"<p>Email: {model.Order.Email},</p>" +
                       $"<p>Nội dung: {model.Order.Discription}</p>" +
                       $"<p>Đây là hệ thống gửi email tự động, vui lòng không phản hồi lại email này.</p>";

            Task.Run(() => HtmlHelpers.SendEmail("gmail", "Email liên hệ từ " + Request.Url?.Host, body,
                ConfigSite.Email, Email, Email, Password));

            TempData["success"] = "Gửi liên hệ thành công ! \nChúng tôi sẽ liên lạc với bạn sớm nhất có thể.";
            return PartialView(model);
        }
        [ChildActionOnly]
        public PartialViewResult Header()
        {
            var model = new HeaderViewModel
            {
                ProductCategories = ProductCategories.Where(a => a.ShowMenu),
                ArticleCategories = ArticleCategories.Where(a => a.CategoryActive && a.ShowMenu),
            };
            return PartialView(model);
        }
        [ChildActionOnly]
        public PartialViewResult Footer()
        {
            var model = new FooterViewModel
            {
                ArticleCategories = ArticleCategories.Where(a => a.ShowFooter && a.TypePost == TypePost.Article),
                Introduce = ArticleCategories.Where(a => a.ShowFooter && a.TypePost == TypePost.Introduce),
                Policies = ArticleCategories.Where(a => a.ShowFooter && a.TypePost == TypePost.Policy),
                FooterProduct = ProductCategories.Where(a => a.ShowFooter)

            };
            return PartialView(model);
        }
        [Route("lien-he")]
        public ActionResult Contact(int result = 0)
        {
            ViewBag.Result = result;
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        [Route("lien-he")]
        public ActionResult Contact(Contact model)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ContactRepository.Insert(model);
                _unitOfWork.Save();

                var subject = "Email liên hệ từ website: " + Request.Url?.Host;
                var body = $"<p>Tên người liên hệ: {model.Fullname},</p>" +
                           $"<p>Số điện thoại: {model.Mobile},</p>" +
                           $"<p>Email: {model.Email},</p>" +
                           $"<p>Địa chỉ: {model.Address},</p>" +
                           $"<p>Nội dung: {model.Body}</p>" +
                           $"<p>Đây là hệ thống gửi email tự động, vui lòng không phản hồi lại email này.</p>";
                Task.Run(() => HtmlHelpers.SendEmail("gmail", subject, body, ConfigSite.Email, Email, Email, Password, Request.Url?.Host));

                return RedirectToAction("Contact", new { result = 1 });
            }
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}