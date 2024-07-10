using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Impl
{
    public class ReviewService : IReviewService
    {
        private readonly SenShineSpaContext _dbContext;
        public ReviewService(SenShineSpaContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<Review> CreateReviewAsync(Review review)
        {
            await _dbContext.Reviews.AddAsync(review);
            await _dbContext.SaveChangesAsync();
            return review;
        }

        public async Task<Review> DeleteReviewAsync(int Id)
        {
            var existingReview = await _dbContext.Reviews.FirstOrDefaultAsync(x => x.Id == Id);
            if (existingReview == null)
            {
                return null;
            }
            _dbContext.Reviews.Remove(existingReview);
            await _dbContext.SaveChangesAsync();
            return existingReview;
        }

        public async Task<Review> EditReviewAsync(int Id, Review review)
        {
            var existingReview = await _dbContext.Reviews.FirstOrDefaultAsync(x => x.Id == Id);
            if (existingReview == null)
            {
                return null;
            }
            existingReview.Rating = review.Rating;
            existingReview.Comment = review.Comment;
            existingReview.ReviewDate = review.ReviewDate;
            await _dbContext.SaveChangesAsync();

            return existingReview;
        }

        public async Task<Review> FindReviewWithItsId(int Id)
        {
            return await _dbContext.Reviews.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<List<Review>> GetAllReviewAsync()
        {
            return await _dbContext.Reviews.ToListAsync();
        }
    }
}
