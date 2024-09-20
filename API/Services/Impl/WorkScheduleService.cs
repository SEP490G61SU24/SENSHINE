using AutoMapper;
using API.Dtos;
using API.Models;
using Microsoft.EntityFrameworkCore;
using API.Ultils;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Data;

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

        public async Task<PaginatedList<UserSlotDTO>> GetWorkSchedules(int pageIndex, int pageSize, string searchTerm, string spaId)
        {
            Role role = _context.Roles.FirstOrDefault(r => r.Id == 4);
            // Tạo query cơ bản
            IQueryable<UserSlot> query = _context.UserSlots.Include(u => u.User).Where(u => u.Status != "empty" && u.User.Roles.Contains(role));

            int? spaIdInt = spaId != null && spaId != "ALL"
             ? int.Parse(spaId)
             : (int?)null;

            if (spaIdInt.HasValue)
            {
                query = query.Where(u => u.User.SpaId == spaIdInt.Value);
            }

            // Nếu có searchTerm, thêm điều kiện tìm kiếm vào query
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.User.UserName.Contains(searchTerm) ||
                                         u.User.Phone.Contains(searchTerm) ||
                                         u.User.FirstName.Contains(searchTerm) ||
                                         u.User.LastName.Contains(searchTerm));
            }

            // Đếm tổng số bản ghi để tính tổng số trang
            var count = await query.CountAsync();

            // Lấy danh sách với phân trang
            var wsList = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var wsDtos = _mapper.Map<IEnumerable<UserSlotDTO>>(wsList);

            return new PaginatedList<UserSlotDTO>
            {
                Items = wsDtos,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = count,
            };
        }

        public async Task UpdateWorkSchedulesStatus(int id)
        {
            var existingUserSlot = await _context.UserSlots
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existingUserSlot == null)
            {
                throw new InvalidOperationException("Appointment not found.");
            }

            if (existingUserSlot.Status == "available")
            {
                existingUserSlot.Status = "empty";
            }

            _context.UserSlots.Update(existingUserSlot);
            await _context.SaveChangesAsync();
        }

        public async Task<UserSlot> AddWorkThisEmployee(int employeeId, int slotId, DateTime date)
        {
            var existingUserSlot = _context.UserSlots.FirstOrDefault(u => u.UserId == employeeId && u.SlotId == slotId && u.SlotDate == date);

            if (existingUserSlot == null)
            {
                UserSlotDTO userSlotDTO = new UserSlotDTO()
                {
                    UserId = employeeId,
                    SlotId = slotId,
                    SlotDate = date,
                    Status = "available"
                };

                var userSlot = _mapper.Map<UserSlot>(userSlotDTO);

                await _context.UserSlots.AddAsync(userSlot);
                await _context.SaveChangesAsync();
                return userSlot;
            }
            else
            {
                if (existingUserSlot.Status == "booked")
                {
                    existingUserSlot.Status = "booked";
                }
                else
                {
                    existingUserSlot.Status = "available";
                }

                await _context.SaveChangesAsync();
                return existingUserSlot;
            }
        }

        public async Task<string> GetStatusEmployeeInThisSlot(int employeeId, int slotId, DateTime date)
        {
            var thisUserSlot = _context.UserSlots.FirstOrDefault(u => u.UserId == employeeId && u.SlotId == slotId && u.SlotDate == date);

            if (thisUserSlot == null)
            {
                return "empty";
            }
            else
            {
                string status = thisUserSlot.Status;

                return status;
            }
        }
    }
}
