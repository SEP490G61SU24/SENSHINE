using API.Dtos;
using API.Models;
using API.Ultils;

namespace API.Services
{
    public interface ISalaryService
    {
        Task<Salary> CreateSalary(Salary salary);
        Task<PaginatedList<SalaryDTO>> GetSalaries(int pageIndex, int pageSize, string searchTerm, string spaId);
        List<Salary> GetAllSalaries();
        ICollection<Salary> GetSalariesByMonthAndYear(int month, int year);
        Salary GetSalary(int id);
        Task<Salary> UpdateSalary(int id, Salary salary);
        Task<bool> SalaryExistByEMY(Salary salary);
        Task<bool> DeleteSalary(int id);
        bool SalaryExist(int id);
        Task<(IEnumerable<int> Months, IEnumerable<decimal> TotalSalaries)> GetMonthlySalariesForYear(int year);
    }
}
