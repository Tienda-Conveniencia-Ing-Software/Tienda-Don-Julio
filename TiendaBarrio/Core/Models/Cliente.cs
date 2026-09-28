namespace TiendaBarrio.Core.Models;

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Cedula { get; set; }
    public string Telefono { get; set; }
    public string Email { get; set; }

    public Cliente(int id, string nombre, string cedula, string telefono, string email)
    {
        // Validación: El nombre no puede estar vacío
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre del cliente no puede estar vacío.");
        }

        // Validación: La cédula no puede estar vacía
        if (string.IsNullOrWhiteSpace(cedula))
        {
            throw new ArgumentException("La cédula del cliente no puede estar vacía.");
        }

        Id = id;
        Nombre = nombre;
        Cedula = cedula;
        Telefono = telefono;
        Email = email;
    }
}
