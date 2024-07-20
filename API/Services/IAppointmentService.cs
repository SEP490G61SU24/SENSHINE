using API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IAppointmentService
    {
        Task<List<Appointment>> GetAllAppointmentsAsync();
        Task<List<Appointment>> GetAppointmentsByDateAsync(DateTime appointmentDate);
        Task<List<Appointment>> GetAppointmentsByCustomerIdAsync(int customerId);
        Task<Appointment> CreateAppointmentAsync(Appointment appointment);
        Task<Appointment> UpdateAppointmentAsync(int id, Appointment appointment);
        Task<Appointment> DeleteAppointmentAsync(int id);
    }
}
