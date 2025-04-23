// File: Models/Product.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        
       
        public string Category { get; set; } = string.Empty;

        public List<string>? Categories { get; set; }  
    }
}
