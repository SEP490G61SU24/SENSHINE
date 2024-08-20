using API.Dtos;
using API.Models;
using API.Services;
using API.Services.Impl;
using API.Ultils;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly SenShineSpaContext _dbContext;
        private readonly IMapper _mapper;
        public AppointmentController(IAppointmentService appointmentService, SenShineSpaContext dbContext, IMapper mapper)
        {
            _appointmentService = appointmentService;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            try
            {
                var appointmentDTOs = await _appointmentService.GetAllAppointmentsAsync();
                return Ok(appointmentDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving appointments: {ex.Message}");
            }
        }


        ////Tim kiem cuoc hen theo id cuoc hen
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByAppointmentId(int id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound("Appointment not found");
                }

                // Fetch address information in service method
                var customerWard = await _dbContext.Wards.FirstOrDefaultAsync(w => w.Code == appointment.Customer.WardCode);
                var customerDistrict = customerWard != null ? await _dbContext.Districts.FirstOrDefaultAsync(d => d.Code == customerWard.DistrictCode) : null;
                var customerProvince = customerDistrict != null ? await _dbContext.Provinces.FirstOrDefaultAsync(p => p.Code == customerDistrict.ProvinceCode) : null;

                var customerAddress = $"{customerWard?.Name ?? "-"} - {customerDistrict?.Name ?? "-"} - {customerProvince?.Name ?? "-"}";

                var appointmentDTO = new AppointmentDTO
                {
                    Id = appointment.Id,
                    CustomerId = appointment.CustomerId,
                    EmployeeId = appointment.EmployeeId,
                    AppointmentDate = appointment.AppointmentDate,
                    AppointmentSlot = appointment.AppointmentSlot,
                    RoomName = appointment.RoomName,
                    BedNumber = appointment.BedNumber,

                    Status = appointment.Status,
                    Customer = new AppointmentUserDTO
                    {
                        Id = appointment.Customer.Id,
                        FullName = appointment.Customer.FirstName + " " + appointment.Customer.MidName + " " + appointment.Customer.LastName,
                        Phone = appointment.Customer.Phone,
                        Address = customerAddress
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
                    }).ToList(),
                    Products = appointment.Products.Select(p => new AppointmentDTO.AppointmentProductDTO
                    {
                        ProductId = p.Id,
                        ProductName = p.ProductName
                    }).ToList()
                };

                return Ok(appointmentDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving appointment: {ex.Message}");
            }
        }

        //Tim kiem theo ID khach hang
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            try
            {
                var appointmentDTOs = await _appointmentService.GetAppointmentsByCustomerIdAsync(customerId);
                if (appointmentDTOs == null || !appointmentDTOs.Any())
                {
                    return NotFound("No appointments found for this customer");
                }
                return Ok(appointmentDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving appointments: {ex.Message}");
            }
        }


        //Tim kiem cuoc hen theo ngay
        [HttpGet("{appointmentDate}")]
        public async Task<IActionResult> GetAppointmentsByDate(DateTime appointmentDate)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByDateAsync(appointmentDate);
                if (appointments == null || appointments.Count == 0)
                {
                    return NotFound("Appointments not found for the specified date");
                }

                // Fetch address information for each appointment
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
                        AppointmentSlot = appointment.AppointmentSlot,
                        RoomName = appointment.RoomName,
                        BedNumber = appointment.BedNumber,

                        Status = appointment.Status,
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
                        }).ToList(),
                        Products = appointment.Products.Select(p => new AppointmentDTO.AppointmentProductDTO
                        {
                            ProductId = p.Id,
                            ProductName = p.ProductName
                        }).ToList()
                    });
                }

                return Ok(appointmentDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving appointments: {ex.Message}");
            }
        }


        // Create - Tạo mới cuộc hẹn
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AppointmentDTO appointmentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Kiểm tra sự tồn tại của khách hàng trong cơ sở dữ liệu
                if (appointmentDTO.CustomerId.HasValue)
                {
                    var customerExists = await _dbContext.Users.AnyAsync(c => c.Id == appointmentDTO.CustomerId.Value);
                    if (!customerExists)
                    {
                        return BadRequest("Customer does not exist.");
                    }
                }

                // Kiểm tra sự tồn tại của nhân viên trong cơ sở dữ liệu
                if (appointmentDTO.EmployeeId.HasValue)
                {
                    var employeeExists = await _dbContext.Users.AnyAsync(e => e.Id == appointmentDTO.EmployeeId.Value);
                    if (!employeeExists)
                    {
                        return BadRequest("Employee does not exist.");
                    }
                }
                // Kiểm tra thời gian slot của cuộc hẹn
                var validSlots = new List<string>
                {
                            AppointmentSlotUtils.Slot1,
                            AppointmentSlotUtils.Slot2,
                            AppointmentSlotUtils.Slot3,
                            AppointmentSlotUtils.Slot4,
                            AppointmentSlotUtils.Slot5,
                            AppointmentSlotUtils.Slot6,
                            AppointmentSlotUtils.Slot7,
                            AppointmentSlotUtils.Slot8,
                            AppointmentSlotUtils.Slot9
                };

                if (!validSlots.Contains(appointmentDTO.AppointmentSlot))
                {
                    return BadRequest("Invalid appointment slot.");
                }


                // Kiểm tra trạng thái cuộc hẹn
                var validStatuses = new List<string>
                {
                        AppointmentStatusUtils.Cancelled,
                        AppointmentStatusUtils.Pending,
                        AppointmentStatusUtils.Doing,
                        AppointmentStatusUtils.Finished,
                        AppointmentStatusUtils.Combo

                };

                if (!validStatuses.Contains(appointmentDTO.Status))
                {
                    return BadRequest("Invalid status value.");
                }
                // Kiểm tra ngày đặt lịch (chỉ cho phép đặt lịch cho các ngày tiếp theo)
                if (appointmentDTO.AppointmentDate.HasValue &&
                    appointmentDTO.AppointmentDate.Value.Date < DateTime.UtcNow.Date)
                {
                    return BadRequest("Cannot create an appointment for a past date.");
                }
                // Kiểm tra sự tồn tại của dịch vụ được thêm vào cuộc hẹn
                List<Service> existingServices = new List<Service>();
                if (appointmentDTO.Services != null && appointmentDTO.Services.Any())
                {
                    var serviceIds = appointmentDTO.Services.Select(s => s.Id).ToList();
                    existingServices = await _dbContext.Services
                                                        .Where(s => serviceIds.Contains(s.Id))
                                                        .ToListAsync();

                    if (existingServices.Count != serviceIds.Count)
                    {
                        return BadRequest("One or more services do not exist.");
                    }

                    // Đảm bảo các dịch vụ tồn tại không bị theo dõi trong ngữ cảnh
                    foreach (var service in existingServices)
                    {
                        _dbContext.Entry(service).State = EntityState.Unchanged;
                    }
                }

                // Kiểm tra sự tồn tại của sản phẩm được thêm vào cuộc hẹn
                List<Product> existingProducts = new List<Product>();
                if (appointmentDTO.Products != null && appointmentDTO.Products.Any())
                {
                    var productIds = appointmentDTO.Products.Select(p => p.ProductId).ToList();
                    existingProducts = await _dbContext.Products
                                                       .Where(p => productIds.Contains(p.Id))
                                                       .ToListAsync();

                    if (existingProducts.Count != productIds.Count)
                    {
                        return BadRequest("One or more products do not exist.");
                    }

                    // Đảm bảo các sản phẩm tồn tại không bị theo dõi trong ngữ cảnh
                    foreach (var product in existingProducts)
                    {
                        _dbContext.Entry(product).State = EntityState.Unchanged;
                    }
                }

                // Ánh xạ DTO thành đối tượng Appointment và gán dịch vụ, sản phẩm đã kiểm tra
                var appointment = _mapper.Map<Appointment>(appointmentDTO);
                appointment.Services = existingServices;
                appointment.Products = existingProducts;

                // Tạo cuộc hẹn mới
                await _appointmentService.CreateAppointmentAsync(appointment);

                return Ok($"Create Appointment Successfully With ID: {appointment.Id}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating appointment: {ex.Message}");
            }
        }

        // Update - Cập nhật cuộc hẹn
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentDTO appointmentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingAppointment = await _dbContext.Appointments
                                                          .Include(a => a.Services)
                                                          .Include(a => a.Products)
                                                          .FirstOrDefaultAsync(a => a.Id == id);
                if (existingAppointment == null)
                {
                    return NotFound("Appointment not found.");
                }

                // Kiểm tra trạng thái cuộc hẹn
                var validStatuses = new List<string>
        {
            AppointmentStatusUtils.Cancelled,
            AppointmentStatusUtils.Pending,
            AppointmentStatusUtils.Doing,
            AppointmentStatusUtils.Finished,
            AppointmentStatusUtils.Combo

        };

                if (!validStatuses.Contains(appointmentDTO.Status))
                {
                    return BadRequest("Invalid status value.");
                }

                // Kiểm tra thời gian slot của cuộc hẹn
                var validSlots = new List<string>
        {
            AppointmentSlotUtils.Slot1,
            AppointmentSlotUtils.Slot2,
            AppointmentSlotUtils.Slot3,
            AppointmentSlotUtils.Slot4,
            AppointmentSlotUtils.Slot5,
            AppointmentSlotUtils.Slot6,
            AppointmentSlotUtils.Slot7,
            AppointmentSlotUtils.Slot8,
            AppointmentSlotUtils.Slot9
        };

                if (!validSlots.Contains(appointmentDTO.AppointmentSlot))
                {
                    return BadRequest("Invalid appointment slot.");
                }

                // Kiểm tra sự tồn tại của khách hàng
                if (appointmentDTO.CustomerId.HasValue)
                {
                    var customerExists = await _dbContext.Users.AnyAsync(c => c.Id == appointmentDTO.CustomerId.Value);
                    if (!customerExists)
                    {
                        return BadRequest("Customer does not exist.");
                    }
                }

                // Kiểm tra sự tồn tại của nhân viên
                if (appointmentDTO.EmployeeId.HasValue)
                {
                    var employeeExists = await _dbContext.Users.AnyAsync(e => e.Id == appointmentDTO.EmployeeId.Value);
                    if (!employeeExists)
                    {
                        return BadRequest("Employee does not exist.");
                    }
                }

                // Xử lý dịch vụ
                List<Service> existingServices = new List<Service>();
                if (appointmentDTO.Services != null && appointmentDTO.Services.Any())
                {
                    var serviceIds = appointmentDTO.Services.Select(s => s.Id).ToList();
                    existingServices = await _dbContext.Services
                                                       .Where(s => serviceIds.Contains(s.Id))
                                                       .ToListAsync();

                    if (existingServices.Count != serviceIds.Count)
                    {
                        return BadRequest("One or more services do not exist.");
                    }
                }

                // Xử lý sản phẩm
                List<Product> existingProducts = new List<Product>();
                if (appointmentDTO.Products != null && appointmentDTO.Products.Any())
                {
                    var productIds = appointmentDTO.Products.Select(p => p.ProductId).ToList();
                    existingProducts = await _dbContext.Products
                                                       .Where(p => productIds.Contains(p.Id))
                                                       .ToListAsync();

                    if (existingProducts.Count != productIds.Count)
                    {
                        return BadRequest("One or more products do not exist.");
                    }
                }

                // Ánh xạ và cập nhật cuộc hẹn
                _mapper.Map(appointmentDTO, existingAppointment);
                existingAppointment.Services = existingServices;
                existingAppointment.Products = existingProducts;

                await _dbContext.SaveChangesAsync();

                return Ok($"Update Appointment Successful With ID: {existingAppointment.Id}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Cannot Update Appointment: {ex.Message}");
            }
        }


        // Delete Appointment
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            if (id < 1)
            {
                return BadRequest("ID Appointment not found");
            }

            try
            {
                var deletedAppointment = await _appointmentService.DeleteAppointmentAsync(id);
                if (deletedAppointment == null)
                {
                    return NotFound("Not found");
                }
                return Ok($"Delete successful ID {deletedAppointment.Id}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}