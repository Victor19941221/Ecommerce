namespace Ecommerce.Models
{
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; }
        public List<Review> Reviews { get; set; }
        public double AverageRating { get; set; }
    }

}
