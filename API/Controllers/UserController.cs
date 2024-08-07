﻿using API.Dtos;
using API.Models;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userName = userIdClaim.Value;
            var user = await _userService.GetByUserName(userName);
            if (user == null)
            {
                return NotFound();
            }

            var userProfile = _mapper.Map<UserDTO>(user);
            return Ok(userProfile);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddUser([FromBody] UserDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.AddUser(userDto);

            var resultDto = _mapper.Map<UserDTO>(user);

            return Ok(resultDto);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var user = _mapper.Map<User>(userDto);
            var user = await _userService.UpdateUser(id, userDto);

            if (user == null)
            {
                return NotFound();
            }

            var userDtoRes = _mapper.Map<UserDTO>(user);
            return Ok(userDtoRes);
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
            if (users == null || !users.Any())
            {
                var emptyArray = new UserDTO[0];
                return Ok(emptyArray);
            }

            var userDtos = _mapper.Map<IEnumerable<UserDTO>>(users);
            return Ok(userDtos);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAll();
            if (users == null || !users.Any())
            {
                var emptyArray = new UserDTO[0];
                return Ok(emptyArray);
            }

            var userDtos = _mapper.Map<IEnumerable<UserDTO>>(users);
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var u = await _userService.GetById(id);
            if (u == null)
            {
                return NotFound();
            }

            var userDto = _mapper.Map<UserDTO>(u);
            return Ok(userDto);
        }
    }
}
