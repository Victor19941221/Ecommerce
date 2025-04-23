using Ecommerce.Models;

namespace Ecommerce.Services
{
    public interface IReviewService
    {
        Task<List<Review>> GetByProductIdAsync(string productId);
        Task<List<Review>> GetByUserIdAsync(string userId);
        Task<Review?> GetByIdAsync(string id);
        Task CreateAsync(Review review);
        Task DeleteAsync(string id);
    }
}
