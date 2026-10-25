using TiendaBarrio.Web.Models;

namespace TiendaBarrio.Web.Services;

public class CartState
{
    private readonly List<CartLine> _items = new();

    public IReadOnlyList<CartLine> Items => _items.AsReadOnly();
    public event Action? OnChange;

    public void AddProduct(ProductItem product, int quantity = 1)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existing != null)
        {
            var newQty = existing.Quantity + quantity;
            existing.Quantity = newQty > product.Stock ? product.Stock : newQty;
        }
        else
        {
            _items.Add(new CartLine
            {
                ProductId = product.Id,
                Name = product.Name,
                UnitPrice = product.Price,
                Quantity = Math.Min(quantity, product.Stock),
                MaxStock = product.Stock
            });
        }
        NotifyStateChanged();
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            if (quantity <= 0)
                _items.Remove(item);
            else
                item.Quantity = Math.Min(quantity, item.MaxStock);

            NotifyStateChanged();
        }
    }

    public void RemoveItem(int productId)
    {
        _items.RemoveAll(i => i.ProductId == productId);
        NotifyStateChanged();
    }

    public void Clear()
    {
        _items.Clear();
        NotifyStateChanged();
    }

    public double Total => _items.Sum(i => i.Subtotal);
    public int Count => _items.Sum(i => i.Quantity);

    private void NotifyStateChanged() => OnChange?.Invoke();
}