using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Ecommerce.Models
{
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string ProductId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = "";
        public string Content { get; set; } = "";
        public int Rating { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }

}
