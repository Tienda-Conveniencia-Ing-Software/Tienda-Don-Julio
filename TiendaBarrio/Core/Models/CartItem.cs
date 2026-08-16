namespace TiendaBarrio.Core.Models;

public class CartItem
{
    public Product Product { get; }
    public int Quantity { get; private set; }

    public CartItem(Product product, int quantity)
    {
        Product = product;
        Quantity = quantity;
    }

    public double Subtotal => Product.Price * Quantity;

    public void SetQuantity(int quantity)
    {
        if (quantity > 0)
        {
            Quantity = quantity;
        }
    }
}