namespace Api.Extensions;

using System.Security.Claims;
using TiendaBarrio.Core.Models;

public static class UserMappingExtensions
{
    /// <summary>Reconstruye el usuario autenticado a partir de los claims del token.</summary>
    public static Cliente ToCliente(this ClaimsPrincipal user)
    {
        return new Cliente
        {
            Id = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0,
            Nombre = user.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
            Email = user.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            Role = user.FindFirstValue(ClaimTypes.Role) ?? "Cliente"
        };
    }
}
