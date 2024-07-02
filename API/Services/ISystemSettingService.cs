using API.Models;

namespace API.Services
{
    public interface ISystemSettingService
    {
        Task<SystemSetting> AddSystemSetting(string key, string value, string? description = null);
        Task<SystemSetting> UpdateSystemSetting(int id, string value, string? description = null);
        Task<bool> DeleteSystemSetting(int id);
        Task<SystemSetting> GetSystemSettingById(int id);
        Task<IEnumerable<SystemSetting>> GetAllSystemSettings();
    }
}
