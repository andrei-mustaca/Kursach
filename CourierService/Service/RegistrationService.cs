using DataBase.DataBase;
using DataBase.CRUD;
using Microsoft.EntityFrameworkCore;

namespace Service;

public class RegistrationService
{
    private readonly CientCRUD _crud;
    private readonly CourierServiceContext _context;
    public RegistrationService(CientCRUD crud, CourierServiceContext context)
    {
        _crud = crud;
        _context = context;
    }

    public async Task Register(Client client)
    {
        if (client.FullName==_context.Clients.FirstOrDefault(c=>c.FullName==client.FullName)?.FullName)
        {
            
        }
        else
        {
            await _crud.CreateAsync(client); 
        }
    }
}