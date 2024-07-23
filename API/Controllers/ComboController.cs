using API.Dtos;
using API.Models;
using API.Services;
using API.Services.Impl;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ComboController : ControllerBase
    {
        private readonly SenShineSpaContext _dbContext;
        private readonly IComboService comboService;
        public ComboController(SenShineSpaContext dbContext, IComboService comboService)
        {
            this._dbContext = dbContext;
            this.comboService = comboService;
        }
        //Lay ra danh sach toan bo combo 
        [HttpGet]
        public async Task<IActionResult> GetAllCombo()
        {
            try
            {
                var listOfCombo = await comboService.GetAllComboAsync();
                return Ok(listOfCombo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy danh sách dịch vụ: {ex.Message}");
            }
        }
        //Lay ra thong tin combo cu the
        [HttpGet]
        public async Task<IActionResult> GetByID(int IdCombo)
        {
            if (IdCombo < 1)
            {
                return BadRequest("ID Combo không tồn tại");
            }
            else
            {
                var combo = await comboService.FindComboWithItsId(IdCombo);
                if (combo == null)
                {
                    return NotFound("Combo không tồn tại");
                }
                return Ok(combo);
            }
        }
        //Tạo combo mới
        [HttpPost]
        [Route("/api/[controller]/[action]")]
        public async Task<IActionResult> Create([FromBody] ComboDTO comboDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Retrieve services based on their IDs
                var services = await _dbContext.Services
                                                .Where(s => comboDTO.ServiceIds.Contains(s.Id))
                                                .ToListAsync();

                // Convert ComboDTO to Combo entity
                var newCombo = new Combo
                {
                    Name = comboDTO.Name,
                    Quantity = comboDTO.Quantity,
                    Note = comboDTO.Note,
                    Discount = comboDTO.Discount,
                    Services = services
                };

                // Calculate the total price of the combo
                newCombo.Price = services.Sum(s => s.Amount);

                // Calculate the sale price after discount
                if (newCombo.Discount.HasValue && newCombo.Price.HasValue)
                {
                    newCombo.SalePrice = newCombo.Price - (newCombo.Price * newCombo.Discount / 100);
                }

                var createdCombo = await comboService.CreateComboAsync(newCombo);
                return Ok($"Tạo mới {createdCombo.Name} thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi tạo combo mới: {ex.Message}");
            }
        }

        // Edit service
        [HttpPut]
        [Route("/api/[controller]/[action]")]
        public async Task<IActionResult> UpdateCombo(int id, [FromBody] ComboDTO comboDTO)
        {
            if (id < 1)
            {
                return BadRequest("ID Combo không hợp lệ");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingCombo = await comboService.FindComboWithItsId(id);
                if (existingCombo == null)
                {
                    return NotFound("Không tìm thấy combo để cập nhật");
                }

                // Cập nhật các thông tin từ serviceDTO vào existingService
                existingCombo.Name = comboDTO.Name;
                existingCombo.Price = comboDTO.Price;
                existingCombo.SalePrice = comboDTO.SalePrice;

                var updatedCombo = await comboService.EditComboAsync(id, existingCombo);
                if (updatedCombo == null)
                {
                    return NotFound("Không tìm thấy combo để cập nhật");
                }
                return Ok(updatedCombo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi cập nhật combo: {ex.Message}");
            }
        }


        // DELETE: api/combo/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCombo(int id)
        {
            if (id < 1)
            {
                return BadRequest("ID Combo không hợp lệ");
            }

            try
            {
                var deletedCombo = await comboService.DeleteComboAsync(id);
                if (deletedCombo == null)
                {
                    return NotFound("Không tìm thấy combo để xóa");
                }
                return Ok($"Đã xóa combo có ID {deletedCombo.Id}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xóa combo: {ex.Message}");
            }
        }
    }
}
