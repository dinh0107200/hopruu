using banruou.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using banruou.Models;
using banruou.ViewModel;
using Helpers;
using System.IO;
using System.Data.Entity;
using PagedList;
using System.Drawing;

namespace banruou.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private IEnumerable<ProductCategory> ProductCategories => _unitOfWork.ProductCategoryRepository.Get(a => a.CategoryActive, q => q.OrderBy(a => a.CategorySort));
        private SelectList ParentProductCategoryList => new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");

        #region product category
        public ActionResult ListCategory()
        {
            var allcats = _unitOfWork.ProductCategoryRepository.Get(orderBy: q => q.OrderBy(a => a.CategorySort));
            return PartialView(allcats);
        }
        public ActionResult ProductCategory(string result = "")
        {
            ViewBag.NewsCat = result;
            ViewBag.RootCats =
                new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");
            return View(new ProductCategory { CategorySort = 1 });
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ProductCategory(ProductCategory category, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    if (file.ContentType != "image/jpeg" & file.ContentType != "image/png" && file.ContentType != "image/gif")
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                    }
                    else
                    {
                        if (file.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                        }
                        else
                        {
                            var imgPath = "/images/productCategory/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);

                            category.Image = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                            file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                        }
                    }
                }
                category.Url = HtmlHelpers.ConvertToUnSign(null, category.Url ?? category.CategoryName);
                _unitOfWork.ProductCategoryRepository.Insert(category);
                _unitOfWork.Save();
                return RedirectToAction("ProductCategory", new { result = "success" });
            }
            ViewBag.RootCats = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");
            return View(category);
        }
        public ActionResult UpdateCategory(int catId = 0)
        {
            var category = _unitOfWork.ProductCategoryRepository.GetById(catId);
            if (category == null)
            {
                return RedirectToAction("ProductCategory");
            }
            ViewBag.RootCats = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");
            return View(category);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateCategory(ProductCategory category, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    if (file.ContentType != "image/jpeg" & file.ContentType != "image/png" && file.ContentType != "image/gif")
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                    }
                    else
                    {
                        if (file.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                        }
                        else
                        {
                            var imgPath = "/images/productCategory/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);

                            if (System.IO.File.Exists(Server.MapPath("/images/productCategory/" + category.Image)))
                            {
                                System.IO.File.Delete(Server.MapPath("/images/productCategory/" + category.Image));
                            }
                            category.Image = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                            file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                        }
                    }
                }
                else
                {
                    category.Image = fc["CurrentFile"];
                }
                category.Url = HtmlHelpers.ConvertToUnSign(null, category.Url ?? category.CategoryName);
                _unitOfWork.ProductCategoryRepository.Update(category);
                _unitOfWork.Save();
                return RedirectToAction("ProductCategory", new { result = "update" });
            }
            ViewBag.RootCats = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");
            return View(category);
        }
        [HttpPost]
        public bool DeleteCategory(int catId = 0)
        {
            var category = _unitOfWork.ProductCategoryRepository.GetById(catId);
            if (category == null)
            {
                return false;
            }
            _unitOfWork.ProductCategoryRepository.Delete(category);
            _unitOfWork.Save();
            return true;
        }   
        [HttpPost]
        public bool UpdateProductCategory(int sort = 1, bool active = false, bool menu = false, int projectCatId = 0)
        {
            var projectCat = _unitOfWork.ProductCategoryRepository.GetById(projectCatId);
            if (projectCat == null)
            {
                return false;
            }
            projectCat.CategorySort = sort;
            projectCat.CategoryActive = active;
            projectCat.ShowMenu = menu;

            _unitOfWork.Save();
            return true;
        }
        #endregion




        #region product
        public ActionResult ListProduct(int? page, string name, int? parentId, int catId = 0, string sort = "date-desc", string result = "")
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            const int pageSize = 15;
            var projects = _unitOfWork.ProductRepository.GetQuery().AsNoTracking();
            if (catId > 0)
            {
                projects = projects.Where(a => a.ProductCategoryId == catId);
            }
            else if (parentId > 0)
            {
                projects = projects.Where(a => a.ProductCategoryId == parentId);
            }
            if (!string.IsNullOrEmpty(name))
            {
                projects = projects.Where(l => l.Name.ToLower().Contains(name.ToLower()));
            }

            switch (sort)
            {
                case "sort-asc":
                    projects = projects.OrderBy(a => a.Sort);
                    break;
                case "sort-desc":
                    projects = projects.OrderByDescending(a => a.Sort);
                    break;
                case "date-asc":
                    projects = projects.OrderBy(a => a.CreateDate);
                    break;
                default:
                    projects = projects.OrderByDescending(a => a.CreateDate);
                    break;
            }
            var model = new ListProjectViewModel
            {
                SelectCategories = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName"),
                Products = projects.ToPagedList(pageNumber, pageSize),
                CatId = catId,
                ParentId = parentId,
                Name = name,
                Sort = sort,
            };
            if (parentId.HasValue)
            {
                model.ChildCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == parentId), "Id", "Categoryname");
            }
            return View(model);
        }
        public ActionResult Product()
        {
            var model = new InsertProjectViewModel
            {
                Product = new Product { Sort = 1, Active = true },
                Categories = ProductCategories,
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Product(InsertProjectViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                var isPost = true;
                var file = Request.Files["Product.Image"];
                if (file != null && file.ContentLength > 0)
                {
                    if (!HtmlHelpers.CheckFileExt(file.FileName, "jpg|jpeg|png|gif"))
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                        isPost = false;
                    }
                    else
                    {

                        if (file.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                            isPost = false;
                        }
                        else
                        {
                            var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);
                            var imgPath = "/images/Product/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                            model.Product.Image = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                            var newImage = Image.FromStream(file.InputStream);
                            var fixSizeImage = HtmlHelpers.FixedSize(newImage, 800, 600, false);
                            HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);
                        }
                    }
                }
                if (isPost)
                {
                    model.Product.Url = HtmlHelpers.ConvertToUnSign(null, model.Product.Url ?? model.Product.Name);
                    var url = _unitOfWork.ProductRepository.GetQuery(a => a.Url == model.Product.Url);
                    if (url != null)
                    {
                        model.Product.Url += "-" + DateTime.Now.Millisecond;
                    }
                    model.Product.ProductCategoryId = model.CategoryId ?? model.ParentId;
                    _unitOfWork.ProductRepository.Insert(model.Product);
                    _unitOfWork.Save();
                }
                return RedirectToAction("ListProduct", new { result = "success" });
            }
            model.ProjectCategoryList = ParentProductCategoryList;
            model.ChildCategoryList = new SelectList(new List<ProductCategory>(), "Id", "CategoryName");

            if (model.ParentId > 0)
            {
                model.ChildCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == model.ParentId), "Id", "CategoryName");
            }
            return View(model);
        }
        public ActionResult UpdateProduct(int proId = 0)
        {
            var product = _unitOfWork.ProductRepository.GetById(proId);
            if (product == null)
            {
                return RedirectToAction("ListProduct");
            }
            var model = new InsertProjectViewModel
            {
                Product = product,
                Categories = ProductCategories,
                ParentId = product.ProductCategory.ParentId ?? product.ProductCategoryId,
                ProjectCategoryList = ParentProductCategoryList,
                CategoryId = product.ProductCategoryId,
            };
            if (model.ParentId > 0)
            {
                model.ChildCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == model.ParentId), "Id", "CategoryName");
            }
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateProduct(InsertProjectViewModel model, FormCollection fc)
        {
            var product = _unitOfWork.ProductRepository.GetById(model.Product.Id);
            if (product == null)
            {
                return RedirectToAction("ListProduct");
            }
            if (ModelState.IsValid)
            {
                var isPost = true;
                var file = Request.Files["Product.Image"];
                if (file != null && file.ContentLength > 0)
                {
                    if (!HtmlHelpers.CheckFileExt(file.FileName, "jpg|jpeg|png|gif"))
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                        isPost = false;
                    }
                    else
                    {
                        var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(file.FileName)) + "-" + DateTime.Now.Millisecond + Path.GetExtension(file.FileName);
                        if (file.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                            isPost = false;
                        }
                        else
                        {
                            var imgPath = "/images/Product/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            HtmlHelpers.DeleteFile(Server.MapPath("/images/Product/" + product.Image));
                            product.Image = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                            var newImage = Image.FromStream(file.InputStream);
                            var fixSizeImage = HtmlHelpers.FixedSize(newImage, 800, 600, false);
                            HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);
                        }
                    }
                }
                if (isPost)
                {
                    product.Image = fc["Product.Image"];
                    product.Url = HtmlHelpers.ConvertToUnSign(null, model.Product.Url ?? model.Product.Name);                   
                    product.ProductCategoryId = model.CategoryId ?? model.ParentId;
                    product.Name = model.Product.Name;
                    product.Body = model.Product.Body;
                    product.Active = model.Product.Active;
                    product.Home = model.Product.Home;
                    product.TitleMeta = model.Product.TitleMeta;
                    product.DescriptionMeta = model.Product.DescriptionMeta;
                    product.Sort = model.Product.Sort;
                    product.Capacity = model.Product.Capacity;
                    product.Application = model.Product.Application;
                    product.Country = model.Product.Country;
                    product.Material = model.Product.Material;
                    product.Price = model.Product.Price;
                    product.Hot = model.Product.Hot;
                    var count = _unitOfWork.ProductRepository.GetQuery(m => m.Url == product.Url).Count();
                    if (count > 1)
                    {
                       product.Url = model.Product.Url += "-" + model.Product.Id;
                        _unitOfWork.Save();
                    }
                }

                _unitOfWork.Save();
                return RedirectToAction("ListProduct", new { result = "update" });
            }
            model.ProjectCategoryList = ParentProductCategoryList;
            if (model.ParentId > 0)
            {
                model.ChildCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == model.ParentId), "Id", "CategoryName");
            }
            return View(model);
        }
        [HttpPost]
        public bool DeleteProduct(int proId = 0)
        {
            var project = _unitOfWork.ProductRepository.GetById(proId);
            if (project == null)
            {
                return false;
            }
            _unitOfWork.ProductRepository.Delete(project);
            _unitOfWork.Save();
            return true;
        }
        [HttpPost]
        public bool QuickUpdate(bool? status, bool active, bool home, int sort = 0, int proId = 0)
        {
            var product = _unitOfWork.ProductRepository.GetById(proId);
            if (product == null)
            {
                return false;
            }
            if (status != null)
            {
                product.Active = Convert.ToBoolean(status);
            }
            if (status != null)
            {
                product.Home = Convert.ToBoolean(status);
            }
            if (sort >= 0)
            {
                product.Sort = sort;
            }
            //project.Hot = hot;
            product.Active = active;
            product.Home = home;
            _unitOfWork.Save();
            return true;
        }
        #endregion


        public JsonResult GetProductCategory(int? parentId)
        {
            var categories = ProductCategories.Where(a => a.ParentId == parentId);
            return Json(categories.Select(a => new { a.Id, Name = a.CategoryName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetChildCategory(int parentId = 0)
        {
            var childCategories = ProductCategories.Where(a => a.ParentId == parentId);
            return Json(childCategories.Select(a => new { a.Id, a.CategoryName }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}