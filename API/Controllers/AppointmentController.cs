using API.Dtos;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            try
            {
                var appointments = await _appointmentService.GetAllAppointmentsAsync();
                var appointmentDTOs = appointments.Select(a => new AppointmentDTO
                {
                    Id = a.Id,
                    CustomerId = a.CustomerId,
                    EmployeeId = a.EmployeeId,
                    AppointmentDate = a.AppointmentDate,
                    Status = a.Status ? "true" : "false",
                    CustomerName = $"{a.Customer.FirstName} {a.Customer.LastName}",
                    EmployeeName = $"{a.Employee.FirstName} {a.Employee.LastName}",
                    ServiceName = string.Join(", ", a.Services.Select(s => s.ServiceName))
                }).ToList();
                return Ok(appointmentDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving appointments: {ex.Message}");
            }
        }

        [HttpGet("{appointmentDate}")]
        public async Task<IActionResult> GetAppointmentsByDate(DateTime appointmentDate)
        {
            var appointments = await _appointmentService.GetAppointmentsByDateAsync(appointmentDate);
            if (appointments == null || appointments.Count == 0)
            {
                return NotFound("Appointments not found for the specified date");
            }

            var appointmentDTOs = appointments.Select(a => new AppointmentDTO
            {
                Id = a.Id,
                CustomerId = a.CustomerId,
                EmployeeId = a.EmployeeId,
                AppointmentDate = a.AppointmentDate,
                Status = a.Status ? "true" : "false",
                CustomerName = $"{a.Customer.FirstName} {a.Customer.LastName}",
                EmployeeName = $"{a.Employee.FirstName} {a.Employee.LastName}",
                ServiceName = string.Join(", ", a.Services.Select(s => s.ServiceName))
            }).ToList();

            return Ok(appointmentDTOs);
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetAppointmentsByCustomerId(int customerId)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByCustomerIdAsync(customerId);
                if (appointments == null || appointments.Count == 0)
                {
                    return NotFound("No appointments found for this customer.");
                }
                var appointmentDTOs = appointments.Select(a => new AppointmentDTO
                {
                    Id = a.Id,
                    CustomerId = a.CustomerId,
                    EmployeeId = a.EmployeeId,
                    AppointmentDate = a.AppointmentDate,
                    Status = a.Status ? "true" : "false",
                    CustomerName = $"{a.Customer.FirstName} {a.Customer.LastName}",
                    EmployeeName = $"{a.Employee.FirstName} {a.Employee.LastName}",
                    ServiceName = string.Join(", ", a.Services.Select(s => s.ServiceName))
                }).ToList();
                return Ok(appointmentDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving appointments: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentDTO appointmentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var appointment = new Appointment
            {
                CustomerId = appointmentDTO.CustomerId ?? 0,
                EmployeeId = appointmentDTO.EmployeeId ?? 0,
                AppointmentDate = appointmentDTO.AppointmentDate ?? DateTime.Now,
                Status = appointmentDTO.Status?.ToLower() == "true"
            };

            var createdAppointment = await _appointmentService.CreateAppointmentAsync(appointment);
            return Ok(createdAppointment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentDTO appointmentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var appointment = new Appointment
            {
                CustomerId = appointmentDTO.CustomerId ?? 0,
                EmployeeId = appointmentDTO.EmployeeId ?? 0,
                AppointmentDate = appointmentDTO.AppointmentDate ?? DateTime.Now,
                Status = appointmentDTO.Status?.ToLower() == "true"
            };

            var updatedAppointment = await _appointmentService.UpdateAppointmentAsync(id, appointment);
            if (updatedAppointment == null)
            {
                return NotFound("Appointment not found");
            }

            return Ok(updatedAppointment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _appointmentService.DeleteAppointmentAsync(id);
            if (appointment == null)
            {
                return NotFound("Appointment not found");
            }

            return Ok($"Deleted appointment with ID {appointment.Id}");
        }
    }
}
