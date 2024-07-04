using API.Models;

namespace API.Services
{
    public interface IReviewService
    {
        //tim kiem tat ca cac Review
        Task<List<Review>> GetAllReviewAsync();

        //tim kiem Review theo id
        Task<Review> FindReviewWithItsId(int Id);
        //them 1 service moi
        Task<Review> CreateReviewAsync(Service services);
        //edit 1 service 
        Task<Review> EditReviewAsync(int Id, Service services);
        //xoa 1 service
        Task<Review> DeleteReviewAsync(int Id);
    }
}
