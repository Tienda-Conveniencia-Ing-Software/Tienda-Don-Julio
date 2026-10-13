namespace TiendaBarrio.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Persistence.Interfaces;

public class FinanceService
{
    private readonly ICashRepository _repository;

    public FinanceService(ICashRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<CashMovement> RegisterIncomeAsync(double amount, string description)
    {
        return await RegisterMovementAsync(MovementType.Ingreso, amount, description);
    }

    public async Task<CashMovement> RegisterExpenseAsync(double amount, string description)
    {
        return await RegisterMovementAsync(MovementType.Gasto, amount, description);
    }

    public async Task<CashMovement> RegisterInventoryPurchaseAsync(double amount, string description)
    {
        return await RegisterMovementAsync(MovementType.CompraInventario, amount, description);
    }

    private async Task<CashMovement> RegisterMovementAsync(MovementType type, double amount, string description)
    {
        // ✅ Ya NO calculamos el ID manualmente. La BD lo asigna.
        var movement = new CashMovement
        {
            Type = type,
            Amount = amount,
            Description = description,
            Date = DateTime.Now
        };

        await _repository.AddAsync(movement);
        await _repository.SaveChangesAsync();

        return movement;
    }

    public async Task<Balance> GetBalanceAsync()
    {
        var movements = await _repository.GetAllAsync();

        double totalIncome = movements
            .Where(m => m.Type == MovementType.Ingreso)
            .Sum(m => m.Amount);

        double totalExpense = movements
            .Where(m => m.Type == MovementType.Gasto || m.Type == MovementType.CompraInventario)
            .Sum(m => m.Amount);

        return new Balance(totalIncome, totalExpense);
    }

    public async Task<List<CashMovement>> GetHistoryAsync()
    {
        return await _repository.GetAllAsync();
    }
}