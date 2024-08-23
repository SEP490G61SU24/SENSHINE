using API.Dtos;
using API.Models;
using API.Ultils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;

namespace API.Services.Impl
{
    public class SalaryService : ISalaryService
    {
        private readonly SenShineSpaContext _context;
        private readonly IMapper _mapper;

        public SalaryService(SenShineSpaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

        public List<Salary> GetAllSalaries()
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
                var salaries = GetAllSalaries();
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

        public async Task<PaginatedList<SalaryDTO>> GetSalaries(int pageIndex, int pageSize, string searchTerm, string spaId)
        {
            // Tạo query cơ bản
            IQueryable<Salary> query = _context.Salaries.Include(e => e.Employee);

            int? spaIdInt = spaId != null && spaId != "ALL"
            ? int.Parse(spaId)
            : (int?)null;

            if (spaIdInt.HasValue)
            {
                query = query.Where(u => u.Employee.SpaId == spaIdInt.Value);
            }

            // Nếu có searchTerm, thêm điều kiện tìm kiếm vào query
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(s => s.Employee.FirstName.Contains(searchTerm) ||
                                         s.Employee.MidName.Contains(searchTerm) ||
                                         s.Employee.LastName.Contains(searchTerm) ||
                                         s.Employee.Phone.Contains(searchTerm));
            }

            // Đếm tổng số bản ghi để tính tổng số trang
            var count = await query.CountAsync();

            // Lấy danh sách với phân trang
            var salaries = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
            var salariesDtos = _mapper.Map<IEnumerable<SalaryDTO>>(salaries);

            return new PaginatedList<SalaryDTO>
            {
                Items = salariesDtos,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = count,
            };
        }
        public async Task<(IEnumerable<int> Months, IEnumerable<decimal> TotalSalaries)> GetMonthlySalariesForYear(int year)
        {
            // Initialize data for all 12 months
            var allMonths = Enumerable.Range(1, 12).ToList();
            var monthlySalaries = await _context.Salaries
                .Where(s => s.SalaryYear == year)
                .GroupBy(s => s.SalaryMonth)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalSalary = g.Sum(s => s.TotalSalary)
                })
                .ToListAsync();

            // Fill in the missing months with a salary of 0
            var salariesByMonth = allMonths
                .Select(month => new
                {
                    Month = month,
                    TotalSalary = monthlySalaries.FirstOrDefault(s => s.Month == month)?.TotalSalary ?? 0
                })
                .OrderBy(x => x.Month)
                .ToList();

            var months = salariesByMonth.Select(x => x.Month).ToArray();
            var totalSalaries = salariesByMonth.Select(x => x.TotalSalary).ToArray();

            return (months, totalSalaries);
        }


    }
}
