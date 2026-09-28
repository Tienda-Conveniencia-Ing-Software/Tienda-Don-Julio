namespace TiendaBarrio.Core.Models;

public class SaleDetail
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }    // Precio de venta unitario
    public double UnitCost { get; set; }     // Precio de compra unitario

    // Subtotal de venta = cantidad * precio de venta
    public double Subtotal => Quantity * UnitPrice;

    // Subtotal de costo = cantidad * precio de compra
    public double SubtotalCost => Quantity * UnitCost;

    // Constructor con validaciones
    public SaleDetail(int productId, string productName, int quantity, double unitPrice, double unitCost)
    {
        // Validación: La cantidad debe ser mayor a 0
        if (quantity <= 0)
        {
            throw new ArgumentException("La cantidad debe ser mayor a 0.");
        }

        // Validación: El precio de venta no puede ser negativo
        if (unitPrice < 0)
        {
            throw new ArgumentException("El precio de venta no puede ser negativo.");
        }

        // Validación: El precio de compra no puede ser negativo
        if (unitCost < 0)
        {
            throw new ArgumentException("El precio de compra no puede ser negativo.");
        }

        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        UnitCost = unitCost;
    }
}
