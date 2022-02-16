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

        public IActionResult Delete(int Id)
        {

            var data = _context.OperationPrices.FirstOrDefault(x => x.operationId == Id);
            try
            {
                _context.OperationPrices.Remove(data);
                _context.SaveChanges();
                RedirectToAction("OperationList");
            }
            catch (Exception e)
            {
                
            }
            return RedirectToAction("OperationList");
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

        public IActionResult OperationDetail(int id)
        {
            var data = _context.OperationPrices.FirstOrDefault(x => x.operationId == id);
            var model = new OperationPriceViewModal()
            {
                operationId = data.operationId,
                operationName = data.operationName,
                description = data.description,
                price = data.price
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult OperationDetail(OperationPriceViewModal model)
        {
            var data = _context.OperationPrices.FirstOrDefault(x => x.operationId == model.operationId);
            data.description = model.description;
            data.operationName = model.operationName;
            data.price = model.price;
            try
            {
                _context.OperationPrices.Update(data);
                _context.SaveChanges();
            }
            catch (Exception e)
            {

            }
            return RedirectToAction("OperationList");
        }
    }
}
