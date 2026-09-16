namespace TiendaBarrio.Persistence;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Core.Config;

public class MovimientoRepository
{
    // Ruta centralizada en DataConfig
    private string RutaMovimientos => DataConfig.MovimientosFile;

    public List<Movimiento> LoadMovimientos()
    {
        var movimientos = new List<Movimiento>();

        // Asegura que la carpeta Data exista
        DataConfig.EnsureDataFolderExists();

        if (!File.Exists(RutaMovimientos))
            return movimientos;

        foreach (string line in File.ReadLines(RutaMovimientos))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Formato: Id;Fecha;Tipo;Descripcion;Monto
            string[] parts = line.Split(';');
            if (parts.Length < 5)
                continue;

            if (!int.TryParse(parts[0], out int id))
                continue;

            if (!DateTime.TryParse(parts[1], out DateTime fecha))
                continue;

            string tipo = parts[2];
            string descripcion = parts[3];

            if (!double.TryParse(parts[4], NumberStyles.Any, CultureInfo.InvariantCulture, out double monto))
                continue;

            try
            {
                var m = new Movimiento(id, tipo, descripcion, monto);
                m.Fecha = fecha;
                movimientos.Add(m);
            }
            catch (ArgumentException)
            {
                // Si el movimiento tiene datos inválidos, se ignora
                continue;
            }
        }

        return movimientos;
    }

    public void SaveMovimientos(List<Movimiento> movimientos)
    {
        // Asegura que la carpeta Data exista
        DataConfig.EnsureDataFolderExists();

        string[] lines = movimientos
            .Select(m => $"{m.Id};{m.Fecha:yyyy-MM-dd HH:mm:ss};{m.Tipo};{m.Descripcion};{m.Monto.ToString(CultureInfo.InvariantCulture)}")
            .ToArray();

        File.WriteAllLines(RutaMovimientos, lines);
    }

    public void AddMovimiento(Movimiento movimiento)
    {
        var movimientos = LoadMovimientos();
        movimientos.Add(movimiento);
        SaveMovimientos(movimientos);
    }

    public void UpdateMovimiento(Movimiento movimiento)
    {
        var movimientos = LoadMovimientos();
        var index = movimientos.FindIndex(m => m.Id == movimiento.Id);

        if (index >= 0)
        {
            movimientos[index] = movimiento;
            SaveMovimientos(movimientos);
        }
        else
        {
            Console.WriteLine($"No se encontró el movimiento con ID {movimiento.Id}.");
        }
    }

    public void DeleteMovimiento(int id)
    {
        var movimientos = LoadMovimientos();
        var movimiento = movimientos.FirstOrDefault(m => m.Id == id);

        if (movimiento != null)
        {
            movimientos.Remove(movimiento);
            SaveMovimientos(movimientos);
        }
        else
        {
            Console.WriteLine($"No se encontró el movimiento con ID {id}.");
        }
    }

    public Movimiento GetMovimientoById(int id)
    {
        return LoadMovimientos().FirstOrDefault(m => m.Id == id);
    }

    public int GetNextMovimientoId()
    {
        var movimientos = LoadMovimientos();
        return movimientos.Count > 0 ? movimientos.Max(m => m.Id) + 1 : 1;
    }

    // Métodos adicionales para consultas financieras

    public double GetTotalIngresos()
    {
        return LoadMovimientos()
            .Where(m => m.Tipo == "Ingreso")
            .Sum(m => m.Monto);
    }

    public double GetTotalGastos()
    {
        return LoadMovimientos()
            .Where(m => m.Tipo == "Gasto")
            .Sum(m => m.Monto);
    }

    public double GetBalance()
    {
        return GetTotalIngresos() - GetTotalGastos();
    }
}
