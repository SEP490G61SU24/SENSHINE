using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Impl
{
    public class AppointmentService : IAppointmentService
    {
        private readonly SenShineSpaContext _context;

        public AppointmentService(SenShineSpaContext context)
        {
            _context = context;
        }

        public async Task<Appointment> AddAppointment(int customerId, int employeeId, DateTime appointmentDate, bool status)
        {
            var appointment = new Appointment
            {
                CustomerId = customerId,
                EmployeeId = employeeId,
                AppointmentDate = appointmentDate,
                Status = status
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task<Appointment> UpdateAppointment(int id, int customerId, int employeeId, DateTime appointmentDate, bool status)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return null;
            }

            appointment.CustomerId = customerId;
            appointment.EmployeeId = employeeId;
            appointment.AppointmentDate = appointmentDate;
            appointment.Status = status;

            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task<bool> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return false;
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Appointment> GetAppointmentById(int id)
        {
            return await _context.Appointments
                                 .Include(a => a.Customer)
                                 .Include(a => a.Employee)
                                 .Include(a => a.Products)
                                 .Include(a => a.Services)
                                 .SingleOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointments()
        {
            return await _context.Appointments
                                 .Include(a => a.Customer)
                                 .Include(a => a.Employee)
                                 .Include(a => a.Products)
                                 .Include(a => a.Services)
                                 .ToListAsync();
        }
    }
}
