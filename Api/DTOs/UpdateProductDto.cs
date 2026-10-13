namespace Api.DTOs;

using System.ComponentModel.DataAnnotations;

/// <summary>Todos los campos son opcionales: los que no se envian se mantienen.</summary>
public class UpdateProductDto
{
    [StringLength(100, MinimumLength = 1)]
    public string? Name { get; set; }

    [Range(0, double.MaxValue)]
    public double? Price { get; set; }

    [Range(0, double.MaxValue)]
    public double? PurchasePrice { get; set; }
}
