using API.Dtos;
using API.Models;
using API.Ultils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services.Impl
{
    public class BedService : IBedService
    {
        private readonly SenShineSpaContext _context;
        private readonly IMapper _mapper;
        public BedService(SenShineSpaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<Bed> AddBed(Bed bed)
        {
            // Check if the BedNumber already exists in the same room
            var existingBed = await _context.Beds
                .FirstOrDefaultAsync(b => b.RoomId == bed.RoomId && b.BedNumber == bed.BedNumber);
            if (existingBed != null)
            {
                throw new InvalidOperationException("A bed with the same BedNumber already exists in the same room.");
            }

            await _context.Beds.AddAsync(bed);
            await _context.SaveChangesAsync();
            return bed;
        }

        public async Task<BedDTO> UpdateBedAsync(int id, BedDTO bedDTO)
        {
            var bed = await _context.Beds
                                    .FirstOrDefaultAsync(b => b.Id == id);

            if (bed == null)
            {
                return null;
            }

            // Check if the BedNumber already exists in the same room (but exclude the current bed)
            var existingBed = await _context.Beds
                .FirstOrDefaultAsync(b => b.RoomId == bedDTO.RoomId && b.BedNumber == bedDTO.BedNumber && b.Id != id);
            if (existingBed != null)
            {
                throw new InvalidOperationException("A bed with the same BedNumber already exists in the same room.");
            }

            // Update bed properties
            bed.RoomId = bedDTO.RoomId;
            bed.BedNumber = bedDTO.BedNumber;
            bed.StatusWorking = bedDTO.StatusWorking;

            // Save changes to the database
            _context.Beds.Update(bed);
            await _context.SaveChangesAsync();

            return new BedDTO
            {
                Id = bed.Id,
                RoomId = bed.RoomId,
                BedNumber = bed.BedNumber,
                StatusWorking = bed.StatusWorking
            };
        }

        public async Task<bool> DeleteBed(int id)
        {
            var bed = await _context.Beds.FindAsync(id);
            if (bed == null)
            {
                return false;
            }

            _context.Beds.Remove(bed);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<BedDTO> GetBedById(int id)
        {
            var bed = await _context.Beds
                                    .Include(b => b.Room)
                                    .FirstOrDefaultAsync(b => b.Id == id);

            if (bed == null)
            {
                return null;
            }

            return new BedDTO
            {
                Id = bed.Id,
                RoomId = bed.RoomId,
                BedNumber = bed.BedNumber,
                StatusWorking = bed.StatusWorking,
                
            };
        }

        public async Task<IEnumerable<BedDTO>> GetAllBeds()
        {
            return await _context.Beds
                                 .Include(b => b.Room)
                                 .Select(b => new BedDTO
                                 {
                                     Id = b.Id,
                                     RoomId = b.RoomId,
                                     BedNumber = b.BedNumber,
                                     StatusWorking = b.StatusWorking,
                                     
                                 })
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Bed>> GetBedByRoomId(int roomId)
        {
            return await _context.Beds.Include(b => b.Room)  
                                 .Where(b => b.RoomId == roomId)  
                                 .ToListAsync();
        }

        public async Task<PaginatedList<BedDTO>> GetBedList(int pageIndex = 1, int pageSize = 10, string? searchTerm = null)
        {
            // Tạo query cơ bản
            IQueryable<Bed> query = _context.Beds.Include(b => b.Room); 

            // Nếu có searchTerm, thêm điều kiện tìm kiếm vào query
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.BedNumber.Contains(searchTerm));
            }

            // Đếm tổng số bản ghi để tính tổng số trang
            var count = await query.CountAsync();

            // Lấy danh sách với phân trang
            var beds = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            var bedDtos = _mapper.Map<IEnumerable<BedDTO>>(beds);
            return new PaginatedList<BedDTO>
            {
                Items = bedDtos,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = count,
            };
        }
    }
}
