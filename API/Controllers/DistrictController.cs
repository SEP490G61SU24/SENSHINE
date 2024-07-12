using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/districts")]
    public class DistrictController : ControllerBase
    {
        private readonly IDistrictService _districtService;

        public DistrictController(IDistrictService districtService)
        {
            _districtService = districtService;
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetDistrictByCode(string code)
        {
            var district = await _districtService.GetDistrictByCode(code);
            if (district == null)
            {
                return NotFound();
            }

            return Ok(district);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDistricts()
        {
            var districts = await _districtService.GetAllDistricts();
            return Ok(districts);
        }
    }
}
