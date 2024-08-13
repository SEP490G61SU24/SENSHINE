using API.Dtos;
using API.Models;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SalaryController : Controller
    {
        private readonly ISalaryService _salaryService;
        private readonly IMapper _mapper;

        public SalaryController(ISalaryService salaryService, IMapper mapper)
        {
            _salaryService = salaryService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SalaryDTO salaryDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var salaryMap = _mapper.Map<Salary>(salaryDTO);
                if (await _salaryService.SalaryExistByEMY(salaryMap))
                {
                    return StatusCode(500, $"This employee already has a salary record for {salaryMap.SalaryMonth}/{salaryMap.SalaryYear}.");
                }
                else
                {
                    var createdSalary = await _salaryService.CreateSalary(salaryMap);
                    return Ok("Salary created successfully.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the salary: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var salaries = _mapper.Map<List<SalaryDTO>>(_salaryService.GetSalaries());
                return Ok(salaries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving salaries: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (!_salaryService.SalaryExist(id))
                    return NotFound("Salary not found.");

                var salary = _mapper.Map<SalaryDTO>(_salaryService.GetSalary(id));
                return Ok(salary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the salary: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetByMonthYear(int month, int year)
        {
            try
            {
                var salaries = _mapper.Map<List<SalaryDTO>>(_salaryService.GetSalariesByMonthAndYear(month, year));
                return Ok(salaries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving salaries: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] SalaryDTO salaryDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (!_salaryService.SalaryExist(id))
                    return NotFound("Salary not found.");

                var existingSalary = _salaryService.GetSalary(id);
                existingSalary.BaseSalary = salaryDTO.BaseSalary;
                existingSalary.Allowances = salaryDTO.Allowances;
                existingSalary.Bonus = salaryDTO.Bonus;
                existingSalary.Deductions = salaryDTO.Deductions;
                existingSalary.TotalSalary = salaryDTO.TotalSalary;
                existingSalary.SalaryMonth = salaryDTO.SalaryMonth;
                existingSalary.SalaryYear = salaryDTO.SalaryYear;

                var salaryUpdate = await _salaryService.UpdateSalary(id, existingSalary);

                if (salaryUpdate == null)
                {
                    return NotFound("Failed to update salary.");
                }

                return Ok("Salary updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the salary: {ex.Message}");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _salaryService.DeleteSalary(id);
                if (!result)
                {
                    return NotFound("Salary not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the salary: {ex.Message}");
            }
        }
    }
}
