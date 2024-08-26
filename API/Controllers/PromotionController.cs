using Microsoft.AspNetCore.Mvc;
using API.Dtos;
using API.Services;

namespace API.Controllers
{
    [Route("api")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpPost("AddPromotion")]
        public async Task<IActionResult> AddPromotion([FromBody] PromotionDTORequest promotionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var promotion = await _promotionService.AddPromotion(promotionDto);
                return Ok(promotion);
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

            
        [HttpPut("EditPromotion/{id}")]
        public async Task<IActionResult> EditPromotion(int id, [FromBody] PromotionDTORequest promotionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var promotion = await _promotionService.EditPromotion(id, promotionDto);
                if (promotion == null)
                {
                    return NotFound();
                }

                return NoContent();
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

        [HttpGet("ListAllPromotion")]
        public async Task<ActionResult<IEnumerable<PromotionDTORespond>>> ListPromotions()
        {
            try
            {
                var promotions = await _promotionService.ListPromotion();
                return Ok(promotions);
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

        [HttpGet("GetPromotionDetail/{id}")]
        public async Task<IActionResult> GetPromotionDetail(int id)
        {
            try
            {

                var promotion = await _promotionService.GetPromotionDetail(id);
                if (promotion == null)
                {
                    return NotFound();
                }

                return Ok(promotion);
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
        
        [HttpGet("GetPromotionsByFilter")]
        public async Task<ActionResult<IEnumerable<PromotionDTORespond>>> GetPromotionsByFilter(string? spaLocation = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var promotions = await _promotionService.GetPromotionsByFilter(spaLocation, startDate, endDate);
                return Ok(promotions);
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


        [HttpDelete("DeletePromotion/{id}")]
        public async Task<IActionResult> DeletePromotion(int id)
        {
            try
            {

                var result = await _promotionService.DeletePromotion(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
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

        [HttpGet("GetPromotionsPaging")]
        public async Task<IActionResult> GetAllPromotionsPaging([FromQuery]int? idspa=null, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                if (pageIndex < 1 || pageSize < 1)
                {
                    return BadRequest("Chỉ số trang hoặc kích thước trang không hợp lệ.");
                }

                var pageData = await _promotionService.GetPromotionListBySpaId(idspa,pageIndex, pageSize, searchTerm,startDate,endDate);
                return Ok(pageData);
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