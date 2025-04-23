using Ecommerce.Models;
using Ecommerce.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Ecommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;

        public CartController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index()
        {
            string? cartJson = HttpContext.Session.GetString("Cart");
            List<CartItem> cart = cartJson != null
                                  ? JsonSerializer.Deserialize<List<CartItem>>(cartJson)
                                  : new List<CartItem>();
            return View(cart);
        }


        // In Cart or Product controller
        [HttpPost]
        public IActionResult AddToCart(string productId)
        {
            var product = _productService.GetByIdAsync(productId).Result;
            if (product == null) return NotFound();

            string? cartJson = HttpContext.Session.GetString("Cart");
            List<CartItem> cart = cartJson != null
                                  ? JsonSerializer.Deserialize<List<CartItem>>(cartJson)
                                  : new List<CartItem>();

            var item = cart.FirstOrDefault(i => i.ProductId == product.Id);
            if (item != null)
            {
                item.Quantity += 1;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id!,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = 1
                });
            }

            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));

            // 🔥 Detta är nyckeln: returnera JSON med antal produkter i varukorgen
            return Json(new { totalItems = cart.Sum(i => i.Quantity) });
        }


        [HttpPost]
        public IActionResult RemoveFromCart(string productId)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
                SaveCartToSession(cart);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Index");
        }

        // Helpers
        private List<CartItem> GetCartFromSession()
        {
            string? cartJson = HttpContext.Session.GetString("Cart");
            return cartJson != null
                ? JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>()
                : new List<CartItem>();
        }

        private void SaveCartToSession(List<CartItem> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString("Cart", cartJson);
        }
    }
}
