using Ecommerce.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Ecommerce.Services
{
    public class ProductService : IProductService
    {
        private readonly IMongoCollection<Product> _products;

        public ProductService(IOptions<MongoSettings> config, IMongoClient mongoClient)
        {
            var db = mongoClient.GetDatabase(config.Value.DatabaseName);
            _products = db.GetCollection<Product>("Products");
        }

        public async Task<IEnumerable<Product>> GetAllAsync() =>
            await _products.Find(_ => true).ToListAsync();

        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category) =>
            await _products.Find(p => p.Category == category).ToListAsync();

        public async Task<Product?> GetByIdAsync(string id) =>
            await _products.Find(p => p.Id == id).FirstOrDefaultAsync();

        public Task CreateAsync(Product product)
        {
            product.Id = ObjectId.GenerateNewId().ToString();
            return _products.InsertOneAsync(product);
        }

        public Task UpdateAsync(string id, Product updated) =>
            _products.ReplaceOneAsync(p => p.Id == id, updated);

        public Task DeleteAsync(string id) =>
            _products.DeleteOneAsync(p => p.Id == id);
    }
}
