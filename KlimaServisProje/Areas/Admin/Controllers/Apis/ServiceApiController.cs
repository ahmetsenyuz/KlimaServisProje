using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using KlimaServisProje.Core.ViewModels;
using KlimaServisProje.Data;
using KlimaServisProje.Extensions;
using KlimaServisProje.Models;
using KlimaServisProje.Models.Identity;
using KlimaServisProje.Services;
using KlimaServisProje.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace KlimaServisProje.Areas.Admin.Controllers.Apis
{
    [Route("api/[controller]/[action]")]
    public class ServiceApiController : ControllerBase
    {
        private readonly MyContext _myContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        public ServiceApiController(MyContext myContext, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _myContext = myContext;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Get(DataSourceLoadOptions loadOptions)
        {
            var data = GetOperations();
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }

        private List<OperatorServiceViewModel> GetOperations()
        {
            var model = _myContext.TroubleRegisters.ToList();
            var data = new List<OperatorServiceViewModel>();
            foreach (var item in model.Where(x => x.TechnicianId == null))
            {
                var user = _myContext.Users.FirstOrDefault(x => x.Id == item.UserId);
                data.Add(new OperatorServiceViewModel()
                {
                    Id = item.Id,
                    UserName = user.Name + " " + user.Surname,
                    Type = item.ACModel + "/" + item.ACType + "/" + item.Capacity + "/" + item.GasType,
                    Description = item.Description
                });
            }
            return data;
        }

        [HttpPut]
        public async Task<IActionResult> Update(string key, string values)
        {
            var data = GetOperations();
            var model = data.FirstOrDefault(x => x.Id.ToString() == key);
            JsonConvert.PopulateObject(values,model);
            var operation = _myContext.TroubleRegisters.FirstOrDefault(x => x.Id == model.Id);
            operation.TechnicianId = model.TechId;
            var tech = _myContext.TechniciansStatus.FirstOrDefault(x => x.TechnicianId == model.TechId);
            tech.Status = true;
            operation.StartedDate = DateTime.UtcNow;
            _myContext.TroubleRegisters.Update(operation);
            var  result = await _myContext.SaveChangesAsync();
            _myContext.TechniciansStatus.Update(tech);
            var result1 = await _myContext.SaveChangesAsync();

            var user = _userManager.Users.FirstOrDefault(x => x.Id == tech.TechnicianId);

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
            return Ok(new JsonResponseViewModel());
        }
    }
}
