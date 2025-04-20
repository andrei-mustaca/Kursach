using DataBase.DataBase;
using DataBase.CRUD;
namespace Service;

public class RegistrationService
{
    private readonly CientCRUD _crud;

    public RegistrationService(CientCRUD crud)
    {
        _crud = crud;
    }

    public async Task Register(Client client)
    {
        await _crud.CreateAsync(client);
    }
}