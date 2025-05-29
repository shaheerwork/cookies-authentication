using Microsoft.AspNetCore.Mvc;
using CookieAuth.Web.Services;
using Microsoft.AspNetCore.Authorization;

namespace CookieAuth.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: /Products
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        // GET: /Products/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: /Products/Category/Electronics
        public async Task<IActionResult> Category(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(nameof(Index));
            }

            var products = await _productService.GetProductsByCategoryAsync(id);
            ViewData["Category"] = id;
            return View(products);
        }

        // GET: /Products/Search?term=phone
        public async Task<IActionResult> Search(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return RedirectToAction(nameof(Index));
            }

            var products = await _productService.SearchProductsAsync(term);
            ViewData["SearchTerm"] = term;
            return View(products);
        }

        // GET: /Products/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(ProductRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var product = await _productService.CreateProductAsync(model);
            
            if (product == null)
            {
                ModelState.AddModelError(string.Empty, "Failed to create product. Please try again.");
                return View(model);
            }

            return RedirectToAction(nameof(Details), new { id = product.Id });
        }
    }
}