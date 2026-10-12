namespace TiendaBarrio.Core.Models;

public class Order
{
    public int Id { get; set; }                       // ← set agregado
    public List<CartItem> Items { get; set; }         // ← set agregado
    public double Total { get; set; }                 // ← set agregado
    public OrderStatus Status { get; set; }           // ← set agregado
    public DateTime CreatedAt { get; set; }           // ← set agregado

    // Constructor vacío (para EF Core)
    public Order()
    {
        Items = new List<CartItem>();
        CreatedAt = DateTime.Now;
        Status = OrderStatus.Pendiente;
    }

    // Constructor con parámetros
    public Order(int id, List<CartItem> items)
    {
        Id = id;
        Items = items;
        Total = items.Sum(i => i.Subtotal);
        Status = OrderStatus.Pendiente;
        CreatedAt = DateTime.Now;
    }

    public void AdvanceStatus(OrderStatus newStatus)
    {
        Status = newStatus;
    }
}
