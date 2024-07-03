using API.Models;

namespace API.Services
{
    public interface IAppointmentService
    {
        Task<Appointment> AddAppointment(int customerId, int employeeId, DateTime appointmentDate, bool status);
        Task<Appointment> UpdateAppointment(int id, int customerId, int employeeId, DateTime appointmentDate, bool status);
        Task<bool> DeleteAppointment(int id);
        Task<Appointment> GetAppointmentById(int id);
        Task<IEnumerable<Appointment>> GetAllAppointments();
    }
}
