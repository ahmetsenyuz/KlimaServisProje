using System.Collections.Generic;
using System.Linq;
using DevExtreme.AspNet.Data;
using KlimaServisProje.Data;
using KlimaServisProje.Extensions;
using KlimaServisProje.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KlimaServisProje.Areas.Admin.Controllers.Apis
{
    [Route("api/[controller]/[action]")]
    public class ServiceApiController : ControllerBase
    {
        private readonly MyContext _myContext;

        public ServiceApiController(MyContext myContext)
        {
            _myContext = myContext;
        }

        [HttpGet]
        public IActionResult Get(DataSourceLoadOptions loadOptions)
        {
            var model = _myContext.TroubleRegisters.ToList();
            var data = new List<OperatorServiceViewModel>();
            foreach (var item in model.Where(x=> x.TechnicianId == null))
            {
                var user = _myContext.Users.FirstOrDefault(x => x.Id == item.UserId);
                 data.Add(new OperatorServiceViewModel()
                 {
                     Id = item.Id,
                     UserName = user.Name+" "+user.Surname,
                     Type = item.ACModel+"/"+item.ACType+"/"+item.Capacity+"/"+item.GasType,
                     Description = item.Description
                 });
            }
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }
    }
}
