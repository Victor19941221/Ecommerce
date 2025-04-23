using Ecommerce.Models;
using Ecommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace Ecommerce.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> MyOrders()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _orderService.GetByUserIdAsync(userId);
            return View(orders);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(string? stripeSessionId, string? emailProvided)
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            var cartItems = !string.IsNullOrEmpty(cartJson)
                ? JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>()
                : new List<CartItem>();

            if (!cartItems.Any()) return RedirectToAction("Index", "Cart");

            string? userId = User.Identity?.IsAuthenticated == true
                ? User.FindFirstValue(ClaimTypes.NameIdentifier)
                : null;

            string? email = User.Identity?.IsAuthenticated == true
                ? User.FindFirstValue(ClaimTypes.Email)
                : emailProvided;

            // Read address from form
            var form = HttpContext.Request.Form;
            var address = new Address
            {
                Street = form["Street"],
                PostalCode = form["PostalCode"],
                City = form["City"]
            };

            var order = new Order
            {
                UserId = userId,
                UserEmail = email,
                StripeSessionId = stripeSessionId,
                IsPaid = true,
                OrderDate = DateTime.UtcNow,
                Items = cartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Name,
                    UnitPrice = ci.Price,
                    Quantity = ci.Quantity
                }).ToList(),
                TotalAmount = cartItems.Sum(ci => ci.Price * ci.Quantity),
                ShippingAddress = address
            };

            await _orderService.CreateAsync(order);
            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Success");
        }

        public IActionResult Success() => View();
   

        


    }
}
