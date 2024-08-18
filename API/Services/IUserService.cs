using API.Dtos;
using API.Models;
using API.Ultils;

namespace API.Services
{
    public interface IUserService
    {
        Task<UserDTO> Authenticate(string username, string password);
        Task<UserDTO> AddUser(UserDTO userDto);
        Task<UserDTO> UpdateUser(int id, UserDTO userDto);
        Task<bool> DeleteUser(int id);
        Task<IEnumerable<UserDTO>> GetAll();
        Task<PaginatedList<UserDTO>> GetUsers(int pageIndex, int pageSize, string searchTerm);
        Task<UserDTO> GetById(int id);
        Task<UserDTO> GetByUserName(string username);
        Task<IEnumerable<UserDTO>> GetUsersByRole(int roleId);
        Task<PaginatedList<UserDTO>> GetUsersByRoleWithPage(int roleId, int pageIndex, int pageSize, string searchTerm);
        Task<string> GetAddress(string wardCode, string districtCode, string provinceCode);
        Task<bool> ChangePassword(string userName, string currentPassword, string newPassword, bool userChange);
	}
}
