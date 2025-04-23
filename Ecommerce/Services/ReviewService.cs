using Ecommerce.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Ecommerce.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IMongoCollection<Review> _reviews;

        public ReviewService(IOptions<MongoSettings> config, IMongoClient mongoClient)
        {
            var db = mongoClient.GetDatabase(config.Value.DatabaseName);
            _reviews = db.GetCollection<Review>("Reviews");
        }

        public async Task<List<Review>> GetByProductIdAsync(string productId) =>
            await _reviews.Find(r => r.ProductId == productId).SortByDescending(r => r.Date).ToListAsync();

        public async Task<List<Review>> GetByUserIdAsync(string userId) =>
            await _reviews.Find(r => r.UserId == userId).ToListAsync();

        public async Task<Review?> GetByIdAsync(string id) =>
            await _reviews.Find(r => r.Id == id).FirstOrDefaultAsync();

        public Task CreateAsync(Review review)
        {
            if (string.IsNullOrEmpty(review.Id))
                review.Id = ObjectId.GenerateNewId().ToString();
            return _reviews.InsertOneAsync(review);
        }

        public Task DeleteAsync(string id) =>
            _reviews.DeleteOneAsync(r => r.Id == id);
    }

}
