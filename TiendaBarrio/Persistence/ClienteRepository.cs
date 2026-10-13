using Microsoft.EntityFrameworkCore;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Persistence.Data;
using TiendaBarrio.Persistence.Interfaces;

namespace TiendaBarrio.Persistence;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Cliente>> GetAllAsync()
    {
        return await _context.Clientes.ToListAsync();
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        return await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddAsync(Cliente cliente)
    {
        await _context.Clientes.AddAsync(cliente);
    }

    public async Task UpdateAsync(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var cliente = await GetByIdAsync(id);
        if (cliente != null)
        {
            _context.Clientes.Remove(cliente);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}