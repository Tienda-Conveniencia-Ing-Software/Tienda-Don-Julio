namespace TiendaBarrio.Core.Models;

public class Movimiento
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Tipo { get; set; }        // "Ingreso" o "Gasto"
    public string Descripcion { get; set; }
    public double Monto { get; set; }

    public Movimiento(int id, string tipo, string descripcion, double monto)
    {
        // Validación: El tipo debe ser "Ingreso" o "Gasto"
        if (tipo != "Ingreso" && tipo != "Gasto")
        {
            throw new ArgumentException("El tipo de movimiento debe ser 'Ingreso' o 'Gasto'.");
        }

        // Validación: El monto debe ser positivo
        if (monto <= 0)
        {
            throw new ArgumentException("El monto del movimiento debe ser mayor a 0.");
        }

        Id = id;
        Fecha = DateTime.Now;
        Tipo = tipo;
        Descripcion = descripcion;
        Monto = monto;
    }
}
