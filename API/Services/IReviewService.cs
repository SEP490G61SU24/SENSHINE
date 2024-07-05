using API.Models;

namespace API.Services
{
    public interface IReviewService
    {
        //tim kiem tat ca cac Review
        Task<List<Review>> GetAllReviewAsync();

        //tim kiem Review theo id
        Task<Review> FindReviewWithItsId(int Id);
        //them 1 Review moi
        Task<Review> CreateReviewAsync(Review review);
        //edit 1 Review 
        Task<Review> EditReviewAsync(int Id, Review review);
        //xoa 1 Review
        Task<Review> DeleteReviewAsync(int Id);
    }
}
