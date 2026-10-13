namespace Api.DTOs;

using System.ComponentModel.DataAnnotations;

public class ProductDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "El precio de venta no puede ser negativo.")]
    public double Price { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El precio de compra no puede ser negativo.")]
    public double PurchasePrice { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }
}
