using System;
using KlimaServisProje.Areas.Admin.ViewModels;
using KlimaServisProje.Data;
using KlimaServisProje.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KlimaServisProje.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace KlimaServisProje.Areas.Admin.Controllers
{
    [Route("[area]/[controller]/[action]")]
    [Area("Admin")]
    [Authorize(Roles = "Admin,Operator,Technician")]
    public class ManageController : Controller
    {
        private readonly MyContext _myContext;

        public ManageController(MyContext myContext, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _myContext = myContext;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Users()
        {
            List<DropdownListItems> list = new List<DropdownListItems>();
            foreach (var item in _myContext.Roles)
            {
                list.Add(new DropdownListItems()
                {
                    Id = item.Id,
                    Name = item.Name
                });
            }
            ViewBag.Roles = list;
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Operations()
        {
            return View();
        }
    }
}