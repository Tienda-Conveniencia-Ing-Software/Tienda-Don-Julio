using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using TiendaBarrio.Web.Models;

namespace TiendaBarrio.Web.Services;

/// <summary>
/// Único lugar donde la web habla con la API. Agrega el token JWT a cada petición
/// y convierte los errores HTTP en mensajes legibles para mostrar en pantalla.
/// </summary>
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
            _session.Start(result.Data);
        }
        return result;
    }

    public void Logout() => _session.Clear();

    // ---------------- Inventario ----------------

    public Task<ApiResult<List<ProductItem>>> GetProductsAsync()
        => SendAsync<List<ProductItem>>(HttpMethod.Get, "api/inventory");

    public Task<ApiResult<ProductItem>> CreateProductAsync(string name, double price, double purchasePrice, int stock)
        => SendAsync<ProductItem>(HttpMethod.Post, "api/inventory", new { name, price, purchasePrice, stock });

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

    // ✅ Corregido para enviar el estado en el cuerpo (JSON) tal como lo espera el Controller
    public Task<ApiResult<bool>> UpdateOrderStatusAsync(int orderId, string newStatus)
        => SendAsync<bool>(HttpMethod.Put, $"api/orders/{orderId}/status", new { NewStatus = newStatus });

    // ---------------- Internos ----------------

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

    private async Task<ApiResult<bool>> SendWithoutBodyAsync(HttpMethod method, string url)
    {
        try
        {
            using var request = BuildRequest(method, url, null);
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

    private string ConnectionError()
        => $"No se pudo conectar con la API en {_http.BaseAddress}. ¿Está levantada (docker compose up)?";

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
    {
        // Mensaje por defecto según el código HTTP
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

            // { "message": "..." }  (MessageDto de la API)
            if (root.TryGetProperty("message", out var msg) && msg.ValueKind == JsonValueKind.String)
            {
                return msg.GetString() ?? message;
            }

            // { "errors": { "Campo": ["error1", ...] } }  (validaciones)
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

            // { "title": "..." }  (ProblemDetails)
            if (root.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String)
            {
                return title.GetString() ?? message;
            }
        }
        catch (JsonException)
        {
            // el cuerpo no era JSON: se usa el mensaje por defecto
        }

        return message;
    }
}
