using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Impl
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly SenShineSpaContext _context;

        public SystemSettingService(SenShineSpaContext context)
        {
            _context = context;
        }

        public async Task<SystemSetting> AddSystemSetting(string key, string value, string? description = null)
        {
            var existingSetting = await _context.SystemSettings.SingleOrDefaultAsync(s => s.Key == key);
            if (existingSetting != null)
            {
                throw new ArgumentException("Setting with this key already exists.");
            }

            var systemSetting = new SystemSetting
            {
                Key = key,
                Value = value,
                Description = description
            };

            _context.SystemSettings.Add(systemSetting);
            await _context.SaveChangesAsync();
            return systemSetting;
        }

        public async Task<SystemSetting> UpdateSystemSetting(int id, string value, string? description = null)
        {
            var systemSetting = await _context.SystemSettings.FindAsync(id);
            if (systemSetting == null)
            {
                return null;
            }

            systemSetting.Value = value;
            systemSetting.Description = description;

            _context.SystemSettings.Update(systemSetting);
            await _context.SaveChangesAsync();
            return systemSetting;
        }

        public async Task<bool> DeleteSystemSetting(int id)
        {
            var systemSetting = await _context.SystemSettings.FindAsync(id);
            if (systemSetting == null)
            {
                return false;
            }

            _context.SystemSettings.Remove(systemSetting);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<SystemSetting> GetSystemSettingById(int id)
        {
            return await _context.SystemSettings.FindAsync(id);
        }

        public async Task<IEnumerable<SystemSetting>> GetAllSystemSettings()
        {
            return await _context.SystemSettings.ToListAsync();
        }
    }
}
