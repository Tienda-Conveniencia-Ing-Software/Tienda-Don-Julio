namespace TiendaBarrio.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    private static void ValidarPermisoTrabajador(Cliente usuario)
    {
        if (usuario == null || !usuario.EsTrabajador())
        {
            throw new UnauthorizedAccessException("Acción denegada. Se requieren permisos de Trabajador para modificar el inventario.");
        }
    }

    // Lectura directa desde el repositorio (usada por la API para no tocar repositorios en los controllers)
public async Task<List<Product>> GetProductsAsync()
{
    return await _productRepository.GetAllAsync();
}

    public Product? FindProduct(List<Product> products, int id)
    {
        return products.FirstOrDefault(p => p.ID == id);
    }

    public List<Product> GetAllProducts(List<Product> products)
    {
        return products;
    }

    public List<Product> GetLowStockProducts(List<Product> products)
    {
        return products.Where(p => p.Stock <= LowStockThreshold).ToList();
    }

    public async Task<Product> RegisterProductAsync(Cliente usuario, List<Product> products, string name, double price, double purchasePrice, int stock)
    {
        ValidarPermisoTrabajador(usuario);

        int id = products.Count > 0 ? products.Max(p => p.ID) + 1 : 1;
        var product = new Product(id, name, price, purchasePrice, stock);

        // ✅ Guardar con EF Core
        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        products.Add(product);

        if (stock > 0)
        {
            double cost = purchasePrice * stock;
            await _financeService.RegisterInventoryPurchaseAsync(cost, $"Producto nuevo: {name.Trim()} x{stock}");
        }

        return product;
    }

    public async Task<bool> UpdateProductAsync(Cliente usuario, List<Product> products, int id, string? newName, double? newPrice, double? newPurchasePrice)
    {
        ValidarPermisoTrabajador(usuario);

        var product = FindProduct(products, id);
        if (product == null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(newName)) product.Name = newName;
        if (newPrice.HasValue) product.Price = newPrice.Value;
        if (newPurchasePrice.HasValue) product.PurchasePrice = newPurchasePrice.Value;

        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteProductAsync(Cliente usuario, List<Product> products, int id)
    {
        ValidarPermisoTrabajador(usuario);

        var product = FindProduct(products, id);
        if (product == null)
        {
            return false;
        }

        await _productRepository.DeleteAsync(id);
        await _productRepository.SaveChangesAsync();

        products.Remove(product);
        return true;
    }

    public async Task<bool> IncreaseStockAsync(Cliente usuario, List<Product> products, int id, int quantity)
    {
        ValidarPermisoTrabajador(usuario);

        var product = FindProduct(products, id);
        if (product == null || quantity <= 0)
        {
            return false;
        }

        product.IncreaseStock(quantity);
        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();

        double cost = product.PurchasePrice * quantity;
        await _financeService.RegisterInventoryPurchaseAsync(cost, $"Reposicion de stock: {product.Name} x{quantity}");

        return true;
    }

    public async Task<bool> DecreaseStockAsync(Cliente usuario, List<Product> products, int id, int quantity)
    {
        ValidarPermisoTrabajador(usuario);

        var product = FindProduct(products, id);
        if (product == null || quantity <= 0 || quantity > product.Stock)
        {
            return false;
        }

        product.ReduceStock(quantity);
        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();

        return true;
    }
}