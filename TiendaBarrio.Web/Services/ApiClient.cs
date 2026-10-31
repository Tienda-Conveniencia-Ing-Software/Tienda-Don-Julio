using TiendaBarrio.Web.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace TiendaBarrio.Web.Services;

public class ApiClient
{
    private readonly HttpClient _http;
    private readonly AuthSession _session;

    public ApiClient(HttpClient http, AuthSession session)
    {
        _http = http;
        _session = session;
    }

    // ---------------- Autenticación ----------------

    public async Task<ApiResult<LoginResponse>> LoginAsync(string email, string password)
    {
        var result = await SendAsync<LoginResponse>(HttpMethod.Post, "api/auth/login", new { email, password });
        if (result.Ok && result.Data != null)
        {
            await _session.StartAsync(result.Data);
        }
        return result;
    }

    public async void Logout() => await _session.ClearAsync();

    public Task<ApiResult<bool>> RegisterAsync(string nombre, string email, string password, string cedula = "", string telefono = "")
        => SendWithoutBodyWithDataAsync(HttpMethod.Post, "api/auth/register", new RegisterRequest { Nombre = nombre, Email = email, Password = password, Cedula = cedula, Telefono = telefono });

    // ---------------- Inventario ----------------

    public Task<ApiResult<List<ProductItem>>> GetProductsAsync()
        => SendAsync<List<ProductItem>>(HttpMethod.Get, "api/inventory");

    public Task<ApiResult<ProductItem>> CreateProductAsync(string name, double price, double purchasePrice, int stock)
        => SendAsync<ProductItem>(HttpMethod.Post, "api/inventory", new { name, price, purchasePrice, stock });

    public Task<ApiResult<bool>> AddStockAsync(int productId, int quantity)
        => SendWithoutBodyWithDataAsync(HttpMethod.Post, $"api/inventory/{productId}/stock", new { Quantity = quantity });

    public Task<ApiResult<bool>> DeleteStockAsync(int productId, int quantity)
        => SendAsync<bool>(HttpMethod.Post, $"api/inventory/{productId}/stock", new { quantity = -quantity });

    public Task<ApiResult<bool>> DeleteProductAsync(int id)
        => SendWithoutBodyAsync(HttpMethod.Delete, $"api/inventory/{id}");

    // ---------------- Finanzas ----------------

    public Task<ApiResult<BalanceInfo>> GetBalanceAsync()
        => SendAsync<BalanceInfo>(HttpMethod.Get, "api/finance/balance");

    public Task<ApiResult<List<MovementItem>>> GetMovementsAsync()
        => SendAsync<List<MovementItem>>(HttpMethod.Get, "api/finance/movements");

    // ---------------- Reportes (HU-14) ----------------

    public Task<ApiResult<List<TopProductItem>>> GetTopProductsAsync()
        => SendAsync<List<TopProductItem>>(HttpMethod.Get, "api/reports/top-products");

    // ---------------- Pedidos (HU-06) ----------------

    public Task<ApiResult<List<OrderResponse>>> GetAllOrdersAsync()
        => SendAsync<List<OrderResponse>>(HttpMethod.Get, "api/orders");

    public Task<ApiResult<int>> GetPendingOrdersCountAsync()
        => SendAsync<int>(HttpMethod.Get, "api/orders/pending-count");

    public Task<ApiResult<OrderResponse>> CreateOrderAsync(CreateOrderRequest order)
        => SendAsync<OrderResponse>(HttpMethod.Post, "api/orders", order);

    public Task<ApiResult<List<OrderResponse>>> GetMyOrdersAsync()
        => SendAsync<List<OrderResponse>>(HttpMethod.Get, "api/orders/mine");

    public Task<ApiResult<bool>> UpdateOrderStatusAsync(int orderId, string newStatus)
        => SendWithoutBodyAsync(HttpMethod.Put, $"api/orders/{orderId}/status", new { NewStatus = newStatus });

