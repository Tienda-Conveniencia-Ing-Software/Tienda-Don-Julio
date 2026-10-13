using System.Collections.Generic;
using System.Threading.Tasks;
using TiendaBarrio.Core.Models;

namespace TiendaBarrio.Persistence.Interfaces;

public interface ISaleRepository
{
    Task<List<Sale>> GetAllAsync();
    Task<Sale?> GetByIdAsync(int id);
    Task AddAsync(Sale sale);
    Task SaveChangesAsync();
}