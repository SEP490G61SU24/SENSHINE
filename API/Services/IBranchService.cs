using API.Dtos;
using API.Models;
using API.Ultils;

namespace API.Services
{
    public interface IBranchService
    {
        Task<Spa> CreateBranch(Spa branch);
        Task<PaginatedList<BranchDTO>> GetBranches(int pageIndex, int pageSize, string searchTerm);
        List<Spa> GetAllBranches();
        Spa GetBranch(int id);
        Task<Spa> UpdateBranch(int id, Spa branch);
        Task<bool> DeleteBranch(int id);
        bool BranchExist(int id);
        int? GetBranchIdByUserId(int id);
    }
}
