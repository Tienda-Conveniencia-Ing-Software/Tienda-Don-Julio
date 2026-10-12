namespace TiendaBarrio.Core.Models;

public class CartItem
{
    public int Id { get; set; }                        // ← PK para EF Core
    public int ProductId { get; set; }                 // ← FK para EF Core
    public Product Product { get; set; }               // ← set agregado
    public int Quantity { get; set; }                  // ← set público
    public int? OrderId { get; set; }                  // ← FK opcional para Order

    // Constructor vacío (para EF Core)
    public CartItem()
    {
    }

    // Constructor con parámetros
    public CartItem(Product product, int quantity)
    {
        Product = product;
        ProductId = product.ID;
        Quantity = quantity;
    }

    public double Subtotal => Product != null ? Product.Price * Quantity : 0;

    public void SetQuantity(int quantity)
    {
        if (quantity > 0)
        {
            Quantity = quantity;
        }
    }
}
