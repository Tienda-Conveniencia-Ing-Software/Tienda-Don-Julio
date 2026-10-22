namespace TiendaBarrio.Core.Models;

public class Order
{
    public int Id { get; set; }
    public int ClienteId { get; set; } // ← Propiedad añadida
    public List<CartItem> Items { get; set; }
    public double Total { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public Order()
    {
        Items = new List<CartItem>();
        CreatedAt = DateTime.Now;
        Status = OrderStatus.Pendiente;
    }

    public Order(int id, List<CartItem> items, int clienteId = 0)
    {
        Id = id;
        ClienteId = clienteId;
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