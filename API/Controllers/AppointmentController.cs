using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost]
        public async Task<IActionResult> AddAppointment(Appointment appointment)
        {
            var addedAppointment = await _appointmentService.AddAppointment(appointment.CustomerId, appointment.EmployeeId, appointment.AppointmentDate, appointment.Status);
            return CreatedAtAction(nameof(GetAppointmentById), new { id = addedAppointment.Id }, addedAppointment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, Appointment appointment)
        {
            var updatedAppointment = await _appointmentService.UpdateAppointment(id, appointment.CustomerId, appointment.EmployeeId, appointment.AppointmentDate, appointment.Status);
            if (updatedAppointment == null)
            {
                return NotFound();
            }

            return Ok(updatedAppointment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var success = await _appointmentService.DeleteAppointment(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var appointment = await _appointmentService.GetAppointmentById(id);
            if (appointment == null)
            {
                return NotFound();
            }

            return Ok(appointment);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await _appointmentService.GetAllAppointments();
            return Ok(appointments);
        }
    }
}


//using API.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace API.Controllers
//{
//    [Route("api/[controller]/[action]")]
//    [ApiController]
//    public class AppointmentController : Controller
//    {
//        private readonly SenShineSpaContext _dbContext;
//        public AppointmentController(SenShineSpaContext dbContext)
//        {
//            this._dbContext = dbContext;
//        }
//        // //       GET: api/AllApointment
//        //           [HttpGet]
//        //            public async Task<ActionResult<IEnumerable<Appointment>>> GetAll()
//        //        {
//        //            try
//        //            {
//        //                var appointments = await _dbContext.Appointments.ToListAsync();
//        //                return Ok(appointments);
//        //            }
//        //            catch (Exception ex)
//        //            {
//        //                return BadRequest($"Failed to retrieve appointments: {ex.Message}");
//        //            }
//        //        }
//        //        // GET: api/GetByAppointmentID lay ra danh sach cuoc hen theo ID
//        //        [HttpGet("{id}")]
//        //        public async Task<ActionResult<Appointment>> GetAppointmentById(int IdAppointment)
//        //        {
//        //            var appointments = await _dbContext.Appointments.FindAsync(IdAppointment);

//        //            if (appointments == null)
//        //            {
//        //                return NotFound();
//        //            }

//        //            return appointments;
//        //        }

//        //        // DELETE: api/Appoitnment xoa cuoc hen theo id 
//        //        [HttpDelete("{id}")]
//        //        public async Task<IActionResult> DeleteAppointment(int IdAppointment)
//        //        {
//        //            var appointments = await _dbContext.Appointments.FindAsync(IdAppointment);
//        //            if (appointments == null)
//        //            {
//        //                return NotFound();
//        //            }

//        //            _dbContext.Appointments.Remove(appointments);
//        //            await _dbContext.SaveChangesAsync();

//        //            return NoContent();
//        //        }
//    }
//}