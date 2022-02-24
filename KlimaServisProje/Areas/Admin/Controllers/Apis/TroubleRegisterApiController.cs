using System.Collections.Generic;
using System.Linq;
using DevExtreme.AspNet.Data;
using KlimaServisProje.Data;
using KlimaServisProje.Extensions;
using KlimaServisProje.Models.Identity;
using KlimaServisProje.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KlimaServisProje.Areas.Admin.Controllers.Apis
{
    [Route("api/[controller]/[action]")]
    public class TroubleRegisterApiController : ControllerBase
    {
        private readonly MyContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TroubleRegisterApiController(MyContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Get(DataSourceLoadOptions loadOptions)
        {
            var data = _context.TroubleRegisters.ToList();
            var model = new List<AdminServiceViewModel>();
            foreach (var item in data)
            {
                var user = _userManager.Users?.FirstOrDefault(x => x.Id == item.UserId);
                var technician = _context.Users.FirstOrDefault(x => x.Id == item.TechnicianId);
                var fee = _context.TroubleOperations?.Where(x => x.TroubleId == item.Id).ToList();
                decimal total = 0.00m;
                foreach (var item1 in fee)
                {
                    total += item1.Price;
                }

                if (technician == null)
                {
                    technician = new ApplicationUser()
                    {
                        Name = "Teknisyen",
                        Surname = "Atanmamış"
                    };

                }
                model.Add(new AdminServiceViewModel()
                {
                    Id = item.Id,
                    AcInfo = item.ACModel+"/"+item.ACType+"/"+item.Capacity+"/"+item.GasType,
                    Address = item.Address,
                    Description = item.Description,
                    TechName = technician?.Name+" "+technician?.Surname,
                    FeeStatus = item.FeeStatus,
                    UserName = user.Name+" "+user.Surname,
                    Finished = item.Finished,
                    TotalFee = total
                });
            }
            return Ok(DataSourceLoader.Load(model, loadOptions));
        }
    }
}
