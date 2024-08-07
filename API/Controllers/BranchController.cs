using API.Dtos;
using API.Models;
using API.Services;
using API.Services.Impl;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

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

                return Ok($"Tạo spa {createdBranch.SpaName} thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra khi tạo spa: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var branchs = _mapper.Map<List<BranchDTO>>(_branchService.GetBranchs());

            return Ok(branchs);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            if (!_branchService.BranchExist(id))
                return NotFound();

            var branch = _mapper.Map<BranchDTO>(_branchService.GetBranch(id));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(branch);
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] BranchDTO branchDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_branchService.BranchExist(id))
                return NotFound();

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
                    return NotFound("Không thể cập nhật spa");
                }

                return Ok($"Cập nhật spa {branchUpdate.SpaName} thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra khi cập nhật spa: {ex.Message}");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _branchService.DeleteBranch(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
