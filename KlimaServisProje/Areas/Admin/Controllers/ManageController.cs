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
using Microsoft.AspNetCore.Identity;

namespace KlimaServisProje.Areas.Admin.Controllers
{
    [Route("[area]/[controller]/[action]")]
    [Area("Admin")]
    [Authorize(Roles = "Admin,Operator,Technician")]
    public class ManageController : Controller
    {
        private readonly MyContext _myContext;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public ManageController(MyContext myContext, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _myContext = myContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
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
                           Name = users.Name,
                           Surname = users.Surname,
                           Role = new RoleViewModel()
                           {
                               RoleId = roles.Id,
                               RoleName = roles.Name
                           }
                       }).ToList();
            return View(list);
        }
        private List<RoleViewModel> GetRoles()
        {
            var roles = _myContext.Roles.ToList();
            var roleList = new List<RoleViewModel>();
            foreach (var rol in roles)
            {
                roleList.Add(new RoleViewModel()
                {
                    RoleId = rol.Id,
                    RoleName = rol.Name
                });
            }
            return roleList;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult UserDetail(string id)
        {
            #region MyRegion
            //if (id == null)
            //{
            //    return RedirectToAction(nameof(Index));
            //}
            //var data = (from users in _myContext.Users where users.Id == id
            //            join userroles in _myContext.UserRoles on users.Id equals userroles.UserId
            //            join roles in _myContext.Roles on userroles.RoleId equals roles.Id
            //            select new UsersViewModel
            //            {
            //                UserId = users.Id,
            //                Username = users.UserName,
            //                Email = users.Email,
            //                Role = roles.Name
            //            });
            //{
            //    return View(data);
            //}
            #endregion

            var model = _myContext.Users?.FirstOrDefault(x => x.Id == id);
            var role = _myContext.UserRoles?.FirstOrDefault(x => x.UserId == id);
            var rolename = _myContext.Roles?.FirstOrDefault(x => x.Id == role.RoleId);
            var data = new UsersViewModel()
            {
                UserId = id,
                Email = model.Email,
                Username = model.UserName,
                Name = model.Name,
                Surname = model.Surname,
                Role = new RoleViewModel()
                {
                    RoleId = role.RoleId,
                    RoleName = rolename.Name
                }
            };
            ViewBag.Rollist = GetRoles();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UserDetail(UsersViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(GetUsers));
            }
            var user = _myContext.Users.FirstOrDefault(x => x.Id == model.UserId);
            var role = _myContext.UserRoles.FirstOrDefault(x => x.UserId == model.UserId);
            var eskirolename = _myContext.Roles?.FirstOrDefault(x => x.Id == role.RoleId);
            var yenirolname = _myContext.Roles.FirstOrDefault(x => x.Id == model.roleName);
            try
            {
                user.UserName = model.Username;
                if (eskirolename != yenirolname)
                {
                    var result1 = await _userManager.RemoveFromRoleAsync(user, eskirolename.Name);
                    var result2 = await _userManager.AddToRoleAsync(user, yenirolname.Name);
                }
                _myContext.Users.Update(user);
                await _myContext.SaveChangesAsync();
                return RedirectToAction(nameof(UserDetail),new {user.Id});
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(GetUsers));
            }
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(string id)
        {
            var model = _myContext.Users?.FirstOrDefault(x => x.Id == id);
            try
            {
                var result = _myContext.Users.Remove(model);
                _myContext.SaveChanges();
                TempData["message"] = $"{model.UserName} kullanıcısı başarıyla silinmiştir.";
               return RedirectToAction(nameof(GetUsers));
            }
            catch (Exception e)
            {
                
            }

            TempData["message"] = $"{model.UserName} kullanıcısı silinemedi.";
            return RedirectToAction(nameof(GetUsers));
        }
    }
}