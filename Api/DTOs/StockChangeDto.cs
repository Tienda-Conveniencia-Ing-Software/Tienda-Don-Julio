namespace Api.DTOs;

/// <summary>Cantidad positiva = aumenta stock, negativa = disminuye. Cero no es valido.</summary>
public class StockChangeDto
{
    public int Quantity { get; set; }
}
