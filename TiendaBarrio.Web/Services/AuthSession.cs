using System.Text.Json;
using Microsoft.JSInterop;
using TiendaBarrio.Web.Models;

namespace TiendaBarrio.Web.Services;

public class AuthSession
{
    private readonly IJSRuntime _js;
    private const string STORAGE_KEY = "auth_session_data";

    public string? Token { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }

    public bool IsLoggedIn => !string.IsNullOrEmpty(Token) && ExpiresAt > DateTime.UtcNow;
    public bool IsWorker => IsLoggedIn && Role == "Trabajador";

    public event Action? Changed;

    public AuthSession(IJSRuntime js)
    {
        _js = js;
    }

    /// <summary>
    /// Restaura la sesión desde localStorage al recargar con F5.
    /// Llama a este método en OnAfterRenderAsync(firstRender: true) de MainLayout.razor
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            var json = await _js.InvokeAsync<string?>("localStorage.getItem", STORAGE_KEY);
            if (!string.IsNullOrEmpty(json))
            {
                var loginData = JsonSerializer.Deserialize<LoginResponse>(json);
                if (loginData != null && loginData.ExpiresAt.ToUniversalTime() > DateTime.UtcNow)
                {
                    Token = loginData.Token;
                    Nombre = loginData.Nombre;
                    Email = loginData.Email;
                    Role = loginData.Role;
                    ExpiresAt = loginData.ExpiresAt.ToUniversalTime();
                    Changed?.Invoke();
                }
                else
                {
                    await ClearAsync();
                }
            }
        }
        catch
        {
            // Evita errores durante prerenderizado
        }
    }

    public async Task StartAsync(LoginResponse login)
    {
        Token = login.Token;
        Nombre = login.Nombre;
        Email = login.Email;
        Role = login.Role;
        ExpiresAt = login.ExpiresAt.ToUniversalTime();

        try
        {
            var json = JsonSerializer.Serialize(login);
            await _js.InvokeVoidAsync("localStorage.setItem", STORAGE_KEY, json);
        }
        catch { }

        Changed?.Invoke();
    }

    public async Task ClearAsync()
    {
        Token = null;
        Nombre = string.Empty;
        Email = string.Empty;
        Role = string.Empty;
        ExpiresAt = default;

        try
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", STORAGE_KEY);
        }
        catch { }

        Changed?.Invoke();
    }
    // Métodos de compatibilidad para llamadas sincrónicas desde ApiClient
    public void Start(LoginResponse login) => _ = StartAsync(login);
    public void Clear() => _ = ClearAsync();
}