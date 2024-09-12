using API.Dtos;
using API.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
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

        public async Task<AppointmentDTO> GetAppointmentsByBedslotDateAsync(int bedId, int slotId, string date)
        {
            var appoint = await _dbContext.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .Include(a => a.Bed)
                .Include(a => a.Slot)
                .Include(a => a.Services)
                .Include(a => a.Combos)
                .FirstOrDefaultAsync(a => a.AppointmentDate.Date == DateTime.Parse(date) && a.BedId == bedId && a.SlotId == slotId);

            return appoint == null ? null : _mapper.Map<AppointmentDTO>(appoint);
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
            if (appointmentDTO.ServiceIDs != null && appointmentDTO.ServiceIDs.Any())
            {
                var serviceIds = appointmentDTO.ServiceIDs.ToList();
                var existingServices = await GetExistingServicesAsync(serviceIds);
                appointment.Services = existingServices;
            }

            // If combos are provided, fetch the existing combos
            if (appointmentDTO.ComboIDs != null && appointmentDTO.ComboIDs.Any())
            {
                var comboIds = appointmentDTO.ComboIDs.ToList();
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

            if (!appointmentDTO.ServiceIDs.IsNullOrEmpty())
            {
                // Update Services
                existingAppointment.Services.Clear();
                var existingServices = await GetExistingServicesAsync(appointmentDTO.ServiceIDs.ToList());
                foreach (var service in existingServices)
                {
                    existingAppointment.Services.Add(service);
                }
            }
            else
            {
                existingAppointment.Services.Clear();
            }

            if (!appointmentDTO.ComboIDs.IsNullOrEmpty())
            {
                // Update Combos
                existingAppointment.Combos.Clear();
                var existingCombos = await GetExistingCombosAsync(appointmentDTO.ComboIDs.ToList());
                foreach (var combo in existingCombos)
                {
                    existingAppointment.Combos.Add(combo);
                }
            }
            else
            {
                existingAppointment.Combos.Clear();
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

        public async Task<List<UserDTO>> GetAvailableEmployeesInThisSlotAsync(int slotId, DateTime date, string spaId)
        {
            int? spaIdInt = spaId != null && spaId != "ALL"
                ? int.Parse(spaId)
                : (int?)null;

            // Fetch users with the necessary role and, optionally, by spaId
            var query = _dbContext.Users.Include(u => u.Roles)
                             .Where(u => u.Roles.Any(r => r.Id == 4));

            if (spaIdInt.HasValue)
            {
                query = query.Where(u => u.SpaId == spaIdInt.Value);
            }

            // Filter out users who have been booked for the specified slot and date
            var availableUsers = await query
                .Where(u => !_dbContext.UserSlots
                    .Any(us => us.UserId == u.Id && us.SlotId == slotId && us.SlotDate == date && us.Status == "booked"))
                .ToListAsync();

            // Map directly to UserDTO
            var availableEmployees = _mapper.Map<List<UserDTO>>(availableUsers);

            return availableEmployees;
        }

        public async Task<SlotDTO> GetSlotByIdAsync(int id)
        {
            var slot = await _dbContext.Slots.FirstOrDefaultAsync(s => s.Id == id);

            return slot == null ? null : _mapper.Map<SlotDTO>(slot);
        }
    }
}
