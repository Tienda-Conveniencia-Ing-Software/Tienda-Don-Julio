namespace TiendaBarrio.Core.Models;

public class Product
{
    public int ID { get; }
    public string Name { get; }
    public double Price { get; }
    public int Stock { get; private set; }

    public Product(int id, string name, double price, int stock)
    {
        ID = id;
        Name = name;
        Price = price;
        Stock = stock;
    }
    public void ReduceStock(int quantity)
    {
        if (quantity > 0 && quantity <= Stock)
        {
            Stock -= quantity;
        }
    }
    public void IncreaseStock(int quantity)
    {
        if (quantity > 0)
        {
            Stock += quantity;
        }
    }

}
