using API.Dtos;
using API.Models;

namespace API.Services
{
   
        public interface ICategoryService
        {
            Task<Category> AddCategory(CategoryDTO categoryDto);
            Task<Category> EditCategory(int id, CategoryDTO categoryDto);
            Task<IEnumerable<CategoryDTO>> ListCategory();
            Task<CategoryDTO> GetCategoryDetail(int id);
            Task<CategoryDTO> GetCategoryByName(string name);
            Task<bool> DeleteCategory(int id);
        }

    
}
