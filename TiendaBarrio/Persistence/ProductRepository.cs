using Microsoft.EntityFrameworkCore;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Persistence.Data;
using TiendaBarrio.Persistence.Interfaces;

namespace TiendaBarrio.Persistence;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    // Constructor para Inyección de Dependencias
    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<Product> LoadProducts()
    {
        return _context.Productos.AsNoTracking().ToList();
    }

    public void SaveProducts(List<Product> products)
    {
        foreach (var product in products)
        {
            var existing = _context.Productos.FirstOrDefault(p => p.ID == product.ID);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(product);
            }
            else
            {
                _context.Productos.Add(product);
            }
        }
        _context.SaveChanges();
    }
}