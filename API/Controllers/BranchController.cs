using API.Dtos;
using API.Models;
using API.Services;
using API.Services.Impl;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BranchController : Controller
    {
        private readonly IBranchService _branchService;
        private readonly IMapper _mapper;

        public BranchController(IBranchService branchService, IMapper mapper)
        {
            _branchService = branchService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BranchDTO branchDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var branchMap = _mapper.Map<Spa>(branchDTO);
                var createdBranch = await _branchService.CreateBranch(branchMap);
                return Ok($"Spa '{createdBranch.SpaName}' created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the spa: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (pageIndex < 1 || pageSize < 1)
                {
                    return BadRequest("Chỉ số trang hoặc kích thước trang không hợp lệ.");
                }

                var branches = await _branchService.GetBranches(pageIndex, pageSize, searchTerm);
                return Ok(branches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the spas: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (!_branchService.BranchExist(id))
                    return NotFound("Spa not found.");

                var branch = _mapper.Map<BranchDTO>(_branchService.GetBranch(id));

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(branch);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the spa: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBranchByUser(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var branchId = _branchService.GetBranchIdByUserId(id);
                return Ok(branchId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the spa by user ID: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersByBranchID(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var users = await _branchService.GetUsersByBranch(id);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the spa by user ID: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] BranchDTO branchDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_branchService.BranchExist(id))
                return NotFound("Spa not found.");

            try
            {
                var existingBranch = _branchService.GetBranch(id);
                existingBranch.SpaName = branchDTO.SpaName;
                existingBranch.ProvinceCode = branchDTO.ProvinceCode;
                existingBranch.DistrictCode = branchDTO.DistrictCode;
                existingBranch.WardCode = branchDTO.WardCode;
                var branchUpdate = await _branchService.UpdateBranch(id, existingBranch);

                if (branchUpdate == null)
                {
                    return NotFound("Failed to update spa.");
                }

                return Ok($"Spa '{branchUpdate.SpaName}' updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the spa: {ex.Message}");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _branchService.DeleteBranch(id);
                if (!result)
                {
                    return NotFound("Spa not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the spa: {ex.Message}");
            }
        }
    }
}
