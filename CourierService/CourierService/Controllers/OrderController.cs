using Microsoft.AspNetCore.Mvc;
using Models;
using Service;

namespace CourierService.Controllers;

public class OrderController : Controller
{
    private readonly OrderService _service;

    public OrderController(OrderService service)
    {
        _service = service;
    }
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(OrderViewModel model)
    {
        if (ModelState.IsValid)
        {
            bool success = await _service.CreateOrder(model);

            if (success)
            {
                return RedirectToAction("MainMenu", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Ошибка при создании заказа.");
            }
        }

        return View(model);
    }
}