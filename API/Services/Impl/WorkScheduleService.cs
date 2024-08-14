using AutoMapper;
using API.Dtos;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Ultils;

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

            _mapper.Map(workScheduleDto, existingWorkSchedule);
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
    }
}
