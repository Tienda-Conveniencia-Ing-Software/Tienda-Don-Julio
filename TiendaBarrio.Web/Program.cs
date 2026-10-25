using TiendaBarrio.Web.Components;
using TiendaBarrio.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// Configuración de la API REST (HttpClient)
// ============================================================================
var apiBaseUrl = builder.Configuration["Api:BaseUrl"] ?? "http://localhost:5222/";
if (!apiBaseUrl.EndsWith('/')) apiBaseUrl += "/";

builder.Services.AddScoped<AuthSession>();
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
builder.Services.AddScoped<ApiClient>();
builder.Services.AddScoped<CartState>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    app.UseHttpsRedirection(); // Solo exige HTTPS en producción
}

app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();