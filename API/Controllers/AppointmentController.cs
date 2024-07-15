using API.Dtos;
using API.Models;
using API.Services;
using API.Services.Impl;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IProductService _productService; 
        private readonly ISpaService _spaService; 
        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        //Lay ra tat ca danh sach cuoc hen
        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            try
            {
                var appointments = await _appointmentService.GetAllAppointmentsAsync();
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving appointments: {ex.Message}");
            }
        }
        // lay danh sach theo appointment id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound("Appointment not found");
            }

            return Ok(appointment);
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetAppointmentsByCustomerId(int customerId) // tim kiem theo customer id
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByCustomerIdAsync(customerId);
                if (appointments == null || appointments.Count == 0)
                {
                    return NotFound("No appointments found for this customer.");
                }
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving appointments: {ex.Message}");
            }
        }
        //Create appointment
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

        // Update Appointment
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentDTO appointmentDTO)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    // Tìm appointment theo ID
        //    var existingAppointment = await _appointmentService.GetAppointmentByIdAsync(id);
        //    if (existingAppointment == null)
        //    {
        //        return NotFound("Appointment not found");
        //    }

        //    // Cap nhat cac truong can thiet
        //    existingAppointment.CustomerId = appointmentDTO.CustomerId ?? existingAppointment.CustomerId;
        //    existingAppointment.EmployeeId = appointmentDTO.EmployeeId ?? existingAppointment.EmployeeId;
        //    existingAppointment.AppointmentDate = appointmentDTO.AppointmentDate ?? existingAppointment.AppointmentDate;

        //    if (appointmentDTO.Status != null)
        //    {
        //        if (Enum.TryParse<AppointmentStatus>(appointmentDTO.Status, true, out var status))
        //        {
        //            existingAppointment.Status = status;
        //        }
        //        else
        //        {
        //            return BadRequest("Invalid status value");
        //        }
        //    }

        //    try
        //    {
        //        var updatedAppointment = await _appointmentService.UpdateAppointmentAsync(id, existingAppointment);
        //        return Ok(updatedAppointment);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error updating appointment: {ex.Message}");
        //    }
        //}


        //Xoa appointment theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _appointmentService.DeleteAppointmentAsync(id);
            if (appointment == null)
            {
                return NotFound("Appointment not found");
            }

            return Ok($"Delete successful appointment with ID {appointment.Id}");
        }

    }
}

