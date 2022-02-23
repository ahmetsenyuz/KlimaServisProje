using System;
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

        public ServiceController(MyContext context, IMapper mapper, UserManager<ApplicationUser> userManager, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _emailSender = emailSender;
            GetTech();
        }

        public IActionResult Services()
        {
            return View();
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
            register.CreatedDate = DateTime.UtcNow;
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
        public IActionResult TechRecordDetail(int Id)
        {
            var model = new TroubleOperationViewModel()
            {
                TroubleId = Id
            };
            ViewBag.Ops = _context.OperationPrices.ToList();
            var FinishedOps = _context.TroubleOperations.Where(x => x.TroubleId == Id).ToList();
            var FinishedOpsView = new List<DropdownListInt>();
            foreach (var item in FinishedOps)
            {
                var op = _context.OperationPrices.FirstOrDefault(x => x.operationId == item.OperationId);
                FinishedOpsView.Add(new DropdownListInt()
                {
                    Id = op.operationId,
                    Name = op.operationName
                });
            }

            ViewBag.AddedOp = FinishedOpsView;
            return View(model);
        }
        [Authorize(Roles = "Technician")]
        [HttpPost]
        public IActionResult TechRecordDetail(TroubleOperationViewModel model)
        {
            var data = new TroubleOperation()
            {
                TroubleId = model.TroubleId,
                OperationId = model.OperationId,
                Price = model.Price
            };
            try
            {
                _context.TroubleOperations.Add(data);
                _context.SaveChanges();
                return RedirectToAction("TechRecords");
            }
            catch (Exception e)
            {
            }
            return View(model);

        }

    }
}
