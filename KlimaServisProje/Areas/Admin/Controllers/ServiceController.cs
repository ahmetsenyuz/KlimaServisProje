using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using KlimaServisProje.Data;
using KlimaServisProje.Models.ArizaKayit;
using KlimaServisProje.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KlimaServisProje.Areas.Admin.Controllers
{
    [Route("[area]/[controller]/[action]")]
    [Area("Admin")]
    [Authorize(Roles = "Admin,Operator,Technician")]
    public class ServiceController : Controller
    {
        private readonly MyContext _context;
        private readonly IMapper _mapper;

        public ServiceController(MyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [Authorize(Roles = "Admin")]

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
        [Authorize(Roles = "Admin")]

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
        [Authorize(Roles = "Admin")]

        public IActionResult Add()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]

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
        [Authorize(Roles = "Admin")]

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
        [Authorize(Roles = "Admin")]

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

        public List<TechnicianViewModel> GeTechniciansWithName()
        {
            var rolename = _context.Roles.FirstOrDefault(x => x.Name == "Technician");
            var role = _context.UserRoles.Where(x => x.RoleId == rolename.Id).ToList();
            var model = new List<TechnicianViewModel>();
            foreach (var item in role)
            {
                var nameSurname = _context.Users.FirstOrDefault(x => x.Id == item.UserId);
                var data = new TechnicianViewModel()
                {
                    Id = item.UserId,
                    Name = nameSurname.Name + " " + nameSurname.Surname
                };
                model.Add(data);
            }
            return model;
        }
        public List<DropdownListItems> GetTechnicians()
        {
            var model = GeTechniciansWithName();
            var list = new List<DropdownListItems>();
            foreach (var item in model.Where(x=> x.Status == false))
            {

                var technicians = new DropdownListItems()
                {
                    Id = item.Id,
                    Name = item.Name
                };
                list.Add(technicians);
            }

            return list;
        }
        [Authorize(Roles = "Admin,Operator")]

        public IActionResult GetReports()
        {
            var data = _context.TroubleRegisters.Where(x=> x.Finished == false).ToList();
            List<TroubleRegisterViewModel> Reports = new List<TroubleRegisterViewModel>();
            foreach (var item in data)
            {
                var model = _mapper.Map<TroubleRegisterViewModel>(item);
                Reports.Add(model);
            }
            ViewBag.Teknisyenler = GeTechniciansWithName();
            return View(Reports);
        }
        [Authorize(Roles = "Admin,Operator")]
        public IActionResult SetTechnician(int id)
        {
            var data = _context.TroubleRegisters.FirstOrDefault(x => x.Id == id);
            var model = _mapper.Map<TroubleRegisterViewModel>(data);
            ViewBag.Technician = GetTechnicians();
            return View(model);
        }
    }
}
