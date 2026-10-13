using Microsoft.EntityFrameworkCore;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Persistence.Data;
using TiendaBarrio.Persistence.Interfaces;

namespace TiendaBarrio.Persistence;

public class CashRepository : ICashRepository
{
    private readonly AppDbContext _context;

    public CashRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CashMovement>> GetAllAsync()
    {
        return await _context.Movimientos.ToListAsync();
    }

    public async Task<CashMovement?> GetByIdAsync(int id)
    {
        return await _context.Movimientos.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task AddAsync(CashMovement movimiento)
    {
        await _context.Movimientos.AddAsync(movimiento);
    }

    public async Task UpdateAsync(CashMovement movimiento)
    {
        _context.Movimientos.Update(movimiento);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var movimiento = await GetByIdAsync(id);
        if (movimiento != null)
        {
            _context.Movimientos.Remove(movimiento);
        }
    }

    public async Task<double> GetTotalIngresosAsync()
    {
        return await _context.Movimientos
            .Where(m => m.Type == MovementType.Ingreso)
            .SumAsync(m => m.Amount);
    }

    public async Task<double> GetTotalGastosAsync()
    {
        return await _context.Movimientos
            .Where(m => m.Type == MovementType.Gasto)
            .SumAsync(m => m.Amount);
    }

    public async Task<double> GetBalanceAsync()
    {
        var ingresos = await GetTotalIngresosAsync();
        var gastos = await GetTotalGastosAsync();
        return ingresos - gastos;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}