using API.Dtos;
using API.Models;
using API.Services;
using API.Services.Impl;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        public EmployeeController(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeDTO employeeDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            List<Role> roleList = new List<Role>();

            foreach (int roleId in employeeDTO.RoleId)
            {
                var role = _employeeService.GetRole(roleId);
                roleList.Add(role);
            }

            try
            {
                var employeeMap = _mapper.Map<User>(employeeDTO);
                employeeMap.Roles = roleList;
                var createdEmployee = await _employeeService.CreateEmployee(employeeMap);

                return Ok($"Tạo nhân viên {createdEmployee.FirstName} thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra khi tạo nhân viên: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employees = _mapper.Map<List<EmployeeDTO>>(_employeeService.GetEmployees());

            return Ok(employees);
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            if (!_employeeService.EmployeeExist(id))
                return NotFound();

            var employee = _mapper.Map<EmployeeDTO>(_employeeService.GetEmployee(id));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(employee);
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeDTO employeeDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_employeeService.EmployeeExist(id))
                return NotFound();

            try
            {
                var existingEmployee = _employeeService.GetEmployee(id);
                existingEmployee.UserName = employeeDTO.UserName;
                existingEmployee.Password = employeeDTO.Password;
                existingEmployee.FirstName = employeeDTO.FirstName;
                existingEmployee.MidName = employeeDTO.MidName;
                existingEmployee.LastName = employeeDTO.LastName;
                existingEmployee.Phone = employeeDTO.Phone;
                existingEmployee.BirthDate = employeeDTO.BirthDate;
                existingEmployee.ProvinceCode = employeeDTO.ProvinceCode;
                existingEmployee.DistrictCode = employeeDTO.DistrictCode;
                existingEmployee.WardCode = employeeDTO.WardCode;
                List<Role> roleList = new List<Role>();

                foreach (int roleId in employeeDTO.RoleId)
                {
                    var role = _employeeService.GetRole(roleId);
                    roleList.Add(role);
                }

                existingEmployee.Roles = roleList;
                var employeeUpdate = await _employeeService.UpdateEmployee(id, existingEmployee);

                if (employeeUpdate == null)
                {
                    return NotFound("Không thể cập nhật nhân viên");
                }

                return Ok($"Cập nhật nhân viên {employeeUpdate.FirstName} thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra khi cập nhật nhân viên: {ex.Message}");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _employeeService.DeleteEmployee(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
