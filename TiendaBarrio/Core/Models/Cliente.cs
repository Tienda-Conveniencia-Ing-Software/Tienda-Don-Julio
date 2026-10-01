namespace TiendaBarrio.Core.Models;

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Cedula { get; set; }
    public string Telefono { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; } // "Trabajador" o "Cliente"

    // Constructor sin parámetros necesario para Entity Framework Core
    public Cliente()
    {
        Role = "Cliente"; // Rol por defecto
    }

    public Cliente(int id, string nombre, string cedula, string telefono, string email, string passwordHash, string role = "Cliente")
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre del cliente no puede estar vacío.");
        }

        if (string.IsNullOrWhiteSpace(cedula))
        {
            throw new ArgumentException("La cédula del cliente no puede estar vacía.");
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("El hash de la contraseña no puede estar vacío.");
        }

        Id = id;
        Nombre = nombre;
        Cedula = cedula;
        Telefono = telefono;
        Email = email;
        PasswordHash = passwordHash;
        Role = string.IsNullOrWhiteSpace(role) ? "Cliente" : role;
    }

    // Método helper para verificar fácilmente si es trabajador
    public bool EsTrabajador()
    {
        return string.Equals(Role, "Trabajador", StringComparison.OrdinalIgnoreCase);
    }
}