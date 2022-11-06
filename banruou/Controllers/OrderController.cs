using banruou.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using banruou.Models;
using banruou.ViewModel;
using PagedList;

namespace banruou.Controllers
{
    public class OrderController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();


        public ActionResult ListOrder(int? page , string name  )
        {
            var pageNumber = page ?? 1;
            const int pageSize = 15;
            var contact = _unitOfWork.OrderRepository.GetQuery(orderBy: l => l.OrderByDescending(a => a.Id));

            if (!string.IsNullOrEmpty(name))
            {
                contact = contact.Where(l => l.Name.Contains(name));
            }
            var model = new ListOrderViewModel
            {
                Orders = contact.ToPagedList(pageNumber, pageSize),
                Name = name
            };
            return View(model);
        }
        public bool DeleteOrder(int contactId = 0)
        {
            var contact = _unitOfWork.OrderRepository.GetById(contactId);
            if (contact == null)
            {
                return false;
            }
            _unitOfWork.OrderRepository.Delete(contact);
            _unitOfWork.Save();
            return true;
        }
    }   
}