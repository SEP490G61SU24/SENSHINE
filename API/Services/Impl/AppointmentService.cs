﻿using API.Dtos;
using API.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace API.Services.Impl
{
    public class AppointmentService : IAppointmentService
    {
        private readonly SenShineSpaContext _dbContext;
        private readonly IMapper _mapper;

        public AppointmentService(SenShineSpaContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<AppointmentDTO>> GetAllAppointmentsAsync()
        {
            var appointments = await _dbContext.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .Include(a => a.Bed)
                .Include(a => a.Slot)
                .Include(a => a.Services)
                .Include(a => a.Combos)
                .ToListAsync();

            return _mapper.Map<List<AppointmentDTO>>(appointments);
        }

        public async Task<List<AppointmentDTO>> GetAppointmentsByCustomerIdAsync(int customerId)
        {
            var appointments = await _dbContext.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .Include(a => a.Bed)
                .Include(a => a.Slot)
                .Include(a => a.Services)
                .Include(a => a.Combos)
                .Where(a => a.Customer.Id == customerId)
                .ToListAsync();

            return _mapper.Map<List<AppointmentDTO>>(appointments);
        }

        public async Task<List<AppointmentDTO>> GetAppointmentsByDateAsync(DateTime appointmentDate)
        {
            var appointments = await _dbContext.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .Include(a => a.Bed)
                .Include(a => a.Slot)
                .Include(a => a.Services)
                .Include(a => a.Combos)
                .Where(a => a.AppointmentDate.Date == appointmentDate.Date)
                .ToListAsync();

            return _mapper.Map<List<AppointmentDTO>>(appointments);
        }

        public async Task<AppointmentDTO> GetAppointmentByIdAsync(int id)
        {
            var appoint = await _dbContext.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .Include(a => a.Bed)
                .Include(a => a.Slot)
                .Include(a => a.Services)
                .Include(a => a.Combos)
                .FirstOrDefaultAsync(a => a.Id == id);

            return appoint == null ? null : _mapper.Map<AppointmentDTO>(appoint);
        }

        public async Task<Appointment> CreateAppointmentAsync(AppointmentDTO appointmentDTO)
        {
            if (appointmentDTO == null)
            {
                throw new ArgumentNullException(nameof(appointmentDTO), "Appointment data cannot be null.");
            }

            var appointment = _mapper.Map<Appointment>(appointmentDTO);

            // If services are provided, fetch the existing services
            if (appointmentDTO.Services != null && appointmentDTO.Services.Any())
            {
                var serviceIds = appointmentDTO.Services.Select(s => s.Id).ToList();
                var existingServices = await GetExistingServicesAsync(serviceIds);
                appointment.Services = existingServices;
            }

            // If combos are provided, fetch the existing combos
            if (appointmentDTO.Combos != null && appointmentDTO.Combos.Any())
            {
                var comboIds = appointmentDTO.Combos.Select(c => c.Id).ToList();
                var existingCombos = await GetExistingCombosAsync(comboIds);
                appointment.Combos = existingCombos;
            }

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await _dbContext.Appointments.AddAsync(appointment);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return appointment;
        }

        public async Task<Appointment> UpdateAppointmentAsync(int id, AppointmentDTO appointmentDTO)
        {
            var existingAppointment = await _dbContext.Appointments
                .Include(a => a.Services)
                .Include(a => a.Combos)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existingAppointment == null)
            {
                throw new InvalidOperationException("Appointment not found.");
            }

            _mapper.Map(appointmentDTO, existingAppointment);

            // Update Services
            existingAppointment.Services.Clear();
            var existingServices = await GetExistingServicesAsync(appointmentDTO.Services.Select(s => s.Id).ToList());
            foreach (var service in existingServices)
            {
                existingAppointment.Services.Add(service);
            }

            // Update Combos
            existingAppointment.Combos.Clear();
            var existingCombos = await GetExistingCombosAsync(appointmentDTO.Combos.Select(c => c.Id).ToList());
            foreach (var combo in existingCombos)
            {
                existingAppointment.Combos.Add(combo);
            }

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return existingAppointment;
        }

        public async Task<Appointment> DeleteAppointmentAsync(int id)
        {
            var existingAppointment = await _dbContext.Appointments
                .Include(a => a.Services)
                .Include(a => a.Combos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingAppointment == null)
            {
                return null;
            }

            existingAppointment.Services.Clear();
            existingAppointment.Combos.Clear();

            _dbContext.Appointments.Remove(existingAppointment);
            await _dbContext.SaveChangesAsync();

            return existingAppointment;
        }

        // Private helper methods
        private async Task<List<Service>> GetExistingServicesAsync(List<int> serviceIds)
        {
            return await _dbContext.Services
                .Where(s => serviceIds.Contains(s.Id))
                .ToListAsync();
        }

        private async Task<List<Combo>> GetExistingCombosAsync(List<int> comboIds)
        {
            return await _dbContext.Combos
                .Where(c => comboIds.Contains(c.Id))
                .ToListAsync();
        }

        public async Task<UserSlot> BookThisUser(int userId, int slotId, DateTime date)
        {
            var existingUserSlot = _dbContext.UserSlots.FirstOrDefault(u => u.UserId == userId && u.SlotId == slotId && u.SlotDate == date);

            if (existingUserSlot == null)
            {
                UserSlotDTO userSlotDTO = new UserSlotDTO()
                {
                    UserId = userId,
                    SlotId = slotId,
                    SlotDate = date,
                    Status = "booked"
                };

                var userSlot = _mapper.Map<UserSlot>(userSlotDTO);

                await _dbContext.UserSlots.AddAsync(userSlot);
                await _dbContext.SaveChangesAsync();
                return userSlot;
            }
            else
            {
                if (existingUserSlot.Status == "booked")
                {
                    existingUserSlot.Status = "empty";
                }
                else
                {
                    existingUserSlot.Status = "booked";
                }

                await _dbContext.SaveChangesAsync();
                return existingUserSlot;
            }
        }

        public async Task<BedSlot> BookThisBed(int bedId, int slotId, DateTime date)
        {
            var existingBedSlot = _dbContext.BedSlots.FirstOrDefault(u => u.BedId == bedId && u.SlotId == slotId && u.SlotDate == date);

            if (existingBedSlot == null)
            {
                BedSlotDTO bedSlotDTO = new BedSlotDTO()
                {
                    BedId = bedId,
                    SlotId = slotId,
                    SlotDate = date,
                    Status = "booked"
                };

                var bedSlot = _mapper.Map<BedSlot>(bedSlotDTO);

                await _dbContext.BedSlots.AddAsync(bedSlot);
                await _dbContext.SaveChangesAsync();
                return bedSlot;
            }
            else
            {
                if (existingBedSlot.Status == "booked")
                {
                    existingBedSlot.Status = "empty";
                }
                else
                {
                    existingBedSlot.Status = "booked";
                }

                await _dbContext.SaveChangesAsync();
                return existingBedSlot;
            }
        }

        public bool IsUserBooked(int userId, int slotId, DateTime date)
        {
            var existingUserSlot = _dbContext.UserSlots.FirstOrDefault(u => u.UserId == userId && u.SlotId == slotId && u.SlotDate == date);

            if (existingUserSlot != null && existingUserSlot.Status == "booked")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsBedBooked(int bedId, int slotId, DateTime date)
        {
            var existingBedSlot = _dbContext.BedSlots.FirstOrDefault(u => u.BedId == bedId && u.SlotId == slotId && u.SlotDate == date);

            if (existingBedSlot != null && existingBedSlot.Status == "booked")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<SlotDTO>> GetAllSlotsAsync()
        {
            var slots = await _dbContext.Slots.ToListAsync();

            return _mapper.Map<List<SlotDTO>>(slots);
        }
    }
}
