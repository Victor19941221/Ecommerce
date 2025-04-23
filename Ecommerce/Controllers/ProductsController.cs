// ProductsController.cs
using Ecommerce.Models;
using Ecommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IReviewService _reviewService;

        public ProductsController(IProductService productService, IReviewService reviewService)
        {
            _productService = productService;
            _reviewService = reviewService;
        }

        public async Task<IActionResult> Index(string? category)
        {
            var products = await _productService.GetAllAsync();

            // Ta ut unika kategorier för navbaren
            var categories = products
                .SelectMany(p => p.Categories ?? new List<string>())
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            // Filtrera produkter om en kategori valts
            if (!string.IsNullOrWhiteSpace(category))
            {
                products = products
                    .Where(p => (p.Categories ?? new List<string>()).Contains(category))
                    .ToList();
                ViewBag.CurrentCategory = category;
            }
            else
            {
                ViewBag.CurrentCategory = "All";
            }

            ViewBag.Categories = categories;

            return View(products);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price")] Product product, string CategoryInput)
        {
            if (!string.IsNullOrEmpty(CategoryInput))
            {
                product.Categories = CategoryInput
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => c.Trim())
                    .ToList();

                // Sätt Category automatiskt
                product.Category = product.Categories.FirstOrDefault() ?? string.Empty;
            }
            else
            {
                // Validering för att undvika tomma kategorier
                ModelState.AddModelError("CategoryInput", "Please enter at least one category.");
            }

            if (ModelState.IsValid)
            {
                await _productService.CreateAsync(product);
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(string id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Description,Price")] Product product, string CategoryInput)
        {
            if (id != product.Id) return NotFound();

            product.Categories = CategoryInput?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Trim())
                .ToList();

            if (ModelState.IsValid)
            {
                await _productService.UpdateAsync(id, product);
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _productService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();

            var reviews = await _reviewService.GetByProductIdAsync(id);
            var vm = new ProductDetailsViewModel
            {
                Product = product,
                Reviews = reviews,
                AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0
            };

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(string productId, int rating, string content)
        {
            var review = new Review
            {
                ProductId = productId,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                UserName = User.Identity?.Name ?? "User",
                Rating = rating,
                Content = content,
                Date = DateTime.UtcNow
            };

            await _reviewService.CreateAsync(review);
            return RedirectToAction("Details", new { id = productId });
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Crud()
        {
            var products = await _productService.GetAllAsync();
            return View(products); // → loads Views/Products/Crud.cshtml
        }

    }
}