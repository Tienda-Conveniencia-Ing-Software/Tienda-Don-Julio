namespace TiendaBarrio.Core.Models;

public class Product
{
    public int ID { get; }
    public string Name { get; }
    public double Price { get; }           
    public double PurchasePrice { get; }   // NUEVO: Precio de compra (costo)
    public int Stock { get; private set; }

    public Product(int id, string name, double price, double purchasePrice, int stock)
    {
        ID = id;
        Name = name;
        
        // Validación: El precio de venta no puede ser negativo
        if (price < 0)
        {
            Console.WriteLine("Advertencia: El precio de venta no puede ser negativo. Se asignará 0.");
            Price = 0;
        }
        else
        {
            Price = price;
        }
        
        // Validación: El precio de compra no puede ser negativo
        if (purchasePrice < 0)
        {
            Console.WriteLine("Advertencia: El precio de compra no puede ser negativo. Se asignará 0.");
            PurchasePrice = 0;
        }
        else
        {
            PurchasePrice = purchasePrice;
        }
        
        // Validación: El stock no puede ser negativo
        if (stock < 0)
        {
            Console.WriteLine("Advertencia: El stock no puede ser negativo. Se asignará 0.");
            Stock = 0;
        }
        else
        {
            Stock = stock;
        }
    }
    
    public void ReduceStock(int quantity)
    {
        // Validación: Solo reducir si la cantidad es positiva y no supera el stock
        if (quantity > 0 && quantity <= Stock)
        {
            Stock -= quantity;
        }
        else if (quantity > Stock)
        {
            Console.WriteLine($"Error: No hay suficiente stock. Stock actual: {Stock}");
        }
    }
    
    public void IncreaseStock(int quantity)
    {
        // Validación: Solo aumentar si la cantidad es positiva
        if (quantity > 0)
        {
            Stock += quantity;
        }
        else
        {
            Console.WriteLine("Error: La cantidad a agregar debe ser mayor a 0.");
        }
    }
}
