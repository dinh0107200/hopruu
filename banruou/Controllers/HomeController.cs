using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using banruou.Models;
using banruou.ViewModel;
using banruou.DAL;
using PagedList;
using System.Data.Entity;
using System.Web.Configuration;
using System.Threading.Tasks;
using Helpers;
using System.Net.Mail;
using System.Net;

namespace banruou.Controllers
{
    public class HomeController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public ConfigSite ConfigSite => (ConfigSite)HttpContext.Application["ConfigSite"];
        private IEnumerable<ProductCategory> ProductCategories =>
            _unitOfWork.ProductCategoryRepository.Get(a => a.CategoryActive, q => q.OrderBy(a => a.CategorySort));
        private IEnumerable<ArticleCategory> ArticleCategories => 
            _unitOfWork.ArticleCategoryRepository.Get(a => a.CategoryActive , q => q.OrderBy(a => a.CategorySort));
        private static string Email => WebConfigurationManager.AppSettings["email"];
        private static string Password => WebConfigurationManager.AppSettings["password"];

        public ActionResult Index()
        {
            var model = new HomeViewModel
            {
                Banners = _unitOfWork.BannerRepository.GetQuery(a => a.Active),
                Articles = _unitOfWork.ArticleRepository.GetQuery(a => a.Active && a.ArticleCategory.CategoryName == "Khuyến mại"),
                Products = _unitOfWork.ProductRepository.GetQuery( a => a.Active),
                NewArticles = _unitOfWork.ArticleRepository.GetQuery(a => a.Active && a.Home == true),
            };
            return View(model);
        }
        [Route("Chi-tiet/{url}.html")]
        public ActionResult ProductDetail(string url)
        {
            var product = _unitOfWork.ProductRepository.GetQuery( a => a.Url == url && a.Active).FirstOrDefault();
            if(product == null)
            {
                return RedirectToAction("Index");
            }
            var model = new ProductDetailViewModel
            {
                Product = product,
                Products = _unitOfWork.ProductRepository.GetQuery(p =>
                p.Active && (p.ProductCategoryId == product.ProductCategoryId && p.Id != product.Id), q => q.OrderByDescending(a => a.CreateDate), 6),
            };
            return View(model);
        }
        [Route("Danh-sach-san-pham")]
        public ActionResult AllProduct(string url , int? Page )
        {
            var pageNumber = Page ?? 1;
            var category = _unitOfWork.ProductCategoryRepository.GetQuery(a => a.Url == url || a.CategoryActive );
            var product = _unitOfWork.ProductRepository.GetQuery(p => p.Active , o => o.OrderByDescending(p => p.CreateDate));
            var model = new AllProductViewModel
            {
                Products = product.ToPagedList(pageNumber , 12),
                ProductCategories = category,
                ProductCount = product.Count(),
                CountCategory = category.Count(),
            };
            return View(model);
        }
        [Route("danh-muc-san-pham/{url}")]
        public ActionResult CategoryProduct(string url , int? page)
        {
            var pageNumber = page ?? 1;
            var category = _unitOfWork.ProductCategoryRepository.Get(a => a.Url == url && a.CategoryActive).FirstOrDefault();
            var product = _unitOfWork.ProductRepository.Get(a => a.ProductCategory.CategoryName == category.CategoryName && a.Active);
            var model = new CategoryProduct
            {
                ProductCategory = category ,
                Products = product.ToPagedList(pageNumber , 6),
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
        public PartialViewResult GetNavProduct()
        {
            var model = new GetNavProduct
            {
                ProductCategories = ProductCategories.Where(a => a.CategoryActive),
            };
            return PartialView(model);
        }
        [Route("danh-muc-bai-viet/{url}")]
        public ActionResult ShowArtilceCategory (string url)
        {
            var category = _unitOfWork.ArticleCategoryRepository.Get(a => a.Url == url && a.CategoryActive).FirstOrDefault();
            var articles = _unitOfWork.ArticleRepository.Get(a => a.ArticleCategory.CategoryName == category.CategoryName && a.Active);
            if(category == null)
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
        [Route("Blog/{url}")]
        public ActionResult ArticleCategory (int? page , string url)
        {
            var category = _unitOfWork.ArticleCategoryRepository.GetQuery(a => a.CategoryActive && a.Url == url).FirstOrDefault();
            if(category == null)
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
                return RedirectToAction("ArtilceDetail", new { url = fi.Url });
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
        public PartialViewResult GetAllProduct(int? page, int? catId , string url)
        {
            var pageNumber = page ?? 1;
            var product = _unitOfWork.ProductRepository.GetQuery(a => a.Active , o => o.OrderByDescending(a => a.CreateDate)).AsNoTracking();
            if(catId > 0)
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
        [Route("Blog")]
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
        [Route("chi-tiet-bai-viet/{url}")]
        public ActionResult ArtilceDetail(string url)
        {
            var artilce = _unitOfWork.ArticleRepository.GetQuery(a => a.Url == url && a.Active).FirstOrDefault();
            if(artilce == null)
            {
                return RedirectToAction("Index");
            }
            var model = new ArticleDetailsViewModel
            {
                Article = artilce,
                Articles = _unitOfWork.ArticleRepository.GetQuery(p =>
                p.Active && (p.ArticleCategoryId == artilce.ArticleCategoryId && p.Id != artilce.Id), q => q.OrderByDescending(a => a.CreateDate), 6),
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
        [HttpPost , ValidateAntiForgeryToken]
        public PartialViewResult Order(OrderViewModel model ,string url)
        {
            var product = _unitOfWork.ProductRepository.Get(a => a.Url == url).FirstOrDefault();
            if (!ModelState.IsValid)
            {
                var models = new OrderViewModel
                {
                    Products = product,
                };
                return PartialView(models);
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
            MailMessage ms = new MailMessage(System.Configuration.ConfigurationManager.AppSettings["Email"].ToString(), ConfigSite.Email);
            ms.Subject = $"Email liên hệ từ " + Request.Url?.Host;
            ms.Body = body;
            ms.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Timeout = 3000000;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            NetworkCredential nc = new NetworkCredential("sendermailasp.net@gmail.com", "wegxxdwhmgbzuiwk");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = nc;
            smtp.Send(ms);
            TempData["success"] = "Gửi liên hệ thành công ! \nChúng tôi sẽ liên lạc với bạn sớm nhất có thể.";
            return PartialView(model);
        }
        public PartialViewResult Header()
        {
            var model = new HeaderViewModel
            {
                ProductCategories = ProductCategories.Where(a => a.ShowMenu),
                IntroduceCat = ArticleCategories.Where(a => a.ShowMenu && a.TypePost == TypePost.Introduce),
                Articles =  ArticleCategories.Where(a => a.CategoryActive && a.ShowMenu && (a.TypePost == TypePost.Article)),
            };
            return PartialView(model);
        }
        public PartialViewResult Footer()
        {
            var model = new FooterViewModel
            {
                ArticleCategories = ArticleCategories.Where(a => a.ShowFooter && a.TypePost == TypePost.Article),
                Introduce = ArticleCategories.Where(a => a.ShowFooter && a.TypePost == TypePost.Introduce),
                Policies = ArticleCategories.Where(a => a.ShowFooter && a.TypePost == TypePost.Policy),
                FooterProduct = ProductCategories.Where(a => a.ShowFooter )

            };
            return PartialView(model);
        }
    }
}