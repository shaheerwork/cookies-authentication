using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CookieAuth.Web.Models;
using Microsoft.AspNetCore.Authorization;
using CookieAuth.Web.Services;

namespace CookieAuth.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IAuthService _authService;

    public HomeController(ILogger<HomeController> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await _authService.GetCurrentUserAsync();
        return View(user);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
