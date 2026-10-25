namespace TiendaBarrio.Web.Models;

// Copias locales de lo que devuelve la API (DTOs). Así la web no depende del Core.

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class ProductItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public double PurchasePrice { get; set; }
    public int Stock { get; set; }
}

public class TopProductItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int TotalQuantitySold { get; set; }
    public double TotalRevenue { get; set; }
}

public class BalanceInfo
{
    public double TotalIncome { get; set; }
    public double TotalExpense { get; set; }
    public double CurrentBalance { get; set; }
}

public class MovementItem
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;   // "Ingreso", "Gasto" o "CompraInventario"
    public double Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}

/// <summary>Resultado de una llamada a la API: o trae datos, o trae un mensaje de error legible.</summary>
public class ApiResult<T>
{
    public bool Ok { get; init; }
    public T? Data { get; init; }
    public string? Error { get; init; }

    public static ApiResult<T> Success(T? data) => new() { Ok = true, Data = data };
    public static ApiResult<T> Fail(string error) => new() { Ok = false, Error = error };
}

public class OrderItemResponse
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double Subtotal => Quantity * UnitPrice; 
}

public class OrderResponse
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public double Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<OrderItemResponse> Items { get; set; } = new();
}

public class CartLine
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double UnitPrice { get; set; }
    public int Quantity { get; set; }
    public int MaxStock { get; set; }
    public double Subtotal => UnitPrice * Quantity; 
}

public class CreateOrderRequest
{
    public int ClienteId { get; set; }
    public List<CreateOrderItem> Items { get; set; } = new();
}

public class CreateOrderItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class RegisterRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
}


