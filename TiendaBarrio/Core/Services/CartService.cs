namespace TiendaBarrio.Core.Services;

using TiendaBarrio.Core.Models;

public class CartService
{
    private readonly List<CartItem> _items = new();

    public List<CartItem> Items => _items;

    public void AddProduct(Product product, int quantity)
    {
        if (quantity <= 0 || quantity > product.Stock)
        {
            Console.WriteLine("Invalid quantity or not enough stock.");
            return;
        }

        var existing = _items.FirstOrDefault(i => i.Product.ID == product.ID);
        if (existing != null)
        {
            existing.SetQuantity(existing.Quantity + quantity);
        }
        else
        {
            _items.Add(new CartItem(product, quantity));
        }
    }

    public void UpdateQuantity(int productId, int newQuantity)
    {
        var item = _items.FirstOrDefault(i => i.Product.ID == productId);
        if (item == null)
        {
            Console.WriteLine("Product not found in cart.");
            return;
        }

        if (newQuantity <= 0)
        {
            _items.Remove(item);
            return;
        }

        if (newQuantity > item.Product.Stock)
        {
            Console.WriteLine("Not enough stock.");
            return;
        }

        item.SetQuantity(newQuantity);
    }

    public void RemoveItem(int productId)
    {
        var item = _items.FirstOrDefault(i => i.Product.ID == productId);
        if (item != null)
        {
            _items.Remove(item);
        }
    }

    public double CalculateTotal()
    {
        return _items.Sum(i => i.Subtotal);
    }

    public void Clear()
    {
        _items.Clear();
    }
}