using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace CookieAuth.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        // Static list to store products (in-memory data)
        private static readonly List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Laptop", Price = 999.99m, Category = "Electronics" },
            new Product { Id = 2, Name = "Smartphone", Price = 699.99m, Category = "Electronics" },
            new Product { Id = 3, Name = "Headphones", Price = 149.99m, Category = "Accessories" },
            new Product { Id = 4, Name = "Coffee Mug", Price = 12.99m, Category = "Home" },
            new Product { Id = 5, Name = "Book", Price = 24.99m, Category = "Books" }
        };

        // GET: api/Products
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_products);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _products.Find(p => p.Id == id);
            if (product == null)
            {
                return NotFound(new { Message = $"Product with ID {id} not found" });
            }

            return Ok(product);
        }

        // GET: api/Products/category/Electronics
        [HttpGet("category/{category}")]
        public IActionResult GetByCategory(string category)
        {
            var products = _products.FindAll(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            return Ok(products);
        }

        // GET: api/Products/search?term=phone
        [HttpGet("search")]
        public IActionResult Search([FromQuery] string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return BadRequest(new { Message = "Search term is required" });
            }

            var products = _products.FindAll(p => 
                p.Name.Contains(term, StringComparison.OrdinalIgnoreCase) || 
                p.Category.Contains(term, StringComparison.OrdinalIgnoreCase));

            return Ok(products);
        }

        // POST: api/Products
        [HttpPost]
        [Authorize] // Only authenticated users can create products
        public IActionResult Create([FromBody] ProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newProduct = new Product
            {
                Id = _products.Count > 0 ? _products.Max(p => p.Id) + 1 : 1,
                Name = request.Name,
                Price = request.Price,
                Category = request.Category
            };

            _products.Add(newProduct);

            return CreatedAtAction(nameof(GetById), new { id = newProduct.Id }, newProduct);
        }
    }

    // Simple Product model
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
    }

    // DTO for creating products
    public class ProductRequest
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string Name { get; set; } = string.Empty;
        
        [System.ComponentModel.DataAnnotations.Range(0.01, 10000)]
        public decimal Price { get; set; }
        
        [System.ComponentModel.DataAnnotations.Required]
        public string Category { get; set; } = string.Empty;
    }
}