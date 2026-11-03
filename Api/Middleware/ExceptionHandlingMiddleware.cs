using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace Api.Middleware;

/// <summary>Formato único de error de toda la API. Incluye "message" porque el ApiClient de Blazor lo lee.</summary>
public record ErrorResponse(int Status, string Error, string Message, string TraceId);

public static class ErrorWriter
{
    public static Task WriteAsync(HttpContext context, int status, string message)
    {
        context.Response.StatusCode = status;
        var body = new ErrorResponse(
            status,
            ReasonPhrases.GetReasonPhrase(status),
            message,
            context.TraceIdentifier);
        return context.Response.WriteAsJsonAsync(body);
    }
}

/// <summary>Convierte cualquier excepción no controlada en 400, 403, 404, 409 o 500 con JSON claro.</summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex) when (!context.Response.HasStarted)
        {
            var (status, message) = ex switch
            {
                ArgumentException => (StatusCodes.Status400BadRequest, ex.Message),
                InvalidOperationException => (StatusCodes.Status400BadRequest, ex.Message),
                KeyNotFoundException => (StatusCodes.Status404NotFound, ex.Message),
                UnauthorizedAccessException => (StatusCodes.Status403Forbidden, ex.Message),
                DbUpdateException => (StatusCodes.Status409Conflict,
                    "La operación choca con datos existentes (registro en uso o duplicado)."),
                _ => (StatusCodes.Status500InternalServerError,
                    "Ocurrió un error interno. Intenta de nuevo más tarde.")
            };

            if (status >= 500 || ex is DbUpdateException)
                _logger.LogError(ex, "Error {Status} en {Method} {Path}", status, context.Request.Method, context.Request.Path);
            else
                _logger.LogWarning("Error {Status} en {Method} {Path}: {Message}", status, context.Request.Method, context.Request.Path, ex.Message);

            context.Response.Clear();
            await ErrorWriter.WriteAsync(context, status, message);
        }
    }
}
