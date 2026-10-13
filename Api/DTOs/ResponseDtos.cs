namespace Api.DTOs;

using TiendaBarrio.Core.Models;

public record ProductResponseDto(int Id, string Name, double Price, double PurchasePrice, int Stock)
{
    public static ProductResponseDto From(Product p) => new(p.ID, p.Name, p.Price, p.PurchasePrice, p.Stock);
}

public record MovementResponseDto(int Id, MovementType Type, double Amount, string Description, DateTime Date)
{
    public static MovementResponseDto From(CashMovement m) => new(m.Id, m.Type, m.Amount, m.Description, m.Date);
}

public record BalanceResponseDto(double TotalIncome, double TotalExpense, double CurrentBalance)
{
    public static BalanceResponseDto From(Balance b) => new(b.TotalIncome, b.TotalExpense, b.CurrentBalance);
}

public record MessageDto(string Message);
