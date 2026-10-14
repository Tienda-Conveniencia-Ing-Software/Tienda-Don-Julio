using TiendaBarrio.Web.Models;

namespace TiendaBarrio.Web.Services;

/// <summary>
/// Guarda quién inició sesión y su token JWT (en memoria, por usuario conectado).
/// Si se recarga la página con F5 se pierde y hay que iniciar sesión otra vez.
/// </summary>
public class AuthSession
{
    public string? Token { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }

    public bool IsLoggedIn => !string.IsNullOrEmpty(Token) && ExpiresAt > DateTime.UtcNow;
    public bool IsWorker => IsLoggedIn && Role == "Trabajador";

    /// <summary>Se dispara al iniciar o cerrar sesión, para que el menú se actualice.</summary>
    public event Action? Changed;

    public void Start(LoginResponse login)
    {
        Token = login.Token;
        Nombre = login.Nombre;
        Email = login.Email;
        Role = login.Role;
        ExpiresAt = login.ExpiresAt.ToUniversalTime();
        Changed?.Invoke();
    }

    public void Clear()
    {
        Token = null;
        Nombre = string.Empty;
        Email = string.Empty;
        Role = string.Empty;
        ExpiresAt = default;
        Changed?.Invoke();
    }
}
