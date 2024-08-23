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

            var emp = await _context.Users.FirstOrDefaultAsync(x => x.Id == workScheduleDto.EmployeeId);

            if (emp == null)
            {
                throw new InvalidOperationException("Không tìm thấy nhân viên.");
            }

            workSchedule.Employee = emp;

            _context.WorkSchedules.Add(workSchedule);
            await _context.SaveChangesAsync();

            return _mapper.Map<WorkScheduleDTO>(workSchedule);
        }

        public async Task<WorkScheduleDTO> UpdateWorkSchedule(int id, WorkScheduleDTO workScheduleDto)
        {
            var existingWorkSchedule = await _context.WorkSchedules.Include(ws => ws.Employee).FirstOrDefaultAsync(ws => ws.Id == id);
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

        public async Task<PaginatedList<WorkScheduleDTO>> GetWorkSchedules(int pageIndex, int pageSize, string searchTerm, string spaId)
        {
            // Tạo query cơ bản
            IQueryable<WorkSchedule> query = _context.WorkSchedules.Include(ws => ws.Employee);

            int? spaIdInt = spaId != null && spaId != "ALL"
             ? int.Parse(spaId)
             : (int?)null;

            if (spaIdInt.HasValue)
            {
                query = query.Where(w => w.Employee.SpaId == spaIdInt.Value);
            }

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

        public async Task<IEnumerable<WeekOptionDTO>> GetAvailableWeeks(int employeeId, int year)
        {
			//         // Lấy danh sách tất cả các lịch làm việc của nhân viên
			//         var workSchedules = await _context.WorkSchedules
			//       .Where(ws => ws.EmployeeId == employeeId && ws.StartDateTime.HasValue && ws.StartDateTime.Value.Year == year)
			//	.ToListAsync();

			//         var workSchedulesDTO = _mapper.Map<IEnumerable<WorkScheduleDTO>>(workSchedules);

			//var weekOptions = workSchedulesDTO
			//       .Select(ws => new WeekOptionDTO
			//       {
			//        Year = ws.StartDateTime.HasValue ? ws.StartDateTime.Value.Year : 0,
			//        WeekNumber = ws.StartDateTime.HasValue ? CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(ws.StartDateTime.Value, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) : 0,
			//        StartDate = ws.StartDateTime.HasValue
			//	        ? new DateTime(ws.StartDateTime.Value.Year, 1, 1)
			//		        .AddDays(7 * (CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(ws.StartDateTime.Value, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) - 1))
			//	        : DateTime.MinValue,
			//        EndDate = ws.EndDateTime.HasValue
			//	        ? new DateTime(ws.EndDateTime.Value.Year, 1, 1)
			//		        .AddDays(7 * (CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(ws.EndDateTime.Value, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) - 1))
			//		        .AddDays(6)
			//	        : DateTime.MinValue
			//       })
			//       .Distinct()
			//       .OrderBy(w => w.Year)
			//       .ThenBy(w => w.WeekNumber)
			//       .ToList();

			//if (!weekOptions.Any())
			//{
			//	var currentYear = year;
			//	var currentWeek = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

			//	var weekOptionsInCurrentYear = Enumerable.Range(currentWeek, 52 - currentWeek + 1)
			//		.Select(week => new WeekOptionDTO
			//		{
			//			Year = currentYear,
			//			WeekNumber = week,
			//			StartDate = new DateTime(currentYear, 1, 1)
			//				.AddDays(7 * (week - 1)),
			//			EndDate = new DateTime(currentYear, 1, 1)
			//				.AddDays(7 * week - 1)
			//		})
			//		.ToList();

			//	return weekOptionsInCurrentYear;
			//}

			//return weekOptions;

			var calendar = CultureInfo.InvariantCulture.Calendar;
			var startOfYear = new DateTime(year, 1, 1);
			var endOfYear = new DateTime(year, 12, 31);

			var weeks = new List<WeekOptionDTO>();
			var currentDate = startOfYear;

			while (currentDate.DayOfWeek != DayOfWeek.Monday)
			{
				currentDate = currentDate.AddDays(1);
			}

			while (currentDate <= endOfYear)
			{
				var weekNumber = calendar.GetWeekOfYear(currentDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
				var startOfWeek = currentDate;
				var endOfWeek = startOfWeek.AddDays(6);

				weeks.Add(new WeekOptionDTO
				{
					Year = year,
					WeekNumber = weekNumber,
					StartDate = startOfWeek,
					EndDate = endOfWeek
				});

				// Di chuyển đến tuần tiếp theo
				currentDate = endOfWeek.AddDays(1);
			}

			return weeks;
		}

        public async Task<IEnumerable<int>> GetAvailableYears(int employeeId)
        {
            var workSchedules = await _context.WorkSchedules
                .Where(ws => ws.EmployeeId == employeeId)
                .ToListAsync();

            var wsDtos = _mapper.Map<IEnumerable<WorkScheduleDTO>>(workSchedules);

            var years = wsDtos
               .Select(ws => ws.StartDateTime?.Year)
               .Where(year => year.HasValue)
               .Select(year => year.Value)
               .Distinct()
               .OrderBy(year => year)
               .ToList();

            var currentYear = DateTime.Now.Year;
            
            if (!years.Any())
            {
                years = new List<int>
                    {
                        currentYear - 1,
                        currentYear,
                        currentYear + 1,
                    };
            }
			else
			{
				var minYear = years.Min();

				var rangeYears = Enumerable.Range(minYear, currentYear - minYear + 1);

				var resultYears = rangeYears
					.Union(new List<int> { currentYear + 1 })
					.Distinct()
					.OrderBy(year => year)
					.ToList();

				years = resultYears;
			}

			return years;
        }
    }
}
