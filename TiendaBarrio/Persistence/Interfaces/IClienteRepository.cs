using System.Collections.Generic;
using System.Threading.Tasks;
using TiendaBarrio.Core.Models;

namespace TiendaBarrio.Persistence.Interfaces;

public interface IClienteRepository
{
    Task<List<Cliente>> GetAllAsync();
    Task<Cliente?> GetByIdAsync(int id);
    Task AddAsync(Cliente cliente);
    Task UpdateAsync(Cliente cliente);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}