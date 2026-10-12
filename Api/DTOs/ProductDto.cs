namespace Api.DTOs;

public class ProductDto
{
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public double PurchasePrice { get; set; }
    public int Stock { get; set; }
}