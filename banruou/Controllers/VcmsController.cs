using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using banruou.DAL;
using banruou.Models;
using banruou.ViewModel;
using Helpers;
using PagedList;
namespace banruou.Controllers
{
    [Authorize]
    public class VcmsController : Controller
    {
        public readonly UnitOfWork _unitOfWork = new UnitOfWork();
        // GET: Vcms
        #region Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(AdminLoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var admin = _unitOfWork.AdminRepository.Get(a => a.Username == model.Username && a.Active).SingleOrDefault();

                if (admin != null && HtmlHelpers.VerifyHash(model.Password, "SHA256", admin.Password))
                {
                    var ticket = new FormsAuthenticationTicket(1, model.Username.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                        admin.ToString(), FormsAuthentication.FormsCookiePath);

                    var encTicket = FormsAuthentication.Encrypt(ticket);
                    // Create the cookie.
                    Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Vcms");
                }
                ModelState.AddModelError("", @"Tên đăng nhập hoặc mật khẩu không chính xác.");
            }
            return View(model);
        }
        public RedirectToRouteResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Vcms");
        }
        #endregion
        #region Admin
        public ActionResult Index()
        {
            var model = new InfoAdminViewModel
            {
                Admins = _unitOfWork.AdminRepository.GetQuery().Count(),
                Articles = _unitOfWork.ArticleRepository.GetQuery().Count(),
                //Contacts = _unitOfWork.ContactRepository.GetQuery().Count(),
                Banners = _unitOfWork.BannerRepository.GetQuery().Count(),
                //Feedbacks = _unitOfWork.FeedbackRepository.GetQuery().Count(),
                Product = _unitOfWork.ProductRepository.GetQuery().Count(),
                //Services = _unitOfWork.ServiceRepository.GetQuery().Count(),
            };
            return View(model);
        }
        [ChildActionOnly]
        public PartialViewResult ListAdmin()
        {
            var admins = _unitOfWork.AdminRepository.Get();
            return PartialView("ListAdmin", admins);
        }
        public ActionResult CreateAdmin(string result = "")
        {
            ViewBag.Result = result;
            return View();
        }
        [HttpPost]
        public ActionResult CreateAdmin(Admin model)
        {
            if (ModelState.IsValid)
            {
                var admin = _unitOfWork.AdminRepository.GetQuery(a => a.Username.Equals(model.Username)).SingleOrDefault();
                if (admin != null)
                {
                    ModelState.AddModelError("", @"Tên đăng nhập này có rồi");
                }
                else
                {
                    var hashPass = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                    _unitOfWork.AdminRepository.Insert(new Admin { Username = model.Username, Password = hashPass, Active = model.Active });
                    _unitOfWork.Save();
                    return RedirectToAction("CreateAdmin", new { result = "success" });
                }
            }
            return View();
        }
        public ActionResult EditAdmin(int adminId = 0)
        {
            var admin = _unitOfWork.AdminRepository.GetById(adminId);
            if (admin == null)
            {
                return RedirectToAction("CreateAdmin");
            }

            var model = new EditAdminViewModel
            {
                Id = admin.Id,
                Username = admin.Username,
                Active = admin.Active,
            };

            return View(model);
        }
        [HttpPost]
        public ActionResult EditAdmin(EditAdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                var admin = _unitOfWork.AdminRepository.GetById(model.Id);
                if (admin == null)
                {
                    return RedirectToAction("CreateAdmin");
                }
                if (admin.Username != model.Username)
                {
                    var exists = _unitOfWork.AdminRepository.GetQuery(a => a.Username.Equals(model.Username)).SingleOrDefault();
                    if (exists != null)
                    {
                        ModelState.AddModelError("", @"Tên đăng nhập này có rồi");
                        return View(model);
                    }
                    admin.Username = model.Username;
                }
                admin.Active = model.Active;
                if (model.Password != null)
                {
                    admin.Password = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                }
                _unitOfWork.Save();
                return RedirectToAction("CreateAdmin", new { result = "update" });
            }
            return View(model);
        }
        public bool DeleteAdmin(string username)
        {
            var admin = _unitOfWork.AdminRepository.GetQuery(a => a.Username.Equals(username)).SingleOrDefault();
            if (admin == null)
            {
                return false;
            }
            if (username == "admin")
            {
                return false;
            }
            _unitOfWork.AdminRepository.Delete(admin);
            _unitOfWork.Save();
            return true;
        }
        public ActionResult ChangePassword(int result = 0)
        {
            ViewBag.Result = result;
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var admin = _unitOfWork.AdminRepository.GetQuery(a => a.Username.Equals(User.Identity.Name,
                StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
                if (admin == null)
                {
                    return HttpNotFound();
                }
                if (HtmlHelpers.VerifyHash(model.OldPassword, "SHA256", admin.Password))
                {
                    admin.Password = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                    _unitOfWork.Save();
                    return RedirectToAction("ChangePassword", new { result = 1 });
                }
                else
                {
                    ModelState.AddModelError("", @"Mật khẩu hiện tại không đúng!");
                    return View();
                }
            }
            return View(model);
        }
        #endregion
        #region ConfigSite
        public ActionResult ConfigSite(string result = "")
        {
            ViewBag.Result = result;
            var config = _unitOfWork.ConfigSiteRepository.Get().FirstOrDefault();
            return View(config);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ConfigSite(ConfigSite model, FormCollection fc)
        {
            var config = _unitOfWork.ConfigSiteRepository.Get().FirstOrDefault();
            if (config == null)
            {
                _unitOfWork.ConfigSiteRepository.Insert(model);
                _unitOfWork.Save();
            }
            else
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;

                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/configs/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                    var newImage = Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1000, 1000, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);

                    if (Request.Files.Keys[i] == "Image")
                    {
                        config.Image = imgFile;
                    }
                    else if (Request.Files.Keys[i] == "AboutImage")
                    {
                        config.AboutImage = imgFile;
                    }
                    else if (Request.Files.Keys[i] == "Favicon")
                    {
                        config.Favicon = imgFile;
                    }
                }

                config.Facebook = model.Facebook;
                config.GoogleMap = model.GoogleMap;
                config.Youtube = model.Youtube;
                config.Twitter = model.Twitter;
                config.Instagram = model.Instagram;
                config.Title = model.Title;
                config.Description = model.Description;
                config.GoogleAnalytics = model.GoogleAnalytics;
                config.Hotline = model.Hotline;
                config.Email = model.Email;
                config.LiveChat = model.LiveChat;
                config.Address = model.Address;
                config.Address2 = model.Address2;
                config.AboutText = model.AboutText;
                config.AboutUrl = model.AboutUrl;
                config.InfoFooter = model.InfoFooter;
                config.InfoContact = model.InfoContact;
                config.Hotline2 = model.Hotline2;
                config.Hotline3 = model.Hotline3;
                config.Hotline4 = model.Hotline4;
                config.Zalo = model.Zalo;

                _unitOfWork.Save();
                HttpContext.Application["ConfigSite"] = config;
                return RedirectToAction("ConfigSite", "Vcms", new { result = "success" });
            }
            return View("ConfigSite", model);
        }
        #endregion
        #region Question
        [ChildActionOnly]
        public ActionResult ListQuestion()
        {
            var allQuestions = _unitOfWork.QuestionRepository.Get(orderBy: q => q.OrderBy(a => a.Sort));
            return PartialView(allQuestions);
        }
        public ActionResult Question(string result = "")
        {
            ViewBag.Question = result;
            var model = new InsertQuestionViewModel
            {
                Question = new Question { Active = true, Sort = 1 }
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Question(InsertQuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Question.Url = HtmlHelpers.ConvertToUnSign(null, model.Question.Url ?? model.Question.QuestionName);

                var questions = _unitOfWork.QuestionRepository.GetQuery().AsNoTracking();
                if (questions.Any(a => a.Url.ToLower().Trim() == model.Question.Url.ToLower().Trim()))
                {
                    model.Question.Url += "-" + DateTime.Now.Millisecond;
                }
                _unitOfWork.QuestionRepository.Insert(model.Question);
                _unitOfWork.Save();
                return RedirectToAction("Question", new { result = "success" });
            }

            return View(model);
        }
        public ActionResult UpdateQuestion(int questionId = 0)
        {
            var question = _unitOfWork.QuestionRepository.GetById(questionId);
            if (question == null)
            {
                return RedirectToAction("Question");
            }
            var model = new InsertQuestionViewModel
            {
                Question = question,
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateQuestion(InsertQuestionViewModel model)
        {
            var question = _unitOfWork.QuestionRepository.GetById(model.Question.Id);
            if (question == null)
            {
                return RedirectToAction("Question");
            }
            if (ModelState.IsValid)
            {
                question.Url = HtmlHelpers.ConvertToUnSign(null, model.Question.Url ?? model.Question.QuestionName);
                question.QuestionName = model.Question.QuestionName;
                question.Body = model.Question.Body;
                question.Active = model.Question.Active;
                question.Sort = model.Question.Sort;

                _unitOfWork.Save();
                return RedirectToAction("Question", new { result = "update" });
            }

            return View(model);
        }
        [HttpPost]
        public bool DeleteQuestion(int questionId = 0)
        {

            var question = _unitOfWork.QuestionRepository.GetById(questionId);
            if (question == null)
            {
                return false;
            }
            _unitOfWork.QuestionRepository.Delete(question);
            _unitOfWork.Save();
            return true;
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}