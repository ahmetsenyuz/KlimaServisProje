using DevExtreme.AspNet.Data;
using KlimaServisProje.Core.ViewModels;
using KlimaServisProje.Data;
using KlimaServisProje.Extensions;
using KlimaServisProje.Models;
using KlimaServisProje.Models.Identity;
using KlimaServisProje.Services;
using KlimaServisProje.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

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
            if (data == null)
                return BadRequest(ModelState.ToFullErrorString());
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }

        private List<OperatorServiceViewModel> GetOperations()
        {
            var model = _myContext.TroubleRegisters.ToList();
            if (!TryValidateModel(model))
                return null;
            var data = new List<OperatorServiceViewModel>();
            foreach (var item in model.Where(x => x.TechnicianId == null))
            {
                try
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
                catch (Exception e)
                {
                    return null;
                }
            }
            return data;
        }

        [HttpPut]
        public async Task<IActionResult> Update(string key, string values)
        {
            var data = GetOperations();
            if (!TryValidateModel(data))
                return BadRequest(ModelState.ToFullErrorString());
            var model = data.FirstOrDefault(x => x.Id.ToString() == key);
            JsonConvert.PopulateObject(values, model);
            if (!TryValidateModel(model))
                return BadRequest(ModelState.ToFullErrorString());
            var operation = _myContext.TroubleRegisters.FirstOrDefault(x => x.Id == model.Id);
            if (!TryValidateModel(operation))
                return BadRequest(ModelState.ToFullErrorString());
            operation.TechnicianId = model.TechId;
            var tech = _myContext.TechniciansStatus.FirstOrDefault(x => x.TechnicianId == model.TechId);
            if (!TryValidateModel(tech))
                return BadRequest(ModelState.ToFullErrorString());
            tech.Status = true;
            operation.StartedDate = DateTime.UtcNow;
            try
            {
                _myContext.TroubleRegisters.Update(operation);
                var result = await _myContext.SaveChangesAsync();
                _myContext.TechniciansStatus.Update(tech);
                var result1 = await _myContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            var user = _userManager.Users.FirstOrDefault(x => x.Id == tech.TechnicianId);
            if (!TryValidateModel(user))
                return BadRequest(ModelState.ToFullErrorString());
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action("TechnicianService", "Service", new
            {
                Area = "Admin"
            }, protocol: Request.Scheme);

            var emailMessage = new EmailMessage()
            {
                Contacts = new string[] { user.Email },
                Subject = "İş atandı",
                Body = $"Lünfen Linke Tıklayarak Mevcut işinize bakınız <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Buraya Tıkla</a>"
            };
            await _emailSender.SendAsync(emailMessage);
            return Ok(new JsonResponseViewModel());
        }
    }
}
