using API.Dtos;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {

        private readonly SenShineSpaContext _dbContext;
        private readonly IReviewService reviewService;
        public ReviewController(SenShineSpaContext dbContext, IReviewService reviewService)
        {
            this._dbContext = dbContext;
            this.reviewService = reviewService;
        }
        //Lay ra danh sach toan bo review 
        [HttpGet]
        public async Task<IActionResult> GetAllReview()
        {
            try
            {
                var listOfReview = await reviewService.GetAllReviewAsync();
                return Ok(listOfReview);
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

        //Lay ra thong tin review cu the
        [HttpGet]
        public async Task<IActionResult> GetByID(int IdReview)
        {
            try
            {
                if (IdReview < 1)
                {
                    return BadRequest("ID Review không tồn tại");
                }
                else
                {
                    var review = await reviewService.FindReviewWithItsId(IdReview);
                    if (review == null)
                    {
                        return NotFound("Review không tồn tại");
                    }
                    return Ok(review);
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

        //Tạo review mới
        [HttpPost]
        [Route("/api/[controller]/[action]")]
        public async Task<IActionResult> Create([FromBody] ReviewDTO reviewDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Chuyển đổi ReviewDTO thành review để lưu vào cơ sở dữ liệu
                var newReview = new Review
                {
                Rating = reviewDTO.Rating,
                Comment = reviewDTO.Comment,
                ReviewDate = reviewDTO.ReviewDate
            };

                var createdReview = await reviewService.CreateReviewAsync(newReview);
                return Ok($"Tạo mới Review thành công");
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

        // Edit review
        [HttpPut]
        [Route("/api/[controller]/[action]")]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] ReviewDTO reviewDTO)
        {
            if (id < 1)
            {
                return BadRequest("ID review không hợp lệ");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingReview = await reviewService.FindReviewWithItsId(id);
                if (existingReview == null)
                {
                    return NotFound("Không tìm thấy review để cập nhật");
                }

                // Cập nhật các thông tin từ serviceDTO vào existingService
                existingReview.Rating = reviewDTO.Rating;
                existingReview.Comment = reviewDTO.Comment;
                existingReview.ReviewDate = reviewDTO.ReviewDate;

                var updatedReview = await reviewService.EditReviewAsync(id, existingReview);
                if (updatedReview == null)
                {
                    return NotFound("Không tìm thấy review để cập nhật");
                }
                return Ok(updatedReview);
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


        // DELETE: api/combo/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            if (id < 1)
            {
                return BadRequest("ID review không hợp lệ");
            }

            try
            {
                var deletedReview = await reviewService.DeleteReviewAsync(id);
                if (deletedReview == null)
                {
                    return NotFound("Không tìm thấy review để xóa");
                }
                return Ok($"Đã xóa review có ID {deletedReview.Id}");
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
