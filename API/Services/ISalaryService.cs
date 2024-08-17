using API.Models;

namespace API.Services
{
    public interface ISalaryService
    {
        Task<Salary> CreateSalary(Salary salary);
        ICollection<Salary> GetSalaries();
        ICollection<Salary> GetSalariesByMonthAndYear(int month, int year);
        Salary GetSalary(int id);
        Task<Salary> UpdateSalary(int id, Salary salary);
        Task<bool> SalaryExistByEMY(Salary salary);
        Task<bool> DeleteSalary(int id);
        bool SalaryExist(int id);
    }
}
