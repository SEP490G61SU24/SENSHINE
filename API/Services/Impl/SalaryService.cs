using API.Dtos;
using API.Models;
using Microsoft.EntityFrameworkCore;

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
            await _context.Salaries.AddAsync(salary);
            await _context.SaveChangesAsync();

            return salary;
        }

        public ICollection<Salary> GetSalaries()
        {
            return _context.Salaries.ToList();
        }

        public ICollection<Salary> GetSalariesByMonthAndYear(int month, int year)
        {
            return _context.Salaries.Where(s => s.SalaryMonth == month && s.SalaryYear == year).ToList();
        }

        public Salary GetSalary(int id)
        {
            var salaries = GetSalaries();
            return salaries.FirstOrDefault(b => b.Id == id);
        }
        public async Task<Salary> UpdateSalary(int id, Salary salary)
        {
            var salaryUpdate = await _context.Salaries.FirstOrDefaultAsync(s => s.Id == id);
            salaryUpdate.BaseSalary = salary.BaseSalary;
            salaryUpdate.Allowances = salary.Allowances;
            salaryUpdate.Bonus = salary.Bonus;
            salaryUpdate.Deductions = salary.Deductions;
            salaryUpdate.TotalSalary = salary.TotalSalary;
            salaryUpdate.SalaryMonth = salary.SalaryMonth;
            salaryUpdate.SalaryYear = salary.SalaryYear;
            await _context.SaveChangesAsync();

            return salaryUpdate;
        }

        public async Task<bool> DeleteSalary(int id)
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

        public bool SalaryExist(int id)
        {
            return _context.Salaries.Any(s => s.Id == id);
        }
    }
}
