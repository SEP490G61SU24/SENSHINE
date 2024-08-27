using API.Dtos;
using API.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

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
                .Include(a => a.Services)
                .ToListAsync();

            var listDtos = _mapper.Map<List<AppointmentDTO>>(appointments);

            return listDtos;
        }

        //Tim kiem theo ID cua khach hang
        public async Task<List<AppointmentDTO>> GetAppointmentsByCustomerIdAsync(int customerId)
        {
            var appointments = await _dbContext.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .Include(a => a.Services)
                .Where(a => a.CustomerId == customerId)
                .ToListAsync();

            var listDtos = _mapper.Map<List<AppointmentDTO>>(appointments);

            return listDtos;
        }

        public async Task<List<AppointmentDTO>> GetAppointmentsByDateAsync(DateTime appointmentDate)
        {
            var appointments = await _dbContext.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .Include(a => a.Services)
                //.Where(a => a.AppointmentDate.Date == appointmentDate.Date)
                .ToListAsync();
            var listDtos = _mapper.Map<List<AppointmentDTO>>(appointments);

            return listDtos;
        }

        public async Task<AppointmentDTO> GetAppointmentByIdAsync(int id)
        {
            var appoint = await _dbContext.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .Include(a => a.Services)
                .FirstOrDefaultAsync(a => a.Id == id);
            var appDto = _mapper.Map<AppointmentDTO>(appoint);
            return appDto;
        }

        public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
        {
            await _dbContext.Appointments.AddAsync(appointment);
            await _dbContext.SaveChangesAsync();
            return appointment;
        }

        public async Task<Appointment> UpdateAppointmentAsync(int id, AppointmentDTO appointmentDTO)
        {
            var existingAppointment = await _dbContext.Appointments
                                                       .Include(a => a.Services)
                                                       .FirstOrDefaultAsync(x => x.Id == id);
            if (existingAppointment == null)
            {
                throw new InvalidOperationException("Không tìm thấy lịch hẹn.");
            }

            _mapper.Map(appointmentDTO, existingAppointment);

            // Xử lý cập nhật các dịch vụ
            existingAppointment.Services.Clear();

            var serviceIds = appointmentDTO.Services.Select(s => s.Id).ToList();

            var existingServices = await _dbContext.Services
                                                   .Where(s => serviceIds.Contains(s.Id))
                                                   .ToListAsync();

            foreach (var service in existingServices)
            {
                _dbContext.Entry(service).State = EntityState.Unchanged;
                existingAppointment.Services.Add(service);
            }

            await _dbContext.SaveChangesAsync();
            return existingAppointment;
        }


        //Delete
        public async Task<Appointment> DeleteAppointmentAsync(int id)
        {
            // Tìm appointment theo ID
            var existingAppointment = await _dbContext.Appointments
                                                 .Include(c => c.Services)
                                                 .FirstOrDefaultAsync(c => c.Id == id);

            if (existingAppointment == null)
            {
                return null;
            }
            // Xóa các liên kết với các dịch vụ nếu cần
            foreach (var service in existingAppointment.Services.ToList())
            {
                // Xóa liên kết giữa combo và dịch vụ
                existingAppointment.Services.Remove(service);
            }

            _dbContext.Appointments.Remove(existingAppointment);
            await _dbContext.SaveChangesAsync();
            return existingAppointment;
        }

    }
}
