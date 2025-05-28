using CookieAuth.API.Services;
using CookieAuth.Shared.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CookieAuth.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.RegisterUserAsync(request);

            if (!result.Succeeded)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.ValidateUserAsync(request);

            if (!result.Succeeded || result.User == null)
                return Unauthorized(result.Message);

            // Create claims for the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.User.Id),
                new Claim(ClaimTypes.Email, result.User.Email),
                new Claim(ClaimTypes.Name, result.User.FullName)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Set authentication properties
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = request.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(request.RememberMe ? 30 : 1)
            };

            // Sign in the user
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { Message = "Logged out successfully" });
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userService.GetUserByIdAsync(userId);
            
            if (user == null)
                return NotFound();

            return Ok(user);
        }
    }
}
