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

            var dropList = GetTechnicians();
            ViewBag.Techs = dropList;
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

        public IActionResult TroubleRegisters()
        {
            return View();
        }

        public IActionResult TechnicianRecords()
        {
            return View();
        }

        public IActionResult TechnicianService()
        {
            var data = _context.OperationPrices.ToList();
            List<DropdownListInt> ops = new List<DropdownListInt>();
            foreach (var item in data)
            {
                var op = _context.OperationPrices.FirstOrDefault(x => x.operationId == item.operationId);
                ops.Add(new DropdownListInt()
                {
                    Id = op.operationId,
                    Name = op.operationName
                });
            };
            var user = _context.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            var reg = _context.TroubleRegisters.FirstOrDefault(x => x.TechnicianId == user.Id && x.Finished == false);
            var model = _mapper.Map<TroubleRegisterViewModel>(reg);
            ViewBag.Ops = ops;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> TechnicianService(TroubleRegisterViewModel model)
        {
            var data = _context.TroubleRegisters.FirstOrDefault(x => x.Id == model.Id);
            data.Finished = true;
            data.FinishedDate = DateTime.UtcNow;
            var techstat = _context.TechniciansStatus.FirstOrDefault(x => x.TechnicianId == data.TechnicianId);
            techstat.Status = false;
            _context.TroubleRegisters.Update(data);
            var result1 = await _context.SaveChangesAsync();
            _context.TechniciansStatus.Update(techstat);
            var result2 = _context.SaveChangesAsync();
            return RedirectToAction(nameof(TechnicianRecords));
        }

    }
}
