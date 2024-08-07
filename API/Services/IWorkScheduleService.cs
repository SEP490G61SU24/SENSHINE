using API.Dtos;
using API.Models;

namespace API.Services
{
    public interface IWorkScheduleService
    {
        Task<WorkScheduleDTO> AddWorkSchedule(WorkScheduleDTO workScheduleDto);
        Task<WorkScheduleDTO> UpdateWorkSchedule(int id, WorkScheduleDTO workScheduleDto);
        Task<bool> DeleteWorkSchedule(int id);
        Task<WorkScheduleDTO> GetWorkScheduleById(int id);
        Task<IEnumerable<WorkScheduleDTO>> GetAllWorkSchedules();
    }
}
