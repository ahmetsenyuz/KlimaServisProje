using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using KlimaServisProje.Core.ViewModels;
using KlimaServisProje.Data;
using KlimaServisProje.Extensions;
using KlimaServisProje.Models.Identity;
using KlimaServisProje.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KlimaServisProje.Areas.Admin.Controllers.Apis
{
    [Route("api/[controller]/[action]")]
    public class UserApiController : ControllerBase
    {
        private readonly MyContext _myContext;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserApiController(MyContext myContext, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _myContext = myContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Get(DataSourceLoadOptions loadOptions)
        {
            var model = GetUsersWithRoles();

            return Ok(DataSourceLoader.Load(model, loadOptions));
        }

        private List<UserRolesViewModel> GetUsersWithRoles()
        {
            var data = _userManager.Users.ToList();
            var model = new List<UserRolesViewModel>();
            foreach (var item in data)
            {
                var role = _myContext.UserRoles.First(x => x.UserId == item.Id);
                var roleName = _myContext.Roles.FirstOrDefault(x => x.Id == role.RoleId);
                model.Add(new UserRolesViewModel()
                {
                    UserId = item.Id,
                    Name = item.Name,
                    Surname = item.Surname,
                    UserName = item.UserName,
                    Email = item.Email,
                    RoleId = role.RoleId,
                    RoleName = roleName.Name
                });
            }

            return model;
        }

        [HttpPut]
        public async Task<IActionResult> Update(string key, string values)
        {
            var model = GetUsersWithRoles();
            var data = model.FirstOrDefault(x => x.UserId == key);
            var eskirole = _roleManager.Roles.FirstOrDefault(x => x.Id == data.RoleId);
            JsonConvert.PopulateObject(values,data);
            if (!TryValidateModel(data))
                return BadRequest(ModelState.ToFullErrorString());
            var user = _myContext.Users.FirstOrDefault(x => x.Id == data.UserId);
            user.Name = data.Name;
            user.Surname = data.Surname;
            user.UserName = data.UserName;
            user.Email = data.Email;
            var yenirole = _roleManager.Roles.FirstOrDefault(x => x.Id == data.RoleId);
            if (eskirole.Name != yenirole.Id)
            {
                var result1 = await _userManager.RemoveFromRoleAsync(user, eskirole.Name);
                var result2 = await _userManager.AddToRoleAsync(user, yenirole.Name);
            }

            try
            {
                var result = await _userManager.UpdateAsync(user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok(new JsonResponseViewModel());
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string key)
        {
            var data = _userManager.Users.FirstOrDefault(x => x.Id == key);
            if (data == null)
                return StatusCode(StatusCodes.Status409Conflict, new JsonResponseViewModel()
                {
                    IsSuccess = false,
                    ErrorMessage = "Kullanıcı Bulunamadı"
                });
            try
            {
                _myContext.Users.Remove(data);
                _myContext.SaveChanges();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok(new JsonResponseViewModel());
        }
    }
}
