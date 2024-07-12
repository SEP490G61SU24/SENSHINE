using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/wards")]
    public class WardController : ControllerBase
    {
        private readonly IWardService _wardService;

        public WardController(IWardService wardService)
        {
            _wardService = wardService;
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetWardByCode(string code)
        {
            var ward = await _wardService.GetWardByCode(code);
            if (ward == null)
            {
                return NotFound();
            }

            return Ok(ward);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWards()
        {
            var wards = await _wardService.GetAllWards();
            return Ok(wards);
        }
    }
}
