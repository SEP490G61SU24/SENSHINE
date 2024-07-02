using API.Models;

namespace API.Services
{
    public interface IComboService
    {
        //tim kiem tat ca cac combo
        Task<List<Combo>> GetAllComboAsync();

        //tim kiem combo theo id
        Task<Combo> FindComboWithItsId(int Id);
        //them 1 combo moi
        Task<Combo> CreateComboAsync(Combo combo);
        //edit 1 combo 
        Task<Combo> EditComboAsync(int Id, Combo combo);
        //xoa 1 combo
        Task<Combo> DeleteComboAsync(int Id);
    }
}
