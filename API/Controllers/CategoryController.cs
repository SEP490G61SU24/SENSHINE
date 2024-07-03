using API.Dtos;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("AddCategory")]
        public async Task<ActionResult<CategoryDTO>> AddCategory(CategoryDTO categoryDto)
        {
            var category = await _categoryService.AddCategory(categoryDto);
            return CreatedAtAction(nameof(GetCategoryDetail), new { id = category.Id }, category);
        }

        [HttpPut("EditCategory/{id}")]
        public async Task<IActionResult> EditCategory(int id, CategoryDTO categoryDto)
        {
            var category = await _categoryService.EditCategory(id, categoryDto);
            if (category == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("ListAllCategory")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> ListCategory()
        {
            var categories = await _categoryService.ListCategory();
            return Ok(categories);
        }

        [HttpGet("GetCategoryDetailById/{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategoryDetail(int id)
        {
            var category = await _categoryService.GetCategoryDetail(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpGet("GetCategoryDetailByName/{name}")]
        public async Task<ActionResult<CategoryDTO>> GetCategoryByName(string name)
        {
            var category = await _categoryService.GetCategoryByName(name);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategory(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }

}