    public Task<ApiResult<object>> ConfirmOrderAsync(int orderId)
        => SendAsync<object>(HttpMethod.Post, $"api/orders/{orderId}/confirm");

    // ---------------- Helpers Internos ----------------

    private HttpRequestMessage BuildRequest(HttpMethod method, string url, object? body)
    {
        var request = new HttpRequestMessage(method, url);
        if (_session.IsLoggedIn)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _session.Token);
        }
        if (body != null)
        {
            request.Content = JsonContent.Create(body);
        }
        return request;
    }

    private async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string url, object? body = null)
    {
        try
        {
            using var request = BuildRequest(method, url, body);
            using var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return ApiResult<T>.Fail(await ReadErrorAsync(response));
            }

            var data = await response.Content.ReadFromJsonAsync<T>();
            return ApiResult<T>.Success(data);
        }
        catch (HttpRequestException)
        {
            return ApiResult<T>.Fail(ConnectionError());
        }
        catch (TaskCanceledException)
        {
            return ApiResult<T>.Fail("La API tardó demasiado en responder.");
        }
        catch (JsonException)
        {
            return ApiResult<T>.Fail("La API devolvió una respuesta que no se pudo interpretar.");
        }
    }

    private async Task<ApiResult<bool>> SendWithoutBodyAsync(HttpMethod method, string url, object? body = null)
    {
        try
        {
            using var request = BuildRequest(method, url, body);
            using var response = await _http.SendAsync(request);

            return response.IsSuccessStatusCode
                ? ApiResult<bool>.Success(true)
                : ApiResult<bool>.Fail(await ReadErrorAsync(response));
        }
        catch (HttpRequestException)
        {
            return ApiResult<bool>.Fail(ConnectionError());
        }
        catch (TaskCanceledException)
        {
            return ApiResult<bool>.Fail("La API tardó demasiado en responder.");
        }
    }

    private async Task<ApiResult<bool>> SendWithoutBodyWithDataAsync(HttpMethod method, string url, object body)
    {
        try
        {
            using var request = BuildRequest(method, url, body);
            using var response = await _http.SendAsync(request);

            return response.IsSuccessStatusCode
                ? ApiResult<bool>.Success(true)
                : ApiResult<bool>.Fail(await ReadErrorAsync(response));
        }
        catch (HttpRequestException) { return ApiResult<bool>.Fail(ConnectionError()); }
        catch (TaskCanceledException) { return ApiResult<bool>.Fail("La API tardó demasiado en responder."); }
    }

    private string ConnectionError()
        => $"No se pudo conectar con la API en {_http.BaseAddress}. ¿Está levantada (docker compose up)?";

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
    {
        var message = response.StatusCode switch
        {
            HttpStatusCode.Unauthorized => "No autorizado: inicia sesión (tu sesión puede haber expirado).",
            HttpStatusCode.Forbidden => "No tienes permisos para esta acción.",
            HttpStatusCode.NotFound => "No se encontró el recurso solicitado.",
            HttpStatusCode.BadRequest => "Datos inválidos.",
            _ => $"Error del servidor ({(int)response.StatusCode})."
        };

        try
        {
            var text = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(text)) return message;

            using var doc = JsonDocument.Parse(text);
            var root = doc.RootElement;
            if (root.ValueKind != JsonValueKind.Object) return message;

            if (root.TryGetProperty("message", out var msg) && msg.ValueKind == JsonValueKind.String)
            {
                return msg.GetString() ?? message;
            }

            if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
            {
                var list = errors.EnumerateObject()
                    .Where(p => p.Value.ValueKind == JsonValueKind.Array)
                    .SelectMany(p => p.Value.EnumerateArray())
                    .Where(e => e.ValueKind == JsonValueKind.String)
                    .Select(e => e.GetString())
                    .ToList();
                if (list.Count > 0) return string.Join(" ", list);
            }

            if (root.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String)
            {
                return title.GetString() ?? message;
            }
        }
        catch (JsonException) { }

        return message;
    }
}