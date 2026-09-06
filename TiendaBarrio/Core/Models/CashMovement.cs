namespace TiendaBarrio.Core.Models;

public class CashMovement
{
    public int Id { get; }
    public MovementType Type { get; }
    public double Amount { get; }
    public string Description { get; }
    public DateTime Date { get; }

    public CashMovement(int id, MovementType type, double amount, string description)
    {
        Id = id;
        Type = type;
        Amount = amount;
        Description = description;
        Date = DateTime.Now;
    }
}