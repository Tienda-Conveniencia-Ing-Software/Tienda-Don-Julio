using System.Collections.Generic;
using System.Threading.Tasks;
using TiendaBarrio.Core.Models;

namespace TiendaBarrio.Persistence.Interfaces;

public interface ICashRepository
{
    Task<List<CashMovement>> GetAllAsync();
    Task<CashMovement?> GetByIdAsync(int id);
    Task AddAsync(CashMovement movimiento);
    Task UpdateAsync(CashMovement movimiento);
    Task DeleteAsync(int id);
    Task<double> GetTotalIngresosAsync();
    Task<double> GetTotalGastosAsync();
    Task<double> GetBalanceAsync();
    Task SaveChangesAsync();
}