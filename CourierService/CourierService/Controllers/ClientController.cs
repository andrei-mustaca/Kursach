using DataBase.DataBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Service;

namespace CourierService.Controllers;

public class ClientController : Controller
{
    private readonly CourierServiceContext _context;
    private readonly OrderService _orderService;

    public ClientController(CourierServiceContext context,OrderService orderService)
    {
        _context = context;
        _orderService = orderService;
    }

    public IActionResult FindOrders()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> FindOrders(ClientSearchViewModel model)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c =>
            c.FullName == model.FullName &&
            c.TelephoneNumber == model.TelephoneNumber &&
            c.Email == model.Email);

        if (client == null)
        {
            ModelState.AddModelError("", "Клиент не найден.");
            return View(model);
        }

        var orders = await _orderService.GetClientOrdersAsync(client.Id);
        return View("ClientOrders", orders);
    }

}