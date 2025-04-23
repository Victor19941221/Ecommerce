using AspNetCore.Identity.Mongo.Model;

namespace Ecommerce.Models
{
    public class ApplicationUser : MongoUser
    {
        public Address PrimaryAddress { get; set; } = new Address();
    }
}
