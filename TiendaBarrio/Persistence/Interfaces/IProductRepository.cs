using System.Collections.Generic;
using System.Threading.Tasks;
using TiendaBarrio.Core.Models;

namespace TiendaBarrio.Persistence.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}