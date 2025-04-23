using Ecommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    
    public class AdminController : Controller
    {
        private readonly IOrderService _orderService;

        public AdminController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllAsync();
            return View(orders); // Views/Admin/Index.cshtml
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> MarkShipped(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            await _orderService.MarkOrderShippedAsync(id);
            return RedirectToAction("Index");
        }

    }
}
