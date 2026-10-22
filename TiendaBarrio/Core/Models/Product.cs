public class Product
{
    public int ID { get; set; }                        // ← set agregado
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }                  // ← set agregado
    public double PurchasePrice { get; set; }          // ← set agregado
    public int Stock { get; set; }                     // ← set agregado

    // Constructor vacío (para EF Core)
    public Product()
    {
    }

    // Constructor con parámetros
    public Product(int id, string name, double price, double purchasePrice, int stock)
    {
        ID = id;
        Name = name;
        Price = price < 0 ? 0 : price;
        PurchasePrice = purchasePrice < 0 ? 0 : purchasePrice;
        Stock = stock < 0 ? 0 : stock;
    }

    public void ReduceStock(int quantity)
    {
        if (quantity > 0 && quantity <= Stock)
            Stock -= quantity;
        else if (quantity > Stock)
            Console.WriteLine($"Error: No hay suficiente stock. Stock actual: {Stock}");
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity > 0)
            Stock += quantity;
        else
            Console.WriteLine("Error: La cantidad a agregar debe ser mayor a 0.");
    }
}
