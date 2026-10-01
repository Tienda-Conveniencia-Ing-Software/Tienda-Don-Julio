namespace TiendaBarrio.Core.Models;

public class CashMovement
{
    public int Id { get; set; }
    public MovementType Type { get; set; }
    public double Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }

    // Constructor sin parámetros necesario para Entity Framework Core
    public CashMovement()
    {
        Date = DateTime.Now;
    }

    public CashMovement(int id, MovementType type, double amount, string description)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("El monto del movimiento debe ser mayor a 0.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("La descripción no puede estar vacía.");
        }

        Id = id;
        Type = type;
        Amount = amount;
        Description = description;
        Date = DateTime.Now;
    }
}