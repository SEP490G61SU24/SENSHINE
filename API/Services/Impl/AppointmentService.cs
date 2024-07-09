using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services.Impl
{
    public class AppointmentService : IAppointmentService
    {
        private readonly SenShineSpaContext _dbContext;

        public AppointmentService(SenShineSpaContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            return await _dbContext.Appointments.ToListAsync();
        }

        public async Task<Appointment> GetAppointmentByIdAsync(int id)
        {
            return await _dbContext.Appointments.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Appointment>> GetAppointmentsByCustomerIdAsync(int customerId)
        {
            return await _dbContext.Appointments.Where(a => a.CustomerId == customerId).ToListAsync();
        }

        public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
        {
            await _dbContext.Appointments.AddAsync(appointment);
            await _dbContext.SaveChangesAsync();
            return appointment;
        }

        public async Task<Appointment> UpdateAppointmentAsync(int id, Appointment appointment)
        {
            var existingAppointment = await _dbContext.Appointments.FirstOrDefaultAsync(x => x.Id == id);
            if (existingAppointment == null)
            {
                return null;
            }

            existingAppointment.CustomerId = appointment.CustomerId;
            existingAppointment.EmployeeId = appointment.EmployeeId;
            existingAppointment.AppointmentDate = appointment.AppointmentDate;
            existingAppointment.Status = appointment.Status;

            await _dbContext.SaveChangesAsync();
            return existingAppointment;
        }

        public async Task<Appointment> DeleteAppointmentAsync(int id)
        {
            var appointment = await _dbContext.Appointments.FirstOrDefaultAsync(x => x.Id == id);
            if (appointment == null)
            {
                return null;
            }

            _dbContext.Appointments.Remove(appointment);
            await _dbContext.SaveChangesAsync();
            return appointment;
        }
    }
}
