using API.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace API.Services.Impl
{
    public class EmployeeService : IEmployeeService
    {
        private readonly SenShineSpaContext _context;

        public EmployeeService(SenShineSpaContext context)
        {
            _context = context;
        }

        public async Task<User> CreateEmployee(User employee)
        {
            await _context.Users.AddAsync(employee);
            await _context.SaveChangesAsync();

            return employee;
        }

        public ICollection<User> GetEmployees()
        {
            var roleSTAFF = _context.Roles.FirstOrDefault(r => r.RoleName == "STAFF");
            var roleRECEPTIONIST = _context.Roles.FirstOrDefault(r => r.RoleName == "RECEPTIONIST");
            return _context.Users.Include(e => e.Roles).Where(e => e.Roles.Contains(roleSTAFF) || e.Roles.Contains(roleRECEPTIONIST)).ToList();
        }

        public User GetEmployee(int id)
        {
            var employees = GetEmployees();
            return employees.FirstOrDefault(e => e.Id == id);
        }

        public Role GetRole(int id)
        {
            return _context.Roles.Where(r => r.Id == id).FirstOrDefault();
        }

        public async Task<User> UpdateEmployee(int id, User employee)
        {
            var employeeUpdate = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            employeeUpdate.UserName = employee.UserName;
            employeeUpdate.Password = employee.Password;
            employeeUpdate.FirstName = employee.FirstName;
            employeeUpdate.MidName = employee.MidName;
            employeeUpdate.LastName = employee.LastName;
            employeeUpdate.Phone = employee.Phone;
            employeeUpdate.BirthDate = employee.BirthDate;
            employeeUpdate.ProvinceCode = employee.ProvinceCode;
            employeeUpdate.DistrictCode = employee.DistrictCode;
            employeeUpdate.WardCode = employee.WardCode;
            employeeUpdate.Roles = employee.Roles;
            await _context.SaveChangesAsync();

            return employeeUpdate;
        }

        public async Task<bool> DeleteEmployee(int id)
        {
            var employee = await _context.Users.FindAsync(id);
            if (employee == null)
            {
                return false;
            }
            _context.Users.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool EmployeeExist(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
