using API.Dtos;
using API.Models;
using API.Services;
using API.Services.Impl;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api")]
    [ApiController]
    
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpPost("AddNews")]
        public async Task<IActionResult> AddNews([FromBody] News newsDto)
        {
            try
            {
                if (newsDto == null)
                {
                    return BadRequest();
                }

                var createdNews = await _newsService.AddNews(newsDto);
                return Ok(createdNews);
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

        [HttpPut("EditNews/{id}")]
        public async Task<IActionResult> EditNews(int id, [FromBody] NewsDTO newsDto)
        {
            try
            {
                if (newsDto == null )
                {
                    return BadRequest();
                }

                var updatedNews = await _newsService.EditNews(id, newsDto);
                if (updatedNews == null)
                {
                    return NotFound();
                }

                return Ok(updatedNews);
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

        [HttpGet("ListAllNews")]
        public async Task<IActionResult> ListNews()
        {
            try
            {
                var newsList = await _newsService.ListNews();
                return Ok(newsList);
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
        [HttpGet("ListNewsSortDESCByNewDate")]
        public async Task<IActionResult> ListNewsSortByNewDate()
        {
            try
            {
                var newsList = await _newsService.ListNewsSortByNewDate();
                return Ok(newsList);
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

        [HttpGet("GetNewsDetail/{id}")]
        public async Task<IActionResult> GetNewsDetail(int id)
        {
            try
            {
                var news = await _newsService.GetNewsDetail(id);
                if (news == null)
                {
                    return NotFound();
                }

                return Ok(news);
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

        [HttpGet("NewsByDate")]
        public async Task<IActionResult> NewsByDate([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            try
            {
                if (from > to)
                {
                    return BadRequest("The 'from' date cannot be later than the 'to' date.");
                }

                var newsList = await _newsService.NewsByDate(from, to);
                return Ok(newsList);
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


        [HttpDelete("DeleteNews/{id}")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            try
            {
                var success = await _newsService.DeleteNews(id);
                if (!success)
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

        [HttpGet("GetNewsPaging")]
        public async Task<IActionResult> GetAllNewsPaging([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                if (pageIndex < 1 || pageSize < 1)
                {
                    return BadRequest("Chỉ số trang hoặc kích thước trang không hợp lệ.");
                }

                var pageData = await _newsService.GetNews(pageIndex, pageSize, searchTerm,startDate,endDate);
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
