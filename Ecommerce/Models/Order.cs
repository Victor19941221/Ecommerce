using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ecommerce.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? UserId { get; set; }
        public string? UserEmail { get; set; }
        public Address ShippingAddress { get; set; } = new Address();
        public List<OrderItem> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public bool IsPaid { get; set; } = false;
        public bool IsShipped { get; set; } = false;
        public string? StripeSessionId { get; set; }
    }
}
