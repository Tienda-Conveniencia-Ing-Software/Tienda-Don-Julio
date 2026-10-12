namespace TiendaBarrio.Core.Models;

public class Usuario
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; }
    public string Email { get; set; }
    public string ContraseñaHash { get; set; }
    public string Rol { get; set; }   // "Admin" o "Vendedor"
    public DateTime FechaCreacion { get; set; }

    // Constructor vacío (para EF Core)
    public Usuario()
    {
        FechaCreacion = DateTime.Now;
    }

    // Constructor con parámetros (para crear usuarios desde código)
    public Usuario(int id, string nombreUsuario, string email, string contraseñaHash, string rol)
    {
        // Validación: El nombre de usuario no puede estar vacío
        if (string.IsNullOrWhiteSpace(nombreUsuario))
        {
            throw new ArgumentException("El nombre de usuario no puede estar vacío.");
        }

        // Validación: El email no puede estar vacío
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("El email no puede estar vacío.");
        }

        // Validación: El rol debe ser "Admin" o "Vendedor"
        if (rol != "Admin" && rol != "Vendedor")
        {
            throw new ArgumentException("El rol debe ser 'Admin' o 'Vendedor'.");
        }

        Id = id;
        NombreUsuario = nombreUsuario;
        Email = email;
        ContraseñaHash = contraseñaHash;
        Rol = rol;
        FechaCreacion = DateTime.Now;
    }
}
