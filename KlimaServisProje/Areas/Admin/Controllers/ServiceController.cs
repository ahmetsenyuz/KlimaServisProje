using System;
using System.Collections.Generic;
using System.Linq;
using KlimaServisProje.Data;
using KlimaServisProje.Models.ArizaKayit;
using KlimaServisProje.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KlimaServisProje.Areas.Admin.Controllers
{
    [Route("[area]/[controller]/[action]")]
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ServiceController : Controller
    {
        private readonly MyContext _context;

        public ServiceController(MyContext context)
        {
            _context = context;
        }

        public IActionResult OperationList()
        {
            var data = _context.OperationPrices.ToList();
            var model = new List<OperationPriceViewModal>();
            foreach (var item in data)
            {
                var operation = new OperationPriceViewModal()
                {
                    operationId = item.operationId,
                    operationName = item.operationName,
                    price = item.price,
                    description = item.description
                };
                model.Add(operation);
            }
            return View(model);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(OperationPriceViewModal model)
        {
            if (!ModelState.IsValid)
            {
                TempData["message"] = "bir hata meydana geldi";
                return View(model);
            }

            var data = new OperationPrice()
            {
                operationId = model.operationId,
                operationName = model.operationName,
                description = model.description,
                price = model.price
            };
            var result = _context.OperationPrices.Add(data);
            try
            {
                _context.SaveChanges();
                return RedirectToAction("OperationList");
            }
            catch (Exception e)
            {
                TempData["message"] = "bir hata meydana geldi";
                return View(model);
            }
        }
    }
}
