namespace TiendaBarrio.Persistence;

using System.Globalization;
using TiendaBarrio.Core.Models;

public class CashRepository
{
    private string RutaMovimientos = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Data", "movimientos.txt");

    public void SaveMovement(CashMovement movement)
    {
        // Format: Id;Type;Amount;Description;Date
        string line = $"{movement.Id};{movement.Type};{movement.Amount.ToString(CultureInfo.InvariantCulture)};{movement.Description};{movement.Date:yyyy-MM-dd HH:mm:ss}";
        File.AppendAllLines(RutaMovimientos, new[] { line });
    }

    public List<CashMovement> LoadMovements()
    {
        var movements = new List<CashMovement>();

        if (!File.Exists(RutaMovimientos))
            return movements;

        foreach (string rawLine in File.ReadLines(RutaMovimientos))
        {
            if (string.IsNullOrWhiteSpace(rawLine))
                continue;

            string[] parts = rawLine.Split(';');
            if (parts.Length < 4)
                continue;

            if (!int.TryParse(parts[0], out int id))
                continue;

            if (!Enum.TryParse(parts[1], out MovementType type))
                continue;

            if (!double.TryParse(parts[2], NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out double amount))
                continue;

            string description = parts[3];

            movements.Add(new CashMovement(id, type, amount, description));
        }

        return movements;
    }
}