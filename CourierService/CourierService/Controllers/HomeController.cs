using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CourierService.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    public IActionResult MainMenu()
    {
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }
    
}