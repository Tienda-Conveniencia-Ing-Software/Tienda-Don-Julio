namespace TiendaBarrio.UI;

using TiendaBarrio.Core.Models;
using TiendaBarrio.Core.Services;
using TiendaBarrio.Utils;

public class FinanceMenu
{
    private readonly FinanceService _financeService = new();

    public void Start()
    {
        bool exit = true;
        while (exit)
        {
            Console.Clear();
            Console.WriteLine("// FINANCE \\\\");
            Console.WriteLine("0. Back");
            Console.WriteLine("1. View balance");
            Console.WriteLine("2. Register manual income");
            Console.WriteLine("3. Register manual expense");
            Console.WriteLine("4. View movement history");
            Console.WriteLine("\nSelect an option: ");

            if (!int.TryParse(Console.ReadLine(), out int option))
            {
                Console.WriteLine("Invalid input.");
                new Pause().pause();
                continue;
            }

            switch (option)
            {
                case 0:
                    exit = false;
                    break;

                case 1:
                    ShowBalance();
                    new Pause().pause();
                    break;

                case 2:
                    RegisterIncomeFlow();
                    new Pause().pause();
                    break;

                case 3:
                    RegisterExpenseFlow();
                    new Pause().pause();
                    break;

                case 4:
                    ShowHistory();
                    new Pause().pause();
                    break;

                default:
                    Console.WriteLine("Option not available");
                    new Pause().pause();
                    break;
            }
        }
    }

    private void ShowBalance()
    {
        Balance balance = _financeService.GetBalance();
        Console.WriteLine($"Total income: {balance.TotalIncome}");
        Console.WriteLine($"Total expense: {balance.TotalExpense}");
        Console.WriteLine($"Current balance: {balance.CurrentBalance}");
    }

    private void RegisterIncomeFlow()
    {
        Console.WriteLine("Enter amount:");
        if (!double.TryParse(Console.ReadLine(), out double amount) || amount <= 0)
        {
            Console.WriteLine("Invalid amount.");
            return;
        }

        Console.WriteLine("Enter description:");
        string description = Console.ReadLine() ?? "Manual income";

        _financeService.RegisterIncome(amount, description);
        Console.WriteLine("Income registered.");
    }

    private void RegisterExpenseFlow()
    {
        Console.WriteLine("Enter amount:");
        if (!double.TryParse(Console.ReadLine(), out double amount) || amount <= 0)
        {
            Console.WriteLine("Invalid amount.");
            return;
        }

        Console.WriteLine("Enter description:");
        string description = Console.ReadLine() ?? "Manual expense";

        _financeService.RegisterExpense(amount, description);
        Console.WriteLine("Expense registered.");
    }

    private void ShowHistory()
    {
        var movements = _financeService.GetHistory();
        if (movements.Count == 0)
        {
            Console.WriteLine("No movements yet.");
            return;
        }

        foreach (var m in movements)
        {
            Console.WriteLine($"#{m.Id} - {m.Type} - {m.Amount} - {m.Description} - {m.Date}");
        }
    }
}