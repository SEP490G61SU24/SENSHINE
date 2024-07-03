using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Impl
{
    public class WorkScheduleService : IWorkScheduleService
    {
        private readonly SenShineSpaContext _context;

        public WorkScheduleService(SenShineSpaContext context)
        {
            _context = context;
        }

        public async Task<WorkSchedule> AddWorkSchedule(int? employeeId, DateTime? startDateTime, DateTime? endDateTime, string? dayOfWeek)
        {
            var workSchedule = new WorkSchedule
            {
                EmployeeId = employeeId,
                StartDateTime = startDateTime,
                EndDateTime = endDateTime,
                DayOfWeek = dayOfWeek
            };

            _context.WorkSchedules.Add(workSchedule);
            await _context.SaveChangesAsync();
            return workSchedule;
        }

        public async Task<WorkSchedule> UpdateWorkSchedule(int id, int? employeeId, DateTime? startDateTime, DateTime? endDateTime, string? dayOfWeek)
        {
            var workSchedule = await _context.WorkSchedules.FindAsync(id);
            if (workSchedule == null)
            {
                return null;
            }

            workSchedule.EmployeeId = employeeId;
            workSchedule.StartDateTime = startDateTime;
            workSchedule.EndDateTime = endDateTime;
            workSchedule.DayOfWeek = dayOfWeek;

            _context.WorkSchedules.Update(workSchedule);
            await _context.SaveChangesAsync();
            return workSchedule;
        }

        public async Task<bool> DeleteWorkSchedule(int id)
        {
            var workSchedule = await _context.WorkSchedules.FindAsync(id);
            if (workSchedule == null)
            {
                return false;
            }

            _context.WorkSchedules.Remove(workSchedule);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<WorkSchedule> GetWorkScheduleById(int id)
        {
            return await _context.WorkSchedules
                                 .Include(ws => ws.Employee)
                                 .SingleOrDefaultAsync(ws => ws.Id == id);
        }

        public async Task<IEnumerable<WorkSchedule>> GetAllWorkSchedules()
        {
            return await _context.WorkSchedules
                                 .Include(ws => ws.Employee)
                                 .ToListAsync();
        }
    }
}
