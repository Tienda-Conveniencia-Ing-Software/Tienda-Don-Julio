namespace TiendaBarrio.Core.Models;

public class Balance
{
    public double TotalIncome { get; }
    public double TotalExpense { get; }
    public double CurrentBalance => TotalIncome - TotalExpense;

    public Balance(double totalIncome, double totalExpense)
    {
        TotalIncome = totalIncome;
        TotalExpense = totalExpense;
    }
}