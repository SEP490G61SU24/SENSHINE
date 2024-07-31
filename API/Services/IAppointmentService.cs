using API.Dtos;
using API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IAppointmentService
    {
        Task<List<AppointmentDTO>> GetAllAppointmentsAsync();
        Task<List<Appointment>> GetAppointmentsByDateAsync(DateTime appointmentDate);
        Task<Appointment> GetAppointmentByIdAsync(int id);
        Task<Appointment> CreateAppointmentAsync(Appointment appointment);
        Task<Appointment> UpdateAppointmentAsync(int id, Appointment appointment);
        Task<Appointment> DeleteAppointmentAsync(int id);
    }
}
