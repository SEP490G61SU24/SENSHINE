using API.Dtos;
using API.Models;

namespace API.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<User> AddUser(string? username = null, string? password = null, string? firstName = null, string? midName = null, string? lastName = null, DateTime? birthDate = null, string? provinceCode = null, string? districtCode = null, string? wardCode = null);
        Task<User> UpdateUser(int id, UserUpdateDto userDto);
        Task<bool> DeleteUser(int id);
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(int id);
        Task<User> GetByUserName(string username);
        Task<IEnumerable<User>> GetUsersByRole(int roleId);
    }
}
