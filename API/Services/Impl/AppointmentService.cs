﻿using API.Dtos;
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

        public async Task<List<AppointmentDTO>> GetAllAppointmentsAsync()
        {
            var appointments = await _dbContext.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .Include(a => a.Services)
                .ToListAsync();

            var appointmentDTOs = new List<AppointmentDTO>();

            foreach (var appointment in appointments)
            {
                var customerWard = await _dbContext.Wards.FirstOrDefaultAsync(w => w.Code == appointment.Customer.WardCode);
                var customerDistrict = customerWard != null ? await _dbContext.Districts.FirstOrDefaultAsync(d => d.Code == customerWard.DistrictCode) : null;
                var customerProvince = customerDistrict != null ? await _dbContext.Provinces.FirstOrDefaultAsync(p => p.Code == customerDistrict.ProvinceCode) : null;

                var address = $"{customerWard?.Name ?? "-"} - {customerDistrict?.Name ?? "-"} - {customerProvince?.Name ?? "-"}";

                appointmentDTOs.Add(new AppointmentDTO
                {
                    Id = appointment.Id,
                    CustomerId = appointment.CustomerId,
                    EmployeeId = appointment.EmployeeId,
                    AppointmentDate = appointment.AppointmentDate,
                    Status = appointment.Status ? "true" : "false",
                    Customer = new AppointmentUserDTO
                    {
                        Id = appointment.Customer.Id,
                        FullName = appointment.Customer.FirstName + " " + appointment.Customer.MidName + " " + appointment.Customer.LastName,
                        Phone = appointment.Customer.Phone,
                        Address = address
                    },
                    Employee = new AppointmentUserDTO
                    {
                        Id = appointment.Employee.Id,
                        FullName = appointment.Employee.FirstName + " " + appointment.Employee.MidName + " " + appointment.Employee.LastName,
                        Phone = appointment.Employee.Phone
                    },
                    Services = appointment.Services.Select(s => new ServiceDTO
                    {
                        Id = s.Id,
                        ServiceName = s.ServiceName,
                        Amount = s.Amount,
                        Description = s.Description
                    }).ToList()
                });
            }

            return appointmentDTOs;
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
