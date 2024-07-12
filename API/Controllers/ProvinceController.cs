using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/provinces")]
    public class ProvinceController : ControllerBase
    {
        private readonly IProvinceService _provinceService;

        public ProvinceController(IProvinceService provinceService)
        {
            _provinceService = provinceService;
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetProvinceByCode(string code)
        {
            var province = await _provinceService.GetProvinceByCode(code);
            if (province == null)
            {
                return NotFound();
            }

            return Ok(province);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProvinces()
        {
            var provinces = await _provinceService.GetAllProvinces();
            return Ok(provinces);
        }
    }
}
