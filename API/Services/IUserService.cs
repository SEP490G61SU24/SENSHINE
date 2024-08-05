using API.Dtos;
using API.Models;

namespace API.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<User> AddUser(UserDto userDto);
        Task<User> UpdateUser(int id, UserDto userDto);
        Task<bool> DeleteUser(int id);
        Task<IEnumerable<UserDto>> GetAll();
        Task<UserDto> GetById(int id);
        Task<User> GetByUserName(string username);
        Task<IEnumerable<User>> GetUsersByRole(int roleId);
        Task<string> GetAddress(string wardCode, string districtCode, string provinceCode);
    }
}
