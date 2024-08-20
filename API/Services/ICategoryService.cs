using API.Dtos;
using API.Models;
using API.Ultils;

namespace API.Services
{
   
        public interface ICategoryService
        {
            Task<Category> AddCategory(CategoryDTO categoryDto);
            Task<Category> EditCategory(int id, CategoryDTO categoryDto);
            Task<IEnumerable<CategoryDTO>> ListCategory();
            Task<CategoryDTO> GetCategoryDetail(int id);
            Task<IEnumerable<CategoryDTO>> GetCategoriesByProductId(int Id);
            Task<CategoryDTO> GetCategoryByName(string name);
            Task<bool> DeleteCategory(int id);
        Task<PaginatedList<CategoryDTO>> GetCategoryList(int pageIndex = 1, int pageSize = 10, string? searchTerm = null);
    }

    
}
