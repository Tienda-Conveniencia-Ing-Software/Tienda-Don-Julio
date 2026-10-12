using Microsoft.EntityFrameworkCore;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Data;
using TiendaBarrio.Persistence.Interfaces;

namespace TiendaBarrio.Persistence;

public class ProductRepository : IProductRepository
{
    private readonly TiendaDbContext _context;

    // Constructor sin parámetros (para compatibilidad con UI Consola y Servicios)
    public ProductRepository()
    {
        _context = new TiendaDbContext();
    }

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
        _context.SaveChanges();
    }
}