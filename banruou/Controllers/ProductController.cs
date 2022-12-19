using banruou.DAL;
using banruou.Models;
using banruou.ViewModel;
using Helpers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace banruou.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private IEnumerable<ProductCategory> ProductCategories => _unitOfWork.ProductCategoryRepository.Get(a => a.CategoryActive, q => q.OrderBy(a => a.CategorySort));
        private SelectList ParentProductCategoryList => new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");

        #region Product category
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

            var model = new InsertProductCategoryViewModel
            {
                Category = new ProductCategory { CategorySort = 1 },
                GroupPropertyOfProducts = _unitOfWork.GroupPropertyOfProductRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort))
            };

            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ProductCategory(InsertProductCategoryViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                var file = Request.Files["Category.Image"];
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

                            model.Category.Image = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                            file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                        }
                    }
                }

                var groupProperty = fc["GroupProperty"];
                if (!string.IsNullOrEmpty(groupProperty))
                {
                    model.Category.GroupPropertyOfProductId = Convert.ToInt32(groupProperty);
                }

                model.Category.Url = HtmlHelpers.ConvertToUnSign(null, model.Category.Url ?? model.Category.CategoryName);
                _unitOfWork.ProductCategoryRepository.Insert(model.Category);
                _unitOfWork.Save();

                var count = _unitOfWork.ProductCategoryRepository.GetQuery(a => a.Url == model.Category.Url).Count();
                if (count > 1)
                {
                    model.Category.Url += "-" + model.Category.Id;
                    _unitOfWork.Save();
                }
                return RedirectToAction("ProductCategory", new { result = "success" });
            }
            ViewBag.RootCats = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");
            model.GroupPropertyOfProducts = _unitOfWork.GroupPropertyOfProductRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));

            return View(model);
        }
        public ActionResult UpdateCategory(int catId = 0)
        {
            var category = _unitOfWork.ProductCategoryRepository.GetById(catId);
            if (category == null)
            {
                return RedirectToAction("ProductCategory");
            }

            var model = new InsertProductCategoryViewModel
            {
                Category = category,
                GroupPropertyOfProducts = _unitOfWork.GroupPropertyOfProductRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort))
            };
            ViewBag.RootCats = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateCategory(InsertProductCategoryViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                var file = Request.Files["Category.Image"];
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

                            if (System.IO.File.Exists(Server.MapPath("/images/productCategory/" + model.Category.Image)))
                            {
                                System.IO.File.Delete(Server.MapPath("/images/productCategory/" + model.Category.Image));
                            }
                            model.Category.Image = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                            file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                        }
                    }
                }
                else
                {
                    model.Category.Image = fc["CurrentFile"];
                }
                model.Category.Url = HtmlHelpers.ConvertToUnSign(null, model.Category.Url ?? model.Category.CategoryName);

                var groupProperty = fc["GroupProperty"];
                if (!string.IsNullOrEmpty(groupProperty))
                {
                    model.Category.GroupPropertyOfProductId = Convert.ToInt32(groupProperty);
                }
                else
                {
                    model.Category.GroupPropertyOfProductId = null;
                }
                _unitOfWork.ProductCategoryRepository.Update(model.Category);
                _unitOfWork.Save();

                var count = _unitOfWork.ProductCategoryRepository.GetQuery(a => a.Url == model.Category.Url).Count();
                if (count > 1)
                {
                    model.Category.Url += "-" + model.Category.Id;
                    _unitOfWork.Save();
                }

                return RedirectToAction("ProductCategory", new { result = "update" });
            }
            ViewBag.RootCats = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");
            model.GroupPropertyOfProducts =
                _unitOfWork.GroupPropertyOfProductRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
            return View(model);
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
        public bool UpdateProductCategory(int sort = 1, bool active = false, bool menu = false, bool home = false, int projectCatId = 0)
        {
            var projectCat = _unitOfWork.ProductCategoryRepository.GetById(projectCatId);
            if (projectCat == null)
            {
                return false;
            }
            projectCat.CategorySort = sort;
            projectCat.CategoryActive = active;
            projectCat.ShowMenu = menu;
            projectCat.ShowHome = home;

            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region Product
        public ActionResult ListProduct(int? page, string name, int? parentId, int? catId, string sort, string result = "")
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            const int pageSize = 20;
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
            var count = _unitOfWork.ProductRepository.GetQuery().Count();
            var model = new InsertProjectViewModel
            {
                Product = new Product { Sort = count + 1, Active = true },
                Categories = ProductCategories
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Product(InsertProjectViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                model.Product.ListImage = fc["Pictures"];
                //model.Product.ProductCategoryId = model.CategoryId ?? model.ParentId;
                model.Product.Url = HtmlHelpers.ConvertToUnSign(null, model.Product.Url ?? model.Product.Name);
                _unitOfWork.ProductRepository.Insert(model.Product);
                _unitOfWork.Save();

                var categoryIds = fc.GetValues("CategoryIds");
                if (categoryIds != null)
                {
                    foreach (var id in categoryIds)
                    {
                        var categoryId = Convert.ToInt32(id);
                        var property = _unitOfWork.ProductCategoryRepository.GetById(categoryId);
                        property?.Products.Add(model.Product);
                    }
                    _unitOfWork.Save();
                }


                var count = _unitOfWork.ProductRepository.GetQuery(a => a.Url == model.Product.Url).Count();
                if (count > 1)
                {
                    model.Product.Url += "-" + model.Product.Id;
                    _unitOfWork.Save();
                }

                var propertyProducts = fc.GetValues("PropertyOfProducts");
                var propertyNames = fc.GetValues("PropertyNames");
                if (propertyProducts != null && propertyNames != null)
                {
                    for (var i = 0; i < propertyProducts.Length; i++)
                    {
                        var pp = propertyProducts[i];
                        var pn = propertyNames[i];

                        //if (string.IsNullOrEmpty(pn)) continue;

                        var newPropery = new PropertyAndProductDetail
                        {
                            ProductId = model.Product.Id,
                            PropertyOfProductId = Convert.ToInt32(pp),
                            Name = pn
                        };
                        _unitOfWork.PropertyAndProductDetailRepository.Insert(newPropery);
                    }
                    _unitOfWork.Save();
                }

                return RedirectToAction("ListProduct", new { result = "success" });
            }
            model.Categories = ProductCategories;
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
                //ParentId = product.ProductCategory.ParentId ?? product.ProductCategoryId,
                //ProjectCategoryList = ParentProductCategoryList,
                //CategoryId = product.ProductCategoryId,
                GroupPropertyOfProducts = _unitOfWork.GroupPropertyOfProductRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort))
            };
            //if (model.ParentId > 0)
            //{
            //    model.ChildCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == model.ParentId), "Id", "CategoryName");
            //}
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
                product.ListImage = fc["Pictures"] == "" ? null : fc["Pictures"];
                product.Url = HtmlHelpers.ConvertToUnSign(null, model.Product.Url ?? model.Product.Name);
                //product.ProductCategoryId = model.CategoryId ?? model.ParentId;
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

                if (product.ProductCategories.Any())
                {
                    product.ProductCategories.Clear();
                }
                if (product.PropertyAndProductDetails.Any())
                {
                    product.PropertyAndProductDetails.Clear();
                }
                _unitOfWork.Save();
                var categoryIds = fc.GetValues("CategoryIds");
                if (categoryIds != null)
                {
                    foreach (var id in categoryIds)
                    {
                        var categoryId = Convert.ToInt32(id);
                        var property = _unitOfWork.ProductCategoryRepository.GetById(categoryId);
                        product.ProductCategories.Add(property);
                    }
                    _unitOfWork.Save();
                }
                var count = _unitOfWork.ProductRepository.GetQuery(a => a.Url == model.Product.Url).Count();
                if (count > 1)
                {
                    product.Url += "-" + product.Id;
                    _unitOfWork.Save();
                }
                var propertyProducts = fc.GetValues("PropertyOfProducts");
                var propertyNames = fc.GetValues("PropertyNames");
                if (propertyProducts != null && propertyNames != null)
                {
                    for (var i = 0; i < propertyProducts.Length; i++)
                    {
                        var pp = propertyProducts[i];
                        var pn = propertyNames[i];

                        //if (string.IsNullOrEmpty(pn)) continue;

                        var newPropery = new PropertyAndProductDetail
                        {
                            ProductId = model.Product.Id,
                            PropertyOfProductId = Convert.ToInt32(pp),
                            Name = pn
                        };
                        _unitOfWork.PropertyAndProductDetailRepository.Insert(newPropery);
                    }
                    _unitOfWork.Save();
                }

                return RedirectToAction("ListProduct", new { result = "update" });
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

        public PartialViewResult GetPropertyByGroup(int groupId)
        {
            var groupProperty = _unitOfWork.ProductCategoryRepository.GetById(groupId);
            return PartialView(groupProperty);
        }
        #endregion

        #region GroupProperty
        public ActionResult PropertyOfProduct(int? id, int result = 0)
        {
            var model = new PropertyOfProduct
            {
                Sort = 1,
                Active = true
            };
            if (id.HasValue)
            {
                model = _unitOfWork.PropertyOfProductRepository.GetById(id);
            }
            ViewBag.Result = result;
            return View(model);
        }
        [HttpPost]
        public ActionResult PropertyOfProduct(PropertyOfProduct model)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.PropertyOfProductRepository.AddOrUpdate(model);
                _unitOfWork.Save();
                return RedirectToAction("PropertyOfProduct", new { id = (int?)null, result = 1 });
            }
            return View(model);
        }
        [ChildActionOnly]
        public PartialViewResult PropertyOfProductList()
        {
            var properties = _unitOfWork.PropertyOfProductRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
            return PartialView(properties);
        }
        [HttpPost]
        public bool DeletePropertyOfProduct(int id)
        {
            var propertyOfProduct = _unitOfWork.PropertyOfProductRepository.GetById(id);
            if (propertyOfProduct == null)
            {
                return false;
            }

            _unitOfWork.PropertyOfProductRepository.Delete(propertyOfProduct);
            _unitOfWork.Save();
            return true;
        }

        public ActionResult GroupPropertyOfProduct(int? id, int result = 0)
        {
            var model = new AddOrUpdateGroupPropertyViewModel
            {
                GroupPropertyOfProduct = new GroupPropertyOfProduct
                {
                    Sort = 1,
                    Active = true,
                    PropertyOfProducts = new List<PropertyOfProduct>()
                }
            };

            if (id.HasValue)
            {
                model.GroupPropertyOfProduct = _unitOfWork.GroupPropertyOfProductRepository.GetById(id);
            }

            model.PropertyOfProducts = _unitOfWork.PropertyOfProductRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));

            return View(model);
        }
        [HttpPost]
        public ActionResult GroupPropertyOfProduct(AddOrUpdateGroupPropertyViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.GroupPropertyOfProductRepository.AddOrUpdate(model.GroupPropertyOfProduct);
                _unitOfWork.Save();

                var propertyIds = fc.GetValues("PropertyId");
                if (propertyIds != null)
                {
                    var group = _unitOfWork.GroupPropertyOfProductRepository.GetById(model.GroupPropertyOfProduct.Id);
                    group.PropertyOfProducts?.Clear();

                    foreach (var propertyId in propertyIds)
                    {
                        var id = Convert.ToInt32(propertyId);
                        var property = _unitOfWork.PropertyOfProductRepository.GetById(id);
                        if (property == null) continue;
                        property.GroupPropertyOfProducts.Add(group);
                        _unitOfWork.Save();
                    }
                }

                return RedirectToAction("PropertyOfProduct", new { id = (int?)null, result = 1 });
            }
            model.PropertyOfProducts = _unitOfWork.PropertyOfProductRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
            return View(model);
        }

        [ChildActionOnly]
        public PartialViewResult GroupPropertyOfProductList()
        {
            var groupPropertyOfProducts = _unitOfWork.GroupPropertyOfProductRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
            return PartialView(groupPropertyOfProducts);
        }
        [HttpPost]
        public bool DeletePropertyAndGroup(int propertyId, int group)
        {
            try
            {
                var groupProperty = _unitOfWork.GroupPropertyOfProductRepository.GetById(group);
                var property = _unitOfWork.PropertyOfProductRepository.GetById(propertyId);
                groupProperty.PropertyOfProducts.Remove(property);
                _unitOfWork.Save();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
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
        public JsonResult GetProperties(int? parentId)
        {
            var categories = _unitOfWork.ProductCategoryRepository.Get(a => a.ParentId == parentId);
            return Json(categories.Select(a => new { a.Id, Name = a.CategoryName }), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}