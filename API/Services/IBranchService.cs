using API.Models;

namespace API.Services
{
    public interface IBranchService
    {
        Task<Spa> CreateBranch(Spa branch);
        ICollection<Spa> GetBranchs();
        Spa GetBranch(int id);
        Task<Spa> UpdateBranch(int id, Spa branch);
        Task<bool> DeleteBranch(int id);
        bool BranchExist(int id);
    }
}
