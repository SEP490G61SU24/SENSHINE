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
                var createdSalary = await _salaryService.CreateSalary(salaryMap);

                return Ok($"Tạo thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra khi tạo: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var salarys = _mapper.Map<List<SalaryDTO>>(_salaryService.GetSalaries());

            return Ok(salarys);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            if (!_salaryService.SalaryExist(id))
                return NotFound();

            var salary = _mapper.Map<SalaryDTO>(_salaryService.GetSalary(id));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(salary);
        }

        [HttpGet]
        public async Task<IActionResult> GetByMonthYear(int month, int year)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var salarys = _mapper.Map<List<SalaryDTO>>(_salaryService.GetSalariesByMonthAndYear(month, year));

            return Ok(salarys);
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] SalaryDTO salaryDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_salaryService.SalaryExist(id))
                return NotFound();

            try
            {
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
                    return NotFound("Không thể cập nhật");
                }

                return Ok($"Cập nhật thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra khi cập nhật: {ex.Message}");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _salaryService.DeleteSalary(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
