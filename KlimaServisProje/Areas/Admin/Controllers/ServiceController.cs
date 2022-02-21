﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AutoMapper;
using KlimaServisProje.Data;
using KlimaServisProje.Models;
using KlimaServisProje.Models.ArizaKayit;
using KlimaServisProje.Models.Identity;
using KlimaServisProje.Services;
using KlimaServisProje.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace KlimaServisProje.Areas.Admin.Controllers
{
    [Route("[area]/[controller]/[action]")]
    [Area("Admin")]
    [Authorize(Roles = "Admin,Operator,Technician")]
    public class ServiceController : Controller
    {
        private readonly MyContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser>_userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ServiceController(MyContext context, IMapper mapper, UserManager<ApplicationUser> userManager, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
            GetTech();
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
        private void GetTech()
        {

            var allUsers = _userManager.Users.ToList();

            var techs = new List<ApplicationUser>();

            foreach (var user in allUsers)
            {
                if (_userManager.IsInRoleAsync(user, "Technician").Result)
                {
                    techs.Add(user);

                    if (!_context.TechniciansStatus.Any(x => x.TechnicianId == user.Id))
                    {
                        _context.TechniciansStatus.Add(new TechniciansStatu()
                        {
                            Name = user.Name+" "+user.Surname,
                            TechnicianId = user.Id
                        });
                    }
                }
            }

            _context.SaveChanges();
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

        public List<DropdownListItems> GetTechnicians()
        {
            var model = _context.TechniciansStatus.ToList();
            var list = new List<DropdownListItems>();
            foreach (var item in model.Where(x=> x.Status == false))
            {

                var technicians = new DropdownListItems()
                {
                    Id = item.TechnicianId,
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

            ViewBag.Teknisyenler = _context.TechniciansStatus.ToList();
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

        [HttpPost]
        public async Task<IActionResult> SetTechnician(TroubleRegisterViewModel model)
        {
            var tech = _context.TechniciansStatus.FirstOrDefault(x => x.TechnicianId == model.TechnicianId);
            tech.Status = true;
            var register = _context.TroubleRegisters.FirstOrDefault(x => x.Id == model.Id);
            register.TechnicianId = model.TechnicianId;
            register.StartedDate = DateTime.UtcNow;
            try
            {
                _context.TechniciansStatus.Update(tech);
                _context.TroubleRegisters.Update(register);
                _context.SaveChanges();
                var user = _userManager.Users.FirstOrDefault(x => x.Id == model.TechnicianId);

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Scheme);

                var emailMessage = new EmailMessage()
                {
                    Contacts = new string[] { user.Email },
                    Subject = "İş atandı",
                    Body = "Üzerinize iş kitlendi"
                };
                await _emailSender.SendAsync(emailMessage);
                return RedirectToAction(nameof(GetReports));
            }
            catch (Exception)
            {

            }
            return View(model);
        }
        [Authorize(Roles = "Technician")]
        public IActionResult TechRecords()
        {
            var data = _context.TroubleRegisters.Where(x => x.Technician.UserName == User.Identity.Name).ToList();
            var model = _mapper.Map<List<TroubleRegisterViewModel>>(data);
            return View(model);
        }
        [Authorize(Roles = "Technician")]
        [HttpPost]
        public IActionResult TechRecordDetail(TroubleOperationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ViewBag.Ops = _context.OperationPrices.ToList();
            ViewBag.AddedOp = _context.TroubleOperations.Where(x => x.TroubleId == model.TroubleId);
            var data = new TroubleOperation()
            {
                OperationId = model.OperationId,
                TroubleId = model.TroubleId,
                Price = model.Price
            };
            try
            {
                _context.TroubleOperations.Add(data);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                
            }
            return View(model);
        }

    }
}
