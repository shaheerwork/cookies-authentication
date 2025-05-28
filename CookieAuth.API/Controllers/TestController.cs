using CookieAuth.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CookieAuth.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private static readonly List<TestItem> _testItems = new()
        {
            new TestItem { Id = 1, Name = "Test Item 1", Description = "Description for test item 1" },
            new TestItem { Id = 2, Name = "Test Item 2", Description = "Description for test item 2" },
            new TestItem { Id = 3, Name = "Test Item 3", Description = "Description for test item 3" }
        };

        // GET: api/Test
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_testItems);
        }

        // GET: api/Test/1
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _testItems.Find(i => i.Id == id);
            if (item == null)
            {
                return NotFound(new { Message = $"Item with ID {id} not found" });
            }

            return Ok(item);
        }

        // POST: api/Test
        [HttpPost]
        public IActionResult Create([FromBody] TestItemRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // In a real app, we would save this to a database
            var newItem = new TestItem
            {
                Id = _testItems.Count > 0 ? _testItems.Max(i => i.Id) + 1 : 1,
                Name = request.Name,
                Description = request.Description
            };

            _testItems.Add(newItem);

            return CreatedAtAction(nameof(GetById), new { id = newItem.Id }, newItem);
        }

        // PUT: api/Test/1
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] TestItemRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingItem = _testItems.Find(i => i.Id == id);
            if (existingItem == null)
            {
                return NotFound(new { Message = $"Item with ID {id} not found" });
            }

            existingItem.Name = request.Name;
            existingItem.Description = request.Description;

            return Ok(existingItem);
        }

        // DELETE: api/Test/1
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existingItem = _testItems.Find(i => i.Id == id);
            if (existingItem == null)
            {
                return NotFound(new { Message = $"Item with ID {id} not found" });
            }

            _testItems.Remove(existingItem);
            return Ok(new { Message = $"Item with ID {id} deleted successfully" });
        }

        // GET: api/Test/secure
        [HttpGet("secure")]
        [Authorize]
        public IActionResult GetSecure()
        {
            var userName = User.Identity?.Name ?? "Unknown User";
            return Ok(new { Message = $"Hello {userName}! This is a secure endpoint" });
        }
    }

    // Models for the TestController
    public class TestItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class TestItemRequest
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
    }
}