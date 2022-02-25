using System;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using KlimaServisProje.Core.ViewModels;
using KlimaServisProje.Data;
using KlimaServisProje.Extensions;
using KlimaServisProje.Models.ArizaKayit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KlimaServisProje.Areas.Admin.Controllers.Apis
{
    [Route("api/[controller]/[action]")]
    public class OperationApiController : ControllerBase
    {
        private readonly MyContext _myContext;

        public OperationApiController(MyContext myContext)
        {
            _myContext = myContext;
        }

        [HttpGet]
        public IActionResult Get(DataSourceLoadOptions loadOptions)
        {
            var model = _myContext.OperationPrices;
            if (!TryValidateModel(model))
                return BadRequest(ModelState.ToFullErrorString());
            return Ok(DataSourceLoader.Load(model, loadOptions));
        }

        [HttpPut]
        public async Task<IActionResult> Update(string key, string values)
        {
            var data = _myContext.OperationPrices.FirstOrDefault(x => x.operationId.ToString() == key);
            if (data == null)
                return StatusCode(StatusCodes.Status409Conflict, new JsonResponseViewModel()
                {
                    IsSuccess = false,
                    ErrorMessage = "İşlem Bulunamadı"
                });
            JsonConvert.PopulateObject(values,data);
            if (!TryValidateModel(data))
                return BadRequest(ModelState.ToFullErrorString());

            var result =  _myContext.OperationPrices.Update(data);
            await _myContext.SaveChangesAsync();
            return Ok(new JsonResponseViewModel());
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string key)
        {
            var data = _myContext.OperationPrices.FirstOrDefault(x => x.operationId.ToString() == key);
            if (data == null)
                return StatusCode(StatusCodes.Status409Conflict, new JsonResponseViewModel()
                {
                    IsSuccess = false,
                    ErrorMessage = "İşlem Bulunamadı"
                });
            try
            {
                var result = _myContext.OperationPrices.Remove(data);
                await _myContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest($"{data.operationName} Silinirken Bir Hata oluştu: {e.Message}");
            }
            return Ok(new JsonResponseViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(string values)
        {
            var newOp = new OperationPrice();
            JsonConvert.PopulateObject(values,newOp);
            if (!TryValidateModel(newOp))
                return BadRequest(ModelState.ToFullErrorString());
            try
            {
                var result = _myContext.OperationPrices.AddAsync(newOp);
                await _myContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest($"{newOp.operationName} Eklenirken Bir Hata oluştu: {e.Message}");
            }
            return Ok(new JsonResponseViewModel());
        }
    }
}
