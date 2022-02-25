using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using KlimaServisProje.Core.ViewModels;
using KlimaServisProje.Data;
using KlimaServisProje.Extensions;
using KlimaServisProje.Models.ArizaKayit;
using KlimaServisProje.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KlimaServisProje.Areas.Admin.Controllers.Apis
{
    [Route("api/[controller]/[action]")]
    public class TechnicianServiceApiController : ControllerBase
    {
        private readonly MyContext _context;

        public TechnicianServiceApiController(MyContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get(DataSourceLoadOptions loadOptions)
        {
            var tech = _context.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            var regis = _context.TroubleRegisters.FirstOrDefault(x => x.TechnicianId == tech.Id && x.Finished == false);
            if (regis == null)
            {
                return BadRequest("Üzerinize Kayıt Bulunamadı");
            }
            var service = _context.TroubleOperations?.Where(x => x.TroubleId == regis.Id).ToList();
            if (service.Count() == 0)
            {
                return BadRequest("Üzerinize Servis Kayıt Bulunamadı");
            }
            var model = new List<TroubleOperationViewModel>();
            foreach (var item in service)
            {
                model.Add(new TroubleOperationViewModel()
                {
                    OperationId = item.OperationId,
                    Price = item.Price,
                    TroubleId = item.TroubleId
                });
            }
            return Ok(DataSourceLoader.Load(model, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Insert(string values)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            var reg = _context.TroubleRegisters.FirstOrDefault(x => x.TechnicianId == user.Id && x.Finished == false);
            if (reg == null)
            {
                return BadRequest("Güncel Servis Kaydınız Bulunamadı.");
            }
            var model = new TroubleOperationViewModel();
            JsonConvert.PopulateObject(values,model);
            if (!TryValidateModel(model))
                return BadRequest(ModelState.ToFullErrorString());
            var data = new TroubleOperation()
            {
                OperationId = model.OperationId,
                TroubleId = reg.Id,
                Price = model.Price
            };
            try
            {
                _context.TroubleOperations.Add(data);
                var result = await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok(new JsonResponseViewModel());
        }
    }
}
