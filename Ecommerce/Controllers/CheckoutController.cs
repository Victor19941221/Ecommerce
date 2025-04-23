using Ecommerce.Models;
using Ecommerce.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using System.Text.Json;

namespace Ecommerce.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IOrderService _orderService;

        public CheckoutController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public IActionResult Checkout(IFormCollection form)
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            var cart = cartJson != null
                ? JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>()
                : new List<CartItem>();

            if (!cart.Any()) return RedirectToAction("Index", "Cart");

            // Läs in adressen från formuläret
            var address = new Address
            {
                Street = form["Street"],
                PostalCode = form["PostalCode"],
                City = form["City"]
            };

            // Validera
            if (string.IsNullOrWhiteSpace(address.Street) ||
                string.IsNullOrWhiteSpace(address.PostalCode) ||
                string.IsNullOrWhiteSpace(address.City))
            {
                TempData["Error"] = "Please fill in all shipping address fields.";
                return RedirectToAction("Index", "Cart");
            }

            // Spara till session
            HttpContext.Session.SetString("ShippingAddress", JsonSerializer.Serialize(address));

            // Skapa Stripe-session
            var lineItems = cart.Select(item => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = item.Price * 100,
                    Currency = "sek",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Name
                    }
                },
                Quantity = item.Quantity
            }).ToList();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = Url.Action("Success", "Checkout", null, Request.Scheme) + "?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = Url.Action("Cancel", "Checkout", null, Request.Scheme)
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Redirect(session.Url);
        }

        public async Task<IActionResult> Success(string session_id)
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            var cartItems = !string.IsNullOrEmpty(cartJson)
                ? JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>()
                : new List<CartItem>();

            if (!cartItems.Any()) return RedirectToAction("Index", "Cart");

            // Hämta adress från session
            var addressJson = HttpContext.Session.GetString("ShippingAddress");
            var address = !string.IsNullOrEmpty(addressJson)
                ? JsonSerializer.Deserialize<Address>(addressJson)
                : new Address();

            var userId = User.Identity?.IsAuthenticated == true
                ? User.FindFirstValue(ClaimTypes.NameIdentifier)
                : null;

            var email = User.Identity?.IsAuthenticated == true
                ? User.FindFirstValue(ClaimTypes.Email)
                : "guest@example.com";

            var order = new Order
            {
                UserId = userId,
                UserEmail = email,
                StripeSessionId = session_id,
                IsPaid = true,
                OrderDate = DateTime.UtcNow,
                Items = cartItems.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    ProductName = i.Name,
                    UnitPrice = i.Price,
                    Quantity = i.Quantity
                }).ToList(),
                TotalAmount = cartItems.Sum(i => i.Price * i.Quantity),
                ShippingAddress = address
            };

            await _orderService.CreateAsync(order);
            HttpContext.Session.Remove("Cart");
            HttpContext.Session.Remove("ShippingAddress");

            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }
    }
}
