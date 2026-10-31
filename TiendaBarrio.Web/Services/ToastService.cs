namespace TiendaBarrio.Web.Services;

public class ToastService
{
    public event Action<string, string>? OnShow;

    public void ShowSuccess(string message) => OnShow?.Invoke(message, "bg-success text-white");
    public void ShowError(string message) => OnShow?.Invoke(message, "bg-danger text-white");
    public void ShowWarning(string message) => OnShow?.Invoke(message, "bg-warning text-dark");
    public void ShowInfo(string message) => OnShow?.Invoke(message, "bg-primary text-white");
}