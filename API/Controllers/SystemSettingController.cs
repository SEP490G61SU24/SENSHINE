using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/systemsettings")]
    public class SystemSettingController : ControllerBase
    {
        private readonly ISystemSettingService _systemSettingService;

        public SystemSettingController(ISystemSettingService systemSettingService)
        {
            _systemSettingService = systemSettingService;
        }

        [HttpPost]
        public async Task<IActionResult> AddSystemSetting(SystemSetting systemSetting)
        {
            try
            {
                var addedSetting = await _systemSettingService.AddSystemSetting(systemSetting.Key, systemSetting.Value, systemSetting.Description);
                return CreatedAtAction(nameof(GetSystemSettingById), new { id = addedSetting.Id }, addedSetting);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSystemSetting(int id, SystemSetting systemSetting)
        {
            var updatedSetting = await _systemSettingService.UpdateSystemSetting(id, systemSetting.Value, systemSetting.Description);
            if (updatedSetting == null)
            {
                return NotFound();
            }

            return Ok(updatedSetting);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSystemSetting(int id)
        {
            var success = await _systemSettingService.DeleteSystemSetting(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSystemSettingById(int id)
        {
            var systemSetting = await _systemSettingService.GetSystemSettingById(id);
            if (systemSetting == null)
            {
                return NotFound();
            }

            return Ok(systemSetting);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSystemSettings()
        {
            var systemSettings = await _systemSettingService.GetAllSystemSettings();
            return Ok(systemSettings);
        }
    }
}
