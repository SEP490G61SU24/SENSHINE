using API.Dtos;
using API.Models;
using API.Ultils;

namespace API.Services
{
    public interface IComboService
    {
        //tim kiem tat ca cac combo
        Task<List<ComboDTO>> GetAllComboAsync();

        //tim kiem combo theo id
        Task<ComboDTO> FindComboWithItsId(int Id);
        //them 1 combo moi
        Task<Combo> CreateComboAsync(Combo combo);
        //edit 1 combo 
        Task<ComboDTO> EditComboAsync(int Id, ComboDTO comboDTO);
        //xoa 1 combo
        Task<Combo> DeleteComboAsync(int Id);
        Task<PaginatedList<ComboDTO2>> GetComboList(int pageIndex = 1, int pageSize = 10, string? searchTerm = null);

    }
}
