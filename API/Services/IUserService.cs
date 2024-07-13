using API.Dtos;
using API.Models;

namespace API.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<User> AddUser(string? username = null, string? phone = null, string? password = null, string? firstName = null, string? midName = null, string? lastName = null, DateTime? birthDate = null, string? provinceCode = null, string? districtCode = null, string? wardCode = null, int? roleId = null);
        Task<User> UpdateUser(int id, UserDto userDto);
        Task<bool> DeleteUser(int id);
        Task<IEnumerable<UserDto>> GetAll();
        Task<UserDto> GetById(int id);
        Task<User> GetByUserName(string username);
        Task<IEnumerable<User>> GetUsersByRole(int roleId);
    }
}
