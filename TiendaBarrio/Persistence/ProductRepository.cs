using Microsoft.EntityFrameworkCore;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Persistence.Data;
using TiendaBarrio.Persistence.Interfaces;

namespace TiendaBarrio.Persistence;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Productos.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Productos.FirstOrDefaultAsync(p => p.ID == id);
    }

    public async Task AddAsync(Product product)
    {
        await _context.Productos.AddAsync(product);
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Productos.Update(product);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var product = await GetByIdAsync(id);
        if (product != null)
        {
            _context.Productos.Remove(product);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}