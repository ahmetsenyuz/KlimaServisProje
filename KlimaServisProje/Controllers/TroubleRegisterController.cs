using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using KlimaServisProje.Data;
using KlimaServisProje.Models.ArizaKayit;
using KlimaServisProje.Models.Identity;
using KlimaServisProje.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KlimaServisProje.Controllers
{
    [Authorize(Roles = "Admin,User,Technician,Operator")]
    public class TroubleRegisterController : Controller
    {
        private readonly MyContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public TroubleRegisterController(MyContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        public IActionResult Reports()
        {
            var result = _context.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            var data = _context.TroubleRegisters.Where(x => x.UserId == result.Id).ToList();
            List<TroubleRegisterViewModel> Reports = new List<TroubleRegisterViewModel>();
            foreach (var item in data)
            {
                var model = _mapper.Map<TroubleRegisterViewModel>(item);
                Reports.Add(model);
            }
            
            return View(Reports);
        }

        public IActionResult CreateReport()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateReport(TroubleRegisterViewModel model)
        {
            var result = _context.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            model.UserId = result.Id;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var data = new TroubleRegister()
            {
                ACModel = model.ACModel,
                ACType = model.ACType,
                Address = model.Address,
                Capacity = model.Capacity,
                Description = model.Description,
                GasType = model.GasType,
                UserId = model.UserId
            };
            try
            {
                _context.TroubleRegisters.Add(data);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                return View(model);
            }
            return RedirectToAction(nameof(Reports));
        }
    }
}
