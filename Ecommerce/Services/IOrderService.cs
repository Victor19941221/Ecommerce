using Ecommerce.Models;

namespace Ecommerce.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllAsync();
        Task<List<Order>> GetByUserIdAsync(string userId);
        Task<Order?> GetByIdAsync(string id);
        Task<Order?> GetByStripeSessionIdAsync(string sessionId);
        Task CreateAsync(Order order);
        Task MarkOrderPaidAsync(string stripeSessionId);
        Task MarkOrderShippedAsync(string id);
    }
}
