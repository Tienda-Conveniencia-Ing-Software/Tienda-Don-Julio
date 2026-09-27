namespace TiendaBarrio.Inventario;

using TiendaBarrio.Core.Models;
using TiendaBarrio.Core.Services;
using TiendaBarrio.Persistence;

public class InventoryService
{
    private readonly ProductRepository _productRepository;
    private readonly FinanceService _financeService;
    private const int LowStockThreshold = 5;

    public InventoryService(ProductRepository productRepository, FinanceService financeService)
    {
        _productRepository = productRepository;
        _financeService = financeService;
    }

    public Product? FindProduct(List<Product> products, int id)
    {
        return products.FirstOrDefault(p => p.ID == id);
    }

    public Product RegisterProduct(List<Product> products, string name, double price, double purchasePrice, int stock)
    {
        int id = products.Count > 0 ? products.Max(p => p.ID) + 1 : 1;
        var product = new Product(id, name, price, purchasePrice, stock);
        products.Add(product);

        _productRepository.SaveProducts(products);

        if (stock > 0)
        {
            double cost = purchasePrice * stock;
            _financeService.RegisterInventoryPurchase(cost, $"Producto nuevo: {name.Trim()} x{stock}");
        }

        return product;
    }

    public bool UpdateProduct(List<Product> products, int id, string? newName, double? newPrice, double? newPurchasePrice)
    {
        var product = FindProduct(products, id);
        if (product == null)
        {
            return false;
        }

        int index = products.IndexOf(product);

        string finalName = newName ?? product.Name;
        double finalPrice = newPrice ?? product.Price;
        double finalPurchasePrice = newPurchasePrice ?? product.PurchasePrice;

        var updated = new Product(product.ID, finalName, finalPrice, finalPurchasePrice, product.Stock);
        products[index] = updated;

        _productRepository.SaveProducts(products);
        return true;
    }

    public bool DeleteProduct(List<Product> products, int id)
    {
        var product = FindProduct(products, id);
        if (product == null)
        {
            return false;
        }

        products.Remove(product);
        _productRepository.SaveProducts(products);
        return true;
    }

    public List<Product> GetAllProducts(List<Product> products)
    {
        return products;
    }

    public bool IncreaseStock(List<Product> products, int id, int quantity)
    {
        var product = FindProduct(products, id);
        if (product == null || quantity <= 0)
        {
            return false;
        }

        product.IncreaseStock(quantity);
        _productRepository.SaveProducts(products);

        double cost = product.PurchasePrice * quantity;
        _financeService.RegisterInventoryPurchase(cost, $"Reposicion de stock: {product.Name} x{quantity}");

        return true;
    }

    public bool DecreaseStock(List<Product> products, int id, int quantity)
    {
        var product = FindProduct(products, id);
        if (product == null || quantity <= 0 || quantity > product.Stock)
        {
            return false;
        }

        product.ReduceStock(quantity);
        _productRepository.SaveProducts(products);
        return true;
    }

    public List<Product> GetLowStockProducts(List<Product> products)
    {
        return products.Where(p => p.Stock <= LowStockThreshold).ToList();
    }
}