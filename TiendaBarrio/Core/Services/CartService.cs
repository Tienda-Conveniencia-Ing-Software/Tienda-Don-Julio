namespace TiendaBarrio.Core.Services;

using TiendaBarrio.Core.Models;

public class CartService
{
    private readonly List<CartItem> _items = new();

    public List<CartItem> Items => _items;

    public bool AddProduct(Product product, int quantity)
    {
        if (quantity <= 0 || quantity > product.Stock)
        {
            return false;
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

        return true;
    }

    public bool UpdateQuantity(int productId, int newQuantity)
    {
        var item = _items.FirstOrDefault(i => i.Product.ID == productId);

        if (item == null)
        {
            return false;
        }

        if (newQuantity <= 0)
        {
            _items.Remove(item);
            return true;
        }

        if (newQuantity > item.Product.Stock)
        {
            return false;
        }

        item.SetQuantity(newQuantity);
        return true;
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