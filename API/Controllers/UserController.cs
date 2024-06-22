﻿using API.Dtos;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddUser([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.AddUser(
                userDto.UserName,
                userDto.Password,
                userDto.FirstName,
                userDto.MidName,
                userDto.LastName,
                userDto.BirthDate,
                userDto.ProvinceCode,
                userDto.DistrictCode,
                userDto.WardCode
            );
            return Ok(user);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.UpdateUser(
                id, userDto
            );

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUser(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("byRole/{roleId}")]
        public async Task<IActionResult> GetUsersByRole(int roleId)
        {
            var users = await _userService.GetUsersByRole(roleId);
            var userDtos = users.Select(u => new UserListDto
            {
                Id = u.Id,
                UserName = u.UserName,
                FirstName = u.FirstName,
                MidName = u.MidName,
                LastName = u.LastName,
                BirthDate = u.BirthDate,
                ProvinceCode = u.ProvinceCode,
                DistrictCode = u.DistrictCode,
                WardCode = u.WardCode,
            });

            return Ok(userDtos);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAll();
            var userDtos = users.Select(u => new UserListDto
            {
                Id = u.Id,
                UserName = u.UserName,
                FirstName = u.FirstName,
                MidName = u.MidName,
                LastName = u.LastName,
                BirthDate = u.BirthDate,
                ProvinceCode = u.ProvinceCode,
                DistrictCode = u.DistrictCode,
                WardCode = u.WardCode,
            });
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserListDto
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                MidName = user.MidName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                ProvinceCode = user.ProvinceCode,
                DistrictCode = user.DistrictCode,
                WardCode = user.WardCode,
            };

            return Ok(userDto);
        }
    }
}
