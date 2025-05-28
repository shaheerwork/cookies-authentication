using CookieAuth.Shared.Models;
using CookieAuth.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace CookieAuth.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model, string returnUrl = "/")
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authService.LoginAsync(model);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Invalid login attempt");
                return View(model);
            }

            return LocalRedirect(returnUrl);
        }

        [HttpGet]
        public IActionResult Register(string returnUrl = "/")
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest model, string returnUrl = "/")
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var result = await _authService.RegisterAsync(model);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Registration failed");
                return View(model);
            }

            // Automatically log in after registration
            var loginResult = await _authService.LoginAsync(new LoginRequest
            {
                Email = model.Email,
                Password = model.Password
            });

            if (!loginResult.Succeeded)
            {
                return RedirectToAction(nameof(Login), new { returnUrl });
            }

            return LocalRedirect(returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
