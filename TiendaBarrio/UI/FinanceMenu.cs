namespace TiendaBarrio.UI;

using TiendaBarrio.Core.Models;
using TiendaBarrio.Core.Services;
using TiendaBarrio.Utils;

public class FinanceMenu
{
    private readonly FinanceService _financeService;

    public FinanceMenu(FinanceService financeService)
    {
        _financeService = financeService;
    }

    public async Task StartAsync()
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
                    await ShowBalanceAsync();
                    new Pause().pause();
                    break;

                case 2:
                    await RegisterIncomeFlowAsync();
                    new Pause().pause();
                    break;

                case 3:
                    await RegisterExpenseFlowAsync();
                    new Pause().pause();
                    break;

                case 4:
                    await ShowHistoryAsync();
                    new Pause().pause();
                    break;

                default:
                    Console.WriteLine("Option not available");
                    new Pause().pause();
                    break;
            }
        }
    }

    private async Task ShowBalanceAsync()
    {
        Balance balance = await _financeService.GetBalanceAsync();
        Console.WriteLine($"Total income: {balance.TotalIncome}");
        Console.WriteLine($"Total expense: {balance.TotalExpense}");
        Console.WriteLine($"Current balance: {balance.CurrentBalance}");
    }

    private async Task RegisterIncomeFlowAsync()
    {
        Console.WriteLine("Enter amount:");
        if (!double.TryParse(Console.ReadLine(), out double amount) || amount <= 0)
        {
            Console.WriteLine("Invalid amount.");
            return;
        }

        Console.WriteLine("Enter description:");
        string description = Console.ReadLine() ?? "Manual income";

        await _financeService.RegisterIncomeAsync(amount, description);
        Console.WriteLine("Income registered.");
    }

    private async Task RegisterExpenseFlowAsync()
    {
        Console.WriteLine("Enter amount:");
        if (!double.TryParse(Console.ReadLine(), out double amount) || amount <= 0)
        {
            Console.WriteLine("Invalid amount.");
            return;
        }

        Console.WriteLine("Enter description:");
        string description = Console.ReadLine() ?? "Manual expense";

        await _financeService.RegisterExpenseAsync(amount, description);
        Console.WriteLine("Expense registered.");
    }

    private async Task ShowHistoryAsync()
    {
        var movements = await _financeService.GetHistoryAsync();
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