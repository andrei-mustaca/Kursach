using DataBase.DataBase;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace CourierService.Controllers;

public class RegistrationController : Controller
{
    
    private readonly RegistrationService _service;

    public RegistrationController(RegistrationService service)
    {
        _service = service;
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(Client client)
    {
        if (!ModelState.IsValid)
            return View(client);
        await _service.Register(client);
        return RedirectToAction("MainMenu","Home");
    }
}