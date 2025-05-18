using DataBase.DataBase;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace CourierService.Controllers;

public class CourierController : Controller
{
    private readonly Service.CourierService _courierService;

    public CourierController(Service.CourierService courierService)
    {
        _courierService = courierService;
    }
    // GET
    public IActionResult CourierRegister()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CourierRegister(Courier courier)
    {
        if (!ModelState.IsValid)
        {
            return View(courier);
        }

        await _courierService.CourierRegister(courier);
        return RedirectToAction("CourierMainMenu","Courier");
    }

    public IActionResult CourierMainMenu()
    {
        return View();
    }

    public IActionResult FindOrders()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> FindOrders(CourierViewModel courier)
    {
        var orders = await _courierService.GetClientOrdersAsync();
        return View("CourierSearchOrder",orders); 
    }

    
}