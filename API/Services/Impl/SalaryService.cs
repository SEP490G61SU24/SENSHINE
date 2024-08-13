using API.Dtos;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Services.Impl
{
    public class SalaryService : ISalaryService
    {
        private readonly SenShineSpaContext _context;

        public SalaryService(SenShineSpaContext context)
        {
            _context = context;
        }

        public async Task<Salary> CreateSalary(Salary salary)
        {
            try
            {
                await _context.Salaries.AddAsync(salary);
                await _context.SaveChangesAsync();

                return salary;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error creating salary.", ex);
            }
        }

        public async Task<bool> SalaryExistByEMY(Salary salary)
        {
            try
            {
                return _context.Salaries.Any(s => s.EmployeeId == salary.EmployeeId && s.SalaryMonth == salary.SalaryMonth && s.SalaryYear == salary.SalaryYear);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error checking salary existence by EmployeeId, Month, and Year.", ex);
            }
        }

        public ICollection<Salary> GetSalaries()
        {
            try
            {
                return _context.Salaries.ToList();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error retrieving salaries.", ex);
            }
        }

        public ICollection<Salary> GetSalariesByMonthAndYear(int month, int year)
        {
            try
            {
                return _context.Salaries.Where(s => s.SalaryMonth == month && s.SalaryYear == year).ToList();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error retrieving salaries for Month {month} and Year {year}.", ex);
            }
        }

        public Salary GetSalary(int id)
        {
            try
            {
                var salaries = GetSalaries();
                return salaries.FirstOrDefault(b => b.Id == id);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error retrieving salary with ID {id}.", ex);
            }
        }

        public async Task<Salary> UpdateSalary(int id, Salary salary)
        {
            try
            {
                var salaryUpdate = await _context.Salaries.FirstOrDefaultAsync(s => s.Id == id);
                if (salaryUpdate != null)
                {
                    salaryUpdate.BaseSalary = salary.BaseSalary;
                    salaryUpdate.Allowances = salary.Allowances;
                    salaryUpdate.Bonus = salary.Bonus;
                    salaryUpdate.Deductions = salary.Deductions;
                    salaryUpdate.TotalSalary = salary.TotalSalary;
                    salaryUpdate.SalaryMonth = salary.SalaryMonth;
                    salaryUpdate.SalaryYear = salary.SalaryYear;
                    await _context.SaveChangesAsync();
                }

                return salaryUpdate;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error updating salary with ID {id}.", ex);
            }
        }

        public async Task<bool> DeleteSalary(int id)
        {
            try
            {
                var salary = await _context.Salaries.FindAsync(id);
                if (salary == null)
                {
                    return false;
                }
                _context.Salaries.Remove(salary);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error deleting salary with ID {id}.", ex);
            }
        }

        public bool SalaryExist(int id)
        {
            try
            {
                return _context.Salaries.Any(s => s.Id == id);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error checking existence of salary with ID {id}.", ex);
            }
        }
    }
}
