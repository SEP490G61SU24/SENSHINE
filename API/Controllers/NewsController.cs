﻿using API.Dtos;
using API.Models;
using API.Services;
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
        public async Task<IActionResult> AddNews([FromBody] NewsDTO newsDto)
        {
            if (newsDto == null)
            {
                return BadRequest();
            }

            var createdNews = await _newsService.AddNews(newsDto);
            return Ok(createdNews);
        }

        [HttpPut("EditNews/{id}")]
        public async Task<IActionResult> EditNews(int id, [FromBody] NewsDTO newsDto)
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

        [HttpGet("ListAllNews")]
        public async Task<IActionResult> ListNews()
        {
            var newsList = await _newsService.ListNews();
            return Ok(newsList);
        }
        [HttpGet("ListNewsSortDESCByNewDate")]
        public async Task<IActionResult> ListNewsSortByNewDate()
        {
            var newsList = await _newsService.ListNewsSortByNewDate();
            return Ok(newsList);
        }

        [HttpGet("GetNewsDetail/{id}")]
        public async Task<IActionResult> GetNewsDetail(int id)
        {
            var news = await _newsService.GetNewsDetail(id);
            if (news == null)
            {
                return NotFound();
            }

            return Ok(news);
        }

        [HttpGet("NewsByDate")]
        public async Task<IActionResult> NewsByDate([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from > to)
            {
                return BadRequest("The 'from' date cannot be later than the 'to' date.");
            }

            var newsList = await _newsService.NewsByDate(from, to);
            return Ok(newsList);
        }


        [HttpDelete("DeleteNews/{id}")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            var success = await _newsService.DeleteNews(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }

}
