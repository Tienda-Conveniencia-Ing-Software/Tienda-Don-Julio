namespace TiendaBarrio.UI;

using TiendaBarrio.Core.Models;
using TiendaBarrio.Core.Services;
using TiendaBarrio.Persistence;
using TiendaBarrio.Utils;

public class InventoryMenu
{
    private readonly InventoryService _inventoryService;

    public InventoryMenu(FinanceService financeService)
    {
        _inventoryService = new InventoryService(new ProductRepository(), financeService);
    }

    public void Start(List<Product> products)
    {
        bool exit = true;
        while (exit)
        {
            Console.Clear();
            Console.WriteLine("// INVENTORY \\\\");
            Console.WriteLine("0. Back");
            Console.WriteLine("1. View products");
            Console.WriteLine("2. Register new product");
            Console.WriteLine("3. Update product");
            Console.WriteLine("4. Delete product");
            Console.WriteLine("5. Increase stock");
            Console.WriteLine("6. Decrease stock");
            Console.WriteLine("7. Low stock alert");
            Console.WriteLine("\nSelect an option: ");

            if (!int.TryParse(Console.ReadLine(), out int option))
            {
                Console.WriteLine("Invalid input.");
                new Pause().pause();
                continue;
            }

            switch (option)
            {
                case 0:
                    exit = false;
                    break;

                case 1:
                    ShowProductsFlow(products);
                    new Pause().pause();
                    break;

                case 2:
                    RegisterProductFlow(products);
                    new Pause().pause();
                    break;

                case 3:
                    UpdateProductFlow(products);
                    new Pause().pause();
                    break;

                case 4:
                    DeleteProductFlow(products);
                    new Pause().pause();
                    break;

                case 5:
                    IncreaseStockFlow(products);
                    new Pause().pause();
                    break;

                case 6:
                    DecreaseStockFlow(products);
                    new Pause().pause();
                    break;

                case 7:
                    LowStockAlertFlow(products);
                    new Pause().pause();
                    break;

                default:
                    Console.WriteLine("Option not available");
                    new Pause().pause();
                    break;
            }
        }
    }

    private void ShowProductsFlow(List<Product> products)
    {
        var all = _inventoryService.GetAllProducts(products);
        if (all.Count == 0)
        {
            Console.WriteLine("No products registered.");
            return;
        }

        foreach (var p in all)
        {
            Console.WriteLine($"[{p.ID}] {p.Name} - Sale: {p.Price}$ - Purchase: {p.PurchasePrice}$ - Stock: {p.Stock}");
        }
    }

    private void RegisterProductFlow(List<Product> products)
    {
        Console.WriteLine("Product name:");
        string name = Console.ReadLine() ?? "";

        Console.WriteLine("Sale price:");
        if (!double.TryParse(Console.ReadLine(), out double price) || price < 0)
        {
            Console.WriteLine("Invalid sale price.");
            return;
        }

        Console.WriteLine("Purchase price:");
        if (!double.TryParse(Console.ReadLine(), out double purchasePrice) || purchasePrice < 0)
        {
            Console.WriteLine("Invalid purchase price.");
            return;
        }

        Console.WriteLine("Initial stock:");
        if (!int.TryParse(Console.ReadLine(), out int stock) || stock < 0)
        {
            Console.WriteLine("Invalid stock.");
            return;
        }

        var product = _inventoryService.RegisterProduct(products, name, price, purchasePrice, stock);
        Console.WriteLine($"Product registered: [{product.ID}] {product.Name}");
    }

    private void UpdateProductFlow(List<Product> products)
    {
        Console.WriteLine("Enter product ID to update:");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        Console.WriteLine("New name (leave empty to keep current):");
        string nameInput = Console.ReadLine() ?? "";
        string? newName = string.IsNullOrWhiteSpace(nameInput) ? null : nameInput;

        Console.WriteLine("New sale price (leave empty to keep current):");
        string priceInput = Console.ReadLine() ?? "";
        double? newPrice = double.TryParse(priceInput, out double parsedPrice) ? parsedPrice : null;

        Console.WriteLine("New purchase price (leave empty to keep current):");
        string purchaseInput = Console.ReadLine() ?? "";
        double? newPurchasePrice = double.TryParse(purchaseInput, out double parsedPurchase) ? parsedPurchase : null;

        bool success = _inventoryService.UpdateProduct(products, id, newName, newPrice, newPurchasePrice);
        Console.WriteLine(success ? "Product updated." : "Product not found.");
    }

    private void DeleteProductFlow(List<Product> products)
    {
        Console.WriteLine("Enter product ID to delete:");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        bool success = _inventoryService.DeleteProduct(products, id);
        Console.WriteLine(success ? "Product deleted." : "Product not found.");
    }

    private void IncreaseStockFlow(List<Product> products)
    {
        Console.WriteLine("Enter product ID:");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        Console.WriteLine("Quantity to add:");
        if (!int.TryParse(Console.ReadLine(), out int quantity))
        {
            Console.WriteLine("Invalid quantity.");
            return;
        }

        bool success = _inventoryService.IncreaseStock(products, id, quantity);
        Console.WriteLine(success ? "Stock increased." : "Could not increase stock (check ID or quantity).");
    }

    private void DecreaseStockFlow(List<Product> products)
    {
        Console.WriteLine("Enter product ID:");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        Console.WriteLine("Quantity to remove:");
        if (!int.TryParse(Console.ReadLine(), out int quantity))
        {
            Console.WriteLine("Invalid quantity.");
            return;
        }

        bool success = _inventoryService.DecreaseStock(products, id, quantity);
        Console.WriteLine(success ? "Stock decreased." : "Could not decrease stock (check ID, quantity or available stock).");
    }

    private void LowStockAlertFlow(List<Product> products)
    {
        var lowStock = _inventoryService.GetLowStockProducts(products);
        if (lowStock.Count == 0)
        {
            Console.WriteLine("No products with low stock.");
            return;
        }

        Console.WriteLine("Products with low stock:");
        foreach (var p in lowStock)
        {
            Console.WriteLine($"[{p.ID}] {p.Name} - Stock: {p.Stock}");
        }
    }
}