using API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return await _dbContext.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .Include(a => a.Services)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsByDateAsync(DateTime appointmentDate)
        {
            return await _dbContext.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .Include(a => a.Services)
                .Where(a => a.AppointmentDate.Date == appointmentDate.Date)
                .ToListAsync();
        }

        public async Task<Appointment> GetAppointmentByIdAsync(int id)
        {
            return await _dbContext.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .Include(a => a.Services)
                .FirstOrDefaultAsync(a => a.Id == id);
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



        //Delete
        public async Task<Appointment> DeleteAppointmentAsync(int id)
        {
            // Tìm combo theo ID
            var existingAppointment = await _dbContext.Appointments
                                                 .Include(c => c.Services) // Bao gồm các dịch vụ liên quan
                                                 .FirstOrDefaultAsync(c => c.Id == id);

            if (existingAppointment == null)
            {
                return null;
            }

            // Xóa các liên kết với các dịch vụ nếu cần
            // Ví dụ: nếu có bảng liên kết nhiều-nhiều, có thể cần xử lý nó tại đây
            foreach (var service in existingAppointment.Services.ToList())
            {
                // Xóa liên kết giữa combo và dịch vụ
                existingAppointment.Services.Remove(service);
            }

            // Xóa combo
            _dbContext.Appointments.Remove(existingAppointment);
            await _dbContext.SaveChangesAsync();
            return existingAppointment;
        }

    }
}
