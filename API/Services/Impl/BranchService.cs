using API.Dtos;
using API.Models;
using API.Ultils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Impl
{
    public class BranchService : IBranchService
    {
        private readonly SenShineSpaContext _context;
        private readonly IMapper _mapper;

        public BranchService(SenShineSpaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Spa> CreateBranch(Spa branch)
        {
            try
            {
                await _context.Spas.AddAsync(branch);
                await _context.SaveChangesAsync();

                return branch;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error creating branch.", ex);
            }
        }

        public List<Spa> GetAllBranches()
        {
            try
            {
                return _context.Spas.ToList();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error retrieving branches.", ex);
            }
        }

        public Spa GetBranch(int id)
        {
            try
            {
                var branchs = GetAllBranches();
                return branchs.FirstOrDefault(b => b.Id == id);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error retrieving branch with ID {id}.", ex);
            }
        }

        public async Task<Spa> UpdateBranch(int id, Spa branch)
        {
            try
            {
                var branchUpdate = await _context.Spas.FirstOrDefaultAsync(s => s.Id == id);
                if (branchUpdate != null)
                {
                    branchUpdate.SpaName = branch.SpaName;
                    branchUpdate.ProvinceCode = branch.ProvinceCode;
                    branchUpdate.DistrictCode = branch.DistrictCode;
                    branchUpdate.WardCode = branch.WardCode;
                    await _context.SaveChangesAsync();
                }

                return branchUpdate;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error updating branch with ID {id}.", ex);
            }
        }

        public async Task<bool> DeleteBranch(int id)
        {
            try
            {
                var branch = await _context.Spas.FindAsync(id);
                if (branch == null)
                {
                    return false;
                }
                _context.Spas.Remove(branch);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error deleting branch with ID {id}.", ex);
            }
        }

        public bool BranchExist(int id)
        {
            try
            {
                return _context.Spas.Any(s => s.Id == id);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error checking existence of branch with ID {id}.", ex);
            }
        }

        public int? GetBranchIdByUserId(int id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(s => s.Id == id);
                return user?.SpaId;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error retrieving branch ID for user with ID {id}.", ex);
            }
        }

        public async Task<PaginatedList<BranchDTO>> GetBranches(int pageIndex, int pageSize, string searchTerm)
        {
            // Tạo query cơ bản
            IQueryable<Spa> query = _context.Spas;

            // Nếu có searchTerm, thêm điều kiện tìm kiếm vào query
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(b => b.SpaName.Contains(searchTerm));
            }

            // Đếm tổng số bản ghi để tính tổng số trang
            var count = await query.CountAsync();

            // Lấy danh sách với phân trang
            var branches = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
            var branchesDtos = _mapper.Map<IEnumerable<BranchDTO>>(branches);

            return new PaginatedList<BranchDTO>
            {
                Items = branchesDtos,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = count,
            };
        }
    }
}
