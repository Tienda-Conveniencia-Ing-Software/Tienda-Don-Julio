namespace TiendaBarrio.Core.DTOs;

// ==========================================
// DTOs para Creación y Gestión de Pedidos
// ==========================================

public class OrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class CreateOrderDto
{
    public int ClienteId { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemResponseDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double Subtotal => Quantity * UnitPrice;
}

public class OrderResponseDto
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public double Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = new();
}

// ==========================================
// DTOs para Autenticación y Registro
// ==========================================

public class RegisterClientDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

// ==========================================
// DTOs para Reportes (HU-14 Productos Más Vendidos)
// ==========================================

public class TopProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int TotalQuantitySold { get; set; }
    public double TotalRevenue { get; set; }
}

// ==========================================
// DTOs para Actualizar Estado de Pedido
// ==========================================

public class UpdateOrderStatusRequest
{
    public string NewStatus { get; set; } = string.Empty;
}
