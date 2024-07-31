using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Dtos;
using API.Models;
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
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var promotion = await _promotionService.AddPromotion(promotionDto);
                return Ok(promotion);
        }

            
            [HttpPut("EditPromotion/{id}")]
            public async Task<IActionResult> EditPromotion(int id, [FromBody] PromotionDTORequest promotionDto)
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

            
            [HttpGet("ListAllPromotion")]
            public async Task<ActionResult<IEnumerable<PromotionDTORespond>>> ListPromotions()
            {
            var promotions = await _promotionService.ListPromotion();
                return Ok(promotions);
            }

            
            [HttpGet("GetPromotionDetail/{id}")]
            public async Task<IActionResult> GetPromotionDetail(int id)
            {
                var promotion = await _promotionService.GetPromotionDetail(id);
                if (promotion == null)
                {
                    return NotFound();
                }

                return Ok(promotion);
            }


        [HttpGet("GetPromotionDetail")]
        public async Task<ActionResult<IEnumerable<PromotionDTORequest>>> GetPromotionDetail([FromQuery] string spaLocation, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var promotions = await _promotionService.GetPromotionsByFilter(spaLocation, startDate, endDate);
            if (promotions == null)
            {
                return NotFound();
            }

            return Ok(promotions);
        }


        [HttpDelete("DeletePromotion/{id}")]
            public async Task<IActionResult> DeletePromotion(int id)
            {
                var result = await _promotionService.DeletePromotion(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }
    }


