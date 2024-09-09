using API.Dtos;
using API.Models;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IUserService _userService;
        private readonly IWorkScheduleService _workScheduleService;
        private const string AppointmentNotFound = "Appointment not found";
        private const string ErrorMessage = "An error occurred: ";

        public AppointmentController(IAppointmentService appointmentService, IUserService userService, IWorkScheduleService workScheduleService)
        {
            _appointmentService = appointmentService;
            _userService = userService;
            _workScheduleService = workScheduleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            return await HandleRequestAsync(async () =>
            {
                var appointmentDTOs = await _appointmentService.GetAllAppointmentsAsync();
                return Ok(appointmentDTOs);
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSlots()
        {
            return await HandleRequestAsync(async () =>
            {
                var slotDTOs = await _appointmentService.GetAllSlotsAsync();
                return Ok(slotDTOs);
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetByAppointmentId(int id)
        {
            return await HandleRequestAsync(async () =>
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound(AppointmentNotFound);
                }
                return Ok(appointment);
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            return await HandleRequestAsync(async () =>
            {
                var appointmentDTOs = await _appointmentService.GetAppointmentsByCustomerIdAsync(customerId);
                if (appointmentDTOs == null || !appointmentDTOs.Any())
                {
                    return NoContent();
                }
                return Ok(appointmentDTOs);
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableEmployeesInThisSlot(int slotId, DateTime date, string spaId)
        {
            return await HandleRequestAsync(async () =>
            {
                var employees = await _appointmentService.GetAvailableEmployeesInThisSlotAsync(slotId, date, spaId);
                if (employees == null || !employees.Any())
                {
                    return NoContent();
                }
                return Ok(employees);
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetByDate(DateTime appointmentDate)
        {
            return await HandleRequestAsync(async () =>
            {
                var appointments = await _appointmentService.GetAppointmentsByDateAsync(appointmentDate);
                if (appointments == null || appointments.Count == 0)
                {
                    return NoContent();
                }
                return Ok(appointments);
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetSlotById(int id)
        {
            return await HandleRequestAsync(async () =>
            {
                var slot = await _appointmentService.GetSlotByIdAsync(id);
                if (slot == null)
                {
                    return NoContent();
                }
                return Ok(slot);
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AppointmentDTO appointmentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return await HandleRequestAsync(async () =>
            {
                if (appointmentDTO.AppointmentDate < DateTime.UtcNow)
                {
                    return BadRequest("Cannot book appointments in the past.");
                }

                var appointment = await _appointmentService.CreateAppointmentAsync(appointmentDTO);
                return Ok($"Appointment created successfully with ID: {appointment.Id}");
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAppointment([FromBody] AppointmentDTO appointmentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return await HandleRequestAsync(async () =>
            {
                var existingAppointment = await _appointmentService.GetAppointmentByIdAsync(appointmentDTO.Id);
                if (existingAppointment == null)
                {
                    return NotFound(AppointmentNotFound);
                }

                await _appointmentService.UpdateAppointmentAsync(appointmentDTO.Id, appointmentDTO);
                return Ok($"Appointment updated successfully with ID: {appointmentDTO.Id}");
            });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            return await HandleRequestAsync(async () =>
            {
                var deletedAppointment = await _appointmentService.DeleteAppointmentAsync(id);
                if (deletedAppointment == null)
                {
                    return NotFound(AppointmentNotFound);
                }
                return Ok($"Appointment deleted successfully with ID: {deletedAppointment.Id}");
            });
        }

        [HttpPost]
        public async Task<IActionResult> BookUser(int userId, int slotId, DateTime date)
        {
            return await HandleRequestAsync(async () =>
            {
                var userSlot = await _appointmentService.BookThisUser(userId, slotId, date);
                if (userSlot == null)
                {
                    return NotFound("User slot could not be booked.");
                }
                return Ok($"User booked successfully for slot ID: {slotId} on {date}");
            });
        }

        [HttpGet]
        public async Task<IActionResult> UserBooked(int userId, int slotId, DateTime date)
        {
            return await HandleRequestAsync(async () =>
            {
                bool isBooked = _appointmentService.IsUserBooked(userId, slotId, date);

                return Ok(isBooked);
            });
        }

        [HttpPost]
        public async Task<IActionResult> BookBed(int bedId, int slotId, DateTime date)
        {
            return await HandleRequestAsync(async () =>
            {
                var bedSlot = await _appointmentService.BookThisBed(bedId, slotId, date);
                if (bedSlot == null)
                {
                    return NotFound("Bed slot could not be booked.");
                }
                return Ok($"Bed booked successfully for slot ID: {slotId} on {date}");
            });
        }

        [HttpGet]
        public async Task<IActionResult> BedBooked(int bedId, int slotId, DateTime date)
        {
            return await HandleRequestAsync(async () =>
            {
                var isBooked = _appointmentService.IsBedBooked(bedId, slotId, date);

                return Ok(isBooked);
            });
        }

        private async Task<IActionResult> HandleRequestAsync(Func<Task<IActionResult>> func)
        {
            try
            {
                return await func();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorMessage + ex.Message);
            }
        }
    }
}
