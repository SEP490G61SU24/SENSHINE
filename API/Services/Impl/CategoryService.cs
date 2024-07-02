using API.Dtos;
using API.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Impl
{
    public class CategoryService : ICategoryService
    {
        private readonly SenShineSpaContext _context;
        private readonly IMapper _mapper;

        public CategoryService(SenShineSpaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Category> AddCategory(CategoryDTO categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return _mapper.Map<Category>(category);
        }

        public async Task<Category> EditCategory(int id, CategoryDTO categoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return null;
            }

            _mapper.Map(categoryDto, category);
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return _mapper.Map<Category>(category);
        }

        public async Task<IEnumerable<CategoryDTO>> ListCategory()
        {
            var categories = await _context.Categories.ToListAsync();
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<CategoryDTO> GetCategoryDetail(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<CategoryDTO> GetCategoryByName(string name)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == name);
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return false;
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }
    }

}
