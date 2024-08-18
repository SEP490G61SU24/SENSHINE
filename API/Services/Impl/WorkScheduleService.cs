using AutoMapper;
using API.Dtos;
using API.Models;
using Microsoft.EntityFrameworkCore;
using API.Ultils;
using System.Globalization;

namespace API.Services.Impl
{
    public class WorkScheduleService : IWorkScheduleService
    {
        private readonly SenShineSpaContext _context;
        private readonly IMapper _mapper;

        public WorkScheduleService(SenShineSpaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<WorkScheduleDTO> AddWorkSchedule(WorkScheduleDTO workScheduleDto)
        {
            var workSchedule = _mapper.Map<WorkSchedule>(workScheduleDto);
            _context.WorkSchedules.Add(workSchedule);
            await _context.SaveChangesAsync();

            return _mapper.Map<WorkScheduleDTO>(workSchedule);
        }

        public async Task<WorkScheduleDTO> UpdateWorkSchedule(int id, WorkScheduleDTO workScheduleDto)
        {
            var existingWorkSchedule = await _context.WorkSchedules.FindAsync(id);
            if (existingWorkSchedule == null)
            {
                return null;
            }

            var emp = await _context.Users.FirstOrDefaultAsync(x => x.Id == workScheduleDto.EmployeeId);
            
            if(emp == null)
            {
                throw new InvalidOperationException("Không tìm thấy nhân viên.");
            }

            _mapper.Map(workScheduleDto, existingWorkSchedule);
            
            existingWorkSchedule.Employee = emp;

            _context.WorkSchedules.Update(existingWorkSchedule);
            await _context.SaveChangesAsync();

            return _mapper.Map<WorkScheduleDTO>(existingWorkSchedule);
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

        public async Task<WorkScheduleDTO> GetWorkScheduleById(int id)
        {
            var workSchedule = await _context.WorkSchedules
                .Include(ws => ws.Employee)
                .FirstOrDefaultAsync(ws => ws.Id == id);

            return workSchedule != null ? _mapper.Map<WorkScheduleDTO>(workSchedule) : null;
        }

        public async Task<IEnumerable<WorkScheduleDTO>> GetAllWorkSchedules()
        {
            var workSchedules = await _context.WorkSchedules
                .Include(ws => ws.Employee)
                .ToListAsync();

            return _mapper.Map<IEnumerable<WorkScheduleDTO>>(workSchedules);
        }

        public async Task<PaginatedList<WorkScheduleDTO>> GetWorkSchedules(int pageIndex, int pageSize, string searchTerm)
        {
            // Tạo query cơ bản
            IQueryable<WorkSchedule> query = _context.WorkSchedules.Include(ws => ws.Employee);

            // Nếu có searchTerm, thêm điều kiện tìm kiếm vào query
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.Employee.UserName.Contains(searchTerm) ||
                                         u.Employee.Phone.Contains(searchTerm) ||
                                         u.Employee.FirstName.Contains(searchTerm) ||
                                         u.Employee.LastName.Contains(searchTerm));
            }

            // Đếm tổng số bản ghi để tính tổng số trang
            var count = await query.CountAsync();

            // Lấy danh sách với phân trang
            var wsList = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var wsDtos = _mapper.Map<IEnumerable<WorkScheduleDTO>>(wsList);

            return new PaginatedList<WorkScheduleDTO>
            {
                Items = wsDtos,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = count,
            };
        }

        public async Task<IEnumerable<WorkScheduleDTO>> GetWorkSchedulesByWeek(int employeeId, DateTime startDate, DateTime endDate)
        {
            var workSchedules = await _context.WorkSchedules
                .Include(ws => ws.Employee)
                .Where(ws => ws.EmployeeId == employeeId &&
                (ws.StartDateTime <= endDate && ws.EndDateTime >= startDate))
                .ToListAsync();

            return _mapper.Map<IEnumerable<WorkScheduleDTO>>(workSchedules);
        }

		public async Task<IEnumerable<WeekOptionDTO>> GetAvailableWeeks(int employeeId)
		{
			var workSchedules = await _context.WorkSchedules.Where(ws => ws.EmployeeId == employeeId).ToListAsync();
            var workSchedulesDTO = _mapper.Map<IEnumerable<WorkScheduleDTO>>(workSchedules);

			// Chuyển đổi dữ liệu thành dạng yêu cầu
			var weekOptions = workSchedulesDTO
				.Select(ws => new WeekOptionDTO
				{
					Year = ws.StartDateTime.HasValue ? ws.StartDateTime.Value.Year : 0,
					WeekNumber = ws.StartDateTime.HasValue ? CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(ws.StartDateTime.Value, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) : 0,
					StartDate = ws.StartDateTime.HasValue
						? new DateTime(ws.StartDateTime.Value.Year, 1, 1)
							.AddDays(7 * (CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(ws.StartDateTime.Value, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) - 1))
						: DateTime.MinValue,
					EndDate = ws.EndDateTime.HasValue
						? new DateTime(ws.EndDateTime.Value.Year, 1, 1)
							.AddDays(7 * (CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(ws.EndDateTime.Value, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) - 1))
							.AddDays(6)
						: DateTime.MinValue
				})
				.Distinct()
				.OrderBy(w => w.Year)
				.ThenBy(w => w.WeekNumber)
				.ToList();

			return weekOptions;
		}

	}
}
