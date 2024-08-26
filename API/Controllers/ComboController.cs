using API.Dtos;
using API.Models;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ComboController : ControllerBase
    {
        private readonly IComboService _comboService;
        private readonly IMapper _mapper;

        public ComboController(IComboService comboService, IMapper mapper)
        {
            _comboService = comboService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCombo()
        {
            try
            {
                var listOfCombo = await _comboService.GetAllComboAsync();
                return Ok(listOfCombo);
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

        [HttpGet]
        public async Task<IActionResult> GetByID(int IdCombo)
        {
            if (IdCombo < 1)
                return BadRequest("Invalid Combo ID.");

            try
            {
                var combo = await _comboService.FindComboWithItsId(IdCombo);
                return combo == null ? NotFound("Combo not found") : Ok(combo);
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

        [HttpPost]
        [Route("/api/[controller]/[action]")]
        public async Task<IActionResult> Create([FromBody] ComboDTO comboDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var newCombo = _mapper.Map<Combo>(comboDTO);
                var createdCombo = await _comboService.CreateComboAsync(newCombo);
                return Ok($"Successfully created {createdCombo.Name}");
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

        [HttpPut]
        [Route("/api/[controller]/[action]/{id}")]
        public async Task<IActionResult> UpdateCombo(int id, [FromBody] ComboDTO comboDTO)
        {
            if (id < 1)
                return BadRequest("Invalid Combo ID.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedCombo = await _comboService.EditComboAsync(id, comboDTO);
                return updatedCombo == null ? NotFound("Combo not found for update") : Ok(updatedCombo);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCombo(int id)
        {
            if (id < 1)
                return BadRequest("Invalid Combo ID.");

            try
            {
                var deletedCombo = await _comboService.DeleteComboAsync(id);
                return deletedCombo == null ? NotFound("Combo not found for deletion") : Ok($"Deleted combo with ID {deletedCombo.Id}");
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

        [HttpGet]
        public async Task<IActionResult> GetAllCombosPaging([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
        {
            try
            {
                if (pageIndex < 1 || pageSize < 1)
                    return BadRequest("Invalid page index or page size.");

                var pageData = await _comboService.GetComboList(pageIndex, pageSize, searchTerm);
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
