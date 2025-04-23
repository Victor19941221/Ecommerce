using Ecommerce.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Ecommerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMongoCollection<Order> _orders;

        public OrderService(IOptions<MongoSettings> config, IMongoClient mongoClient)
        {
            var db = mongoClient.GetDatabase(config.Value.DatabaseName);
            _orders = db.GetCollection<Order>("Orders");
        }

        public async Task<List<Order>> GetAllAsync() =>
            await _orders.Find(_ => true).SortByDescending(o => o.OrderDate).ToListAsync();

        public async Task<List<Order>> GetByUserIdAsync(string userId) =>
            await _orders.Find(o => o.UserId == userId).SortByDescending(o => o.OrderDate).ToListAsync();

        public async Task<Order?> GetByIdAsync(string id) =>
            await _orders.Find(o => o.Id == id).FirstOrDefaultAsync();

        public async Task<Order?> GetByStripeSessionIdAsync(string sessionId) =>
            await _orders.Find(o => o.StripeSessionId == sessionId).FirstOrDefaultAsync();

        public Task CreateAsync(Order order)
        {
            if (string.IsNullOrEmpty(order.Id))
                order.Id = ObjectId.GenerateNewId().ToString();
            return _orders.InsertOneAsync(order);
        }

        public async Task MarkOrderPaidAsync(string stripeSessionId)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.StripeSessionId, stripeSessionId);
            var update = Builders<Order>.Update.Set(o => o.IsPaid, true);
            await _orders.UpdateOneAsync(filter, update);
        }

        public async Task MarkOrderShippedAsync(string id)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
            var update = Builders<Order>.Update.Set(o => o.IsShipped, true);
            await _orders.UpdateOneAsync(filter, update);
        }
    }
}
