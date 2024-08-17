using API.Dtos;
using API.Models;

namespace API.Services
{
    public interface IEmployeeService
    {
        Task<User> CreateEmployee(User employee);
        ICollection<User> GetEmployees();
        ICollection<User> GetCustomer(string name);
        User GetEmployee(int id);
        Role GetRole(int id);
        Task<User> UpdateEmployee(int id, User employee);
        Task<bool> DeleteEmployee(int id);
        bool EmployeeExist(int id);
    }
}
