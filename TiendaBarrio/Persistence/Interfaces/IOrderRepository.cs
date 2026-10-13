using System.Collections.Generic;
using System.Threading.Tasks;
using TiendaBarrio.Core.Models;

namespace TiendaBarrio.Persistence.Interfaces;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(int id);
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
    Task SaveChangesAsync();
}
