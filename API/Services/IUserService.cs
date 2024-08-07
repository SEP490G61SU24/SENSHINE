using API.Dtos;
using API.Models;

namespace API.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<User> AddUser(UserDTO userDto);
        Task<User> UpdateUser(int id, UserDTO userDto);
        Task<bool> DeleteUser(int id);
        Task<IEnumerable<UserDTO>> GetAll();
        Task<UserDTO> GetById(int id);
        Task<User> GetByUserName(string username);
        Task<IEnumerable<User>> GetUsersByRole(int roleId);
        Task<string> GetAddress(string wardCode, string districtCode, string provinceCode);
    }
}
