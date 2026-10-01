namespace TiendaBarrio.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Persistence.Interfaces;

public class InventoryService
{
    private readonly IProductRepository _productRepository;
    private readonly FinanceService _financeService;
    private const int LowStockThreshold = 5;

    public InventoryService(IProductRepository productRepository, FinanceService financeService)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _financeService = financeService ?? throw new ArgumentNullException(nameof(financeService));
    }

    // Método privado para asegurar que la acción sea realizada por un trabajador
    private static void ValidarPermisoTrabajador(Cliente usuario)
    {
        if (usuario == null || !usuario.EsTrabajador())
        {
            throw new UnauthorizedAccessException("Acción denegada. Se requieren permisos de Trabajador para modificar el inventario.");
        }
    }

    public Product? FindProduct(List<Product> products, int id)
    {
        return products.FirstOrDefault(p => p.ID == id);
    }

    // Consulta pública: cualquier usuario o cliente puede listar/ver productos
    public List<Product> GetAllProducts(List<Product> products)
    {
        return products;
    }

    public List<Product> GetLowStockProducts(List<Product> products)
    {
        return products.Where(p => p.Stock <= LowStockThreshold).ToList();
    }

    // Acciones administrativas protegidas por rol
    public Product RegisterProduct(Cliente usuario, List<Product> products, string name, double price, double purchasePrice, int stock)
    {
        ValidarPermisoTrabajador(usuario);

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

    public bool UpdateProduct(Cliente usuario, List<Product> products, int id, string? newName, double? newPrice, double? newPurchasePrice)
    {
        ValidarPermisoTrabajador(usuario);

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

    public bool DeleteProduct(Cliente usuario, List<Product> products, int id)
    {
        ValidarPermisoTrabajador(usuario);

        var product = FindProduct(products, id);
        if (product == null)
        {
            return false;
        }

        products.Remove(product);
        _productRepository.SaveProducts(products);
        return true;
    }

    public bool IncreaseStock(Cliente usuario, List<Product> products, int id, int quantity)
    {
        ValidarPermisoTrabajador(usuario);

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

    public bool DecreaseStock(Cliente usuario, List<Product> products, int id, int quantity)
    {
        ValidarPermisoTrabajador(usuario);

        var product = FindProduct(products, id);
        if (product == null || quantity <= 0 || quantity > product.Stock)
        {
            return false;
        }

        product.ReduceStock(quantity);
        _productRepository.SaveProducts(products);
        return true;
    }
}