namespace TiendaBarrio.Core.Models;

public class CartItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }

    public double Subtotal => Quantity * (UnitPrice > 0 ? UnitPrice : (Product?.Price ?? 0));

    // Constructor sin parámetros para EF Core
    public CartItem() { }

    // Constructor de 2 argumentos para compatibilidad con CartService
    public CartItem(Product product, int quantity)
    {
        Product = product;
        ProductId = product?.ID ?? 0;
        Quantity = quantity;
        UnitPrice = product?.Price ?? 0;
    }

    // Método requerido por CartService.cs
    public void SetQuantity(int quantity)
    {
        if (quantity > 0)
        {
            Quantity = quantity;
        }
    }
}