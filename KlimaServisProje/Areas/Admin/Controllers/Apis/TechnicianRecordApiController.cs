using DevExtreme.AspNet.Data;
using KlimaServisProje.Areas.Admin.ViewModels;
using KlimaServisProje.Data;
using KlimaServisProje.Extensions;
using KlimaServisProje.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace KlimaServisProje.Areas.Admin.Controllers.Apis
{
    [Route("api/[controller]/[action]")]
    public class TechnicianRecordApiController : ControllerBase
    {
        private readonly MyContext _context;

        public TechnicianRecordApiController(MyContext context, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get(DataSourceLoadOptions loadOptions)
        {
            var tech = _context.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            var data = _context.TroubleRegisters.Where(x => x.TechnicianId == tech.Id).ToList();
            if (data.Count() == 0)
            {
                return BadRequest("Servis Kaydınız Bulunamamıştır.");
            }
            var model = new List<TechnicianRecordsViewModel>();
            foreach (var item in data)
            {
                var user = _context.Users.FirstOrDefault(x => x.Id == item.UserId);
                model.Add(new TechnicianRecordsViewModel()
                {
                    Id = item.Id,
                    ACModel = item.ACModel,
                    ACType = item.ACType,
                    Capacity = item.Capacity,
                    StartedDate = item.StartedDate,
                    Address = item.Address,
                    Description = item.Description,
                    FinishDate = item.FinishedDate,
                    GasType = item.GasType,
                    Finished = item.Finished,
                    UsersName = user.Name+" "+user.Surname
                });
            }
            return Ok(DataSourceLoader.Load(model, loadOptions));
        }
    }
}
