using API.Dtos;
using API.Models;
using API.Services;
using API.Ultils;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IUserService _userService;
        private readonly ISpaService _spaService;
        private readonly IMapper _mapper;
        public AppointmentController(IAppointmentService appointmentService, IUserService userService, ISpaService spaService,IMapper mapper)
        {
            _appointmentService = appointmentService;
            _userService = userService;
            _spaService = spaService;
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
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
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

                return Ok(appointment);
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
                    return NoContent();
                }
                return Ok(appointmentDTOs);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
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
                    return NoContent();
                }

                return Ok(appointments);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
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
                var cus = await _userService.GetById(appointmentDTO.CustomerId);
                if (cus == null)
                {
                    return BadRequest("Khách hàng không tồn tại.");
                }

                // Kiểm tra sự tồn tại của nhân viên trong cơ sở dữ liệu
                var emp = await _userService.GetById(appointmentDTO.EmployeeId);
                if (emp == null)
                {
                    return BadRequest("Nhân viên không tồn tại.");
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
                    return BadRequest("Không thể đặt lịch trong quá khứ.");
                }
                // Kiểm tra sự tồn tại của dịch vụ được thêm vào cuộc hẹn
                List<Service> existingServices = new List<Service>();
                if (appointmentDTO.Services != null && appointmentDTO.Services.Any())
                {
                    var checkValidSv = await _spaService.ValidateServicesAsync(appointmentDTO);

                    if (!checkValidSv)
                    {
                        return BadRequest("Một hoặc nhiều dịch vụ không tồn tại.");
                    }

                    // Đảm bảo các dịch vụ tồn tại không bị theo dõi trong ngữ cảnh
                    //foreach (var service in existingServices)
                    //{
                    //    _dbContext.Entry(service).State = EntityState.Unchanged;
                    //}
                }

                // Ánh xạ DTO thành đối tượng Appointment và gán dịch vụ, sản phẩm đã kiểm tra
                var appointment = _mapper.Map<Appointment>(appointmentDTO);
                appointment.Services = existingServices;

                // Tạo cuộc hẹn mới
                await _appointmentService.CreateAppointmentAsync(appointment);

                return Ok($"Create Appointment Successfully With ID: {appointment.Id}");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
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
                var existingAppointment = await _appointmentService.GetAppointmentByIdAsync(appointmentDTO.Id);

                if (existingAppointment == null)
                {
                    return NotFound("Không tìm thấy lịch hẹn.");
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

                // Kiểm tra sự tồn tại của khách hàng trong cơ sở dữ liệu
                var cus = await _userService.GetById(appointmentDTO.CustomerId);
                if (cus == null)
                {
                    return BadRequest("Khách hàng không tồn tại.");
                }

                // Kiểm tra sự tồn tại của nhân viên trong cơ sở dữ liệu
                var emp = await _userService.GetById(appointmentDTO.EmployeeId);
                if (emp == null)
                {
                    return BadRequest("Nhân viên không tồn tại.");
                }

                await _appointmentService.UpdateAppointmentAsync(appointmentDTO.Id, appointmentDTO);

                return Ok($"Update Appointment Successful With ID: {existingAppointment.Id}");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
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
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }
    }
}