namespace TiendaBarrio.Core.Models;

public class Order
{
    public int Id { get; }
    public List<CartItem> Items { get; }
    public double Total { get; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; }

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