namespace Api.DTOs;

using System.ComponentModel.DataAnnotations;
using TiendaBarrio.Core.Models;

public class MovementDto
{
    /// <summary>Ingreso o Gasto. CompraInventario se registra solo desde el inventario.</summary>
    public MovementType Type { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0.")]
    public double Amount { get; set; }

    [Required(ErrorMessage = "La descripcion es obligatoria.")]
    [StringLength(200, MinimumLength = 1)]
    public string Description { get; set; } = string.Empty;
}
