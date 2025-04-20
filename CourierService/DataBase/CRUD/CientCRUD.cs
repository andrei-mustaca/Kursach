using DataBase.DataBase;
using Microsoft.EntityFrameworkCore;

namespace DataBase.CRUD;

public class CientCRUD
{
    private readonly CourierServiceContext _context;

    public CientCRUD(CourierServiceContext context)
    {
        _context = context;
    }

    public async Task<List<Client>> GetAllAsync() =>
        await _context.Clients.ToListAsync();

    public async Task<Client?> GetByIdAsync(Guid id) =>
        await _context.Clients.FindAsync(id);

    public async Task CreateAsync(Client user)
    {
        user.Id = Guid.NewGuid();
        _context.Clients.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Client user)
    {
        _context.Clients.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _context.Clients.FindAsync(id);
        if (user != null)
        {
            _context.Clients.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}