namespace TiendaBarrio.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Persistence.Interfaces;

public class FinanceService
{
    private readonly ICashRepository _repository;
    private int _nextId = 1;

    // Inyección de dependencias para desacoplar el repositorio
    public FinanceService(ICashRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        var existing = _repository.LoadMovements();
        if (existing != null && existing.Count > 0)
        {
            _nextId = existing.Max(m => m.Id) + 1;
        }
    }

    public CashMovement RegisterIncome(double amount, string description)
    {
        return RegisterMovement(MovementType.Ingreso, amount, description);
    }

    public CashMovement RegisterExpense(double amount, string description)
    {
        return RegisterMovement(MovementType.Gasto, amount, description);
    }

    public CashMovement RegisterInventoryPurchase(double amount, string description)
    {
        return RegisterMovement(MovementType.CompraInventario, amount, description);
    }

    private CashMovement RegisterMovement(MovementType type, double amount, string description)
    {
        var movement = new CashMovement(_nextId, type, amount, description);
        _nextId++;
        _repository.SaveMovement(movement);
        return movement;
    }

    public Balance GetBalance()
    {
        var movements = _repository.LoadMovements();

        double totalIncome = movements
            .Where(m => m.Type == MovementType.Ingreso)
            .Sum(m => m.Amount);

        double totalExpense = movements
            .Where(m => m.Type == MovementType.Gasto || m.Type == MovementType.CompraInventario)
            .Sum(m => m.Amount);

        return new Balance(totalIncome, totalExpense);
    }

    public List<CashMovement> GetHistory()
    {
        return _repository.LoadMovements();
    }
}