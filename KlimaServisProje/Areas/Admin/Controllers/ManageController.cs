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

namespace KlimaServisProje.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ManageController : Controller
    {
        private readonly MyContext _myContext;

        public ManageController(MyContext myContext)
        {
            _myContext = myContext;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetUsers()
        {
            var list =(from users in _myContext.Users
                       join userroles in _myContext.UserRoles on users.Id equals userroles.UserId
                       join roles in _myContext.Roles on userroles.RoleId equals roles.Id
                       select new UsersViewModel
                       {
                           UserId = users.Id,
                           Username = users.UserName,
                           Email = users.Email,
                           Role = roles.Name
                       }).ToList();
            ViewBag.Roles = GetRoles();
            return View(list);
        }
        private List<SelectListItem> GetRoles()
        {
            var roles = _myContext.Roles.ToList();
            var roleList = new List<SelectListItem>();
            foreach (var rol in roles)
            {
                roleList.Add(new SelectListItem(rol.Name, rol.Id));
            }
            return roleList;
        }
        public IActionResult UserDetail(string id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var data = (from users in _myContext.Users where users.Id == id
                        join userroles in _myContext.UserRoles on users.Id equals userroles.UserId
                        join roles in _myContext.Roles on userroles.RoleId equals roles.Id
                        select new UsersViewModel
                        {
                            UserId = users.Id,
                            Username = users.UserName,
                            Email = users.Email,
                            Role = roles.Name
                        });
            {
                return View(data);
            }
        }
    }
}
