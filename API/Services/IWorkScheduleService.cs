using API.Models;

namespace API.Services
{
    public interface IWorkScheduleService
    {
        Task<WorkSchedule> AddWorkSchedule(int? employeeId, DateTime? startDateTime, DateTime? endDateTime, string? dayOfWeek);
        Task<WorkSchedule> UpdateWorkSchedule(int id, int? employeeId, DateTime? startDateTime, DateTime? endDateTime, string? dayOfWeek);
        Task<bool> DeleteWorkSchedule(int id);
        Task<WorkSchedule> GetWorkScheduleById(int id);
        Task<IEnumerable<WorkSchedule>> GetAllWorkSchedules();
    }
}
