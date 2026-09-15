namespace TiendaBarrio.Core.Models;

public class Product
{
    public int ID { get; }
    public string Name { get; }
    public double SPrice { get; private set; }
    public double BPrice { get; private set; }
    public int Stock { get; private set; }

    public Product(int id, string name, double sprice, double bprice, int stock)
    {
        ID = id;
        Name = name;

        // Validación: el precio de venta no puede ser negativo
        if (sprice < 0)
        {
            Console.WriteLine("Advertencia: El precio de venta no puede ser negativo. Se asignará 0.");
            SPrice = 0;
        }
        else
        {
            SPrice = sprice;
        }

        // Validación: el precio de compra no puede ser negativo
        if (bprice < 0)
        {
            Console.WriteLine("Advertencia: El precio de compra no puede ser negativo. Se asignará 0.");
            BPrice = 0;
        }
        else
        {
            BPrice = bprice;
        }

        // Validación: el stock no puede ser negativo
        if (stock < 0)
        {
            Console.WriteLine("Advertencia: El stock no puede ser negativo. Se asignará 0.");
        }
        else
        {
            Stock = stock;
        }
    }

    public void ReduceStock(int quantity)
    {
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