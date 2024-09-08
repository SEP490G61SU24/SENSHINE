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
        Task<List<SlotDTO>> GetAllSlotsAsync();
        Task<List<AppointmentDTO>> GetAppointmentsByDateAsync(DateTime appointmentDate);
        Task<AppointmentDTO> GetAppointmentByIdAsync(int id);
        Task<Appointment> CreateAppointmentAsync(AppointmentDTO appointmentDTO);
        Task<Appointment> UpdateAppointmentAsync(int id, AppointmentDTO appointmentDTO);
        Task<Appointment> DeleteAppointmentAsync(int id);
        Task<List<AppointmentDTO>> GetAppointmentsByCustomerIdAsync(int customerId);
        Task<UserSlot> BookThisUser(int userId, int slotId, DateTime date);
        Task<BedSlot> BookThisBed(int bedId, int slotId, DateTime date);
        bool IsUserBooked(int userId, int slotId, DateTime date);
        bool IsBedBooked(int bedId, int slotId, DateTime date);
    }
}
