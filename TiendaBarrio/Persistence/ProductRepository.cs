using Microsoft.EntityFrameworkCore;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Data;
using TiendaBarrio.Persistence.Interfaces;

namespace TiendaBarrio.Persistence;

public class ProductRepository : IProductRepository
{
    private readonly TiendaDbContext _context;

    // Constructor para Inyección de Dependencias
    public ProductRepository(TiendaDbContext context)
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

        // La lista recibida es el estado completo: se eliminan de la BD los productos que ya no estan
        var ids = products.Select(p => p.ID).ToHashSet();
        var removed = _context.Productos.Where(p => !ids.Contains(p.ID)).ToList();
        if (removed.Count > 0)
        {
            _context.Productos.RemoveRange(removed);
        }

        _context.SaveChanges();
    }
}