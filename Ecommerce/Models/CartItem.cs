namespace Ecommerce.Models
{
    public class CartItem
    {
        public string ProductId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

}
