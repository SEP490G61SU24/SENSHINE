using API.Dtos;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ServiceController : Controller
    {
        private readonly ISpaService spaService;
        public ServiceController(ISpaService spaService)
        {
            this.spaService = spaService;
        }

        //Lay ra danh sach toan bo service 
        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            try
            {
                var listOfServices = await spaService.GetAllServiceAsync();
                return Ok(listOfServices);
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

        //Lay ra thong tin service cu the
        [HttpGet]
        public async Task<IActionResult> GetByID(int Id)
        {
            try
            {
                if (Id < 1)
                {
                    return BadRequest("ID Service không tồn tại");
                }
                else
                {
                    var service = await spaService.FindServiceWithItsId(Id);
                    if (service == null)
                    {
                        return NotFound("Service không tồn tại");
                    }
                    return Ok(service);
                }
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

        //Tạo service mới
        [HttpPost]
        [Route("/api/[controller]/[action]")]
        public async Task<IActionResult> Create([FromBody] ServiceDTO serviceDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Chuyển đổi ServiceDTO thành Service để lưu vào cơ sở dữ liệu
                var newService = new Service
                {
                    ServiceName = serviceDTO.ServiceName,
                    Amount = serviceDTO.Amount,
                    Description = serviceDTO.Description
                };

                var createdService = await spaService.CreateServiceAsync(newService);
                return Ok($"Tạo mới dịch vụ {createdService.ServiceName} thành công");
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

        // Edit service
        [HttpPut]
        [Route("/api/[controller]/[action]")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] ServiceDTO serviceDTO)
        {
            if (id < 1)
            {
                return BadRequest("ID Service không hợp lệ");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingService = await spaService.FindServiceWithItsId(id);
                if (existingService == null)
                {
                    return NotFound("Không tìm thấy dịch vụ để cập nhật");
                }

                // Cập nhật các thông tin từ serviceDTO vào existingService
                existingService.ServiceName = serviceDTO.ServiceName;
                existingService.Amount = serviceDTO.Amount;
                existingService.Description = serviceDTO.Description;

                var updatedService = await spaService.EditServiceAsync(id, existingService);
                if (updatedService == null)
                {
                    return NotFound("Không tìm thấy dịch vụ để cập nhật");
                }
                return Ok(updatedService);
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

        // DELETE: api/service/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            if (id < 1)
            {
                return BadRequest("ID Service không hợp lệ");
            }

            try
            {
                var deletedService = await spaService.DeleteServiceAsync(id);
                if (deletedService == null)
                {
                    return NotFound("Không tìm thấy dịch vụ để xóa");
                }
                return Ok($"Đã xóa dịch vụ có ID {deletedService.Id} thành công!");
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
