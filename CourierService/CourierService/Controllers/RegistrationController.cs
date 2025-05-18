using DataBase.DataBase;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace CourierService.Controllers;

public class RegistrationController : Controller
{
    
    private readonly RegistrationService _service;
    private readonly CourierServiceContext _context;

    public RegistrationController(RegistrationService service,CourierServiceContext context)
    {
        _service = service;
        _context = context;
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