using TiendaBarrio.Web.Components;
using TiendaBarrio.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// La web NO toca la base de datos ni los Services del Core:
// todo pasa por la API REST (HttpClient). URL configurable en Api:BaseUrl
// (en appsettings.json, o con la variable de entorno Api__BaseUrl).
// ============================================================================

var apiBaseUrl = builder.Configuration["Api:BaseUrl"] ?? "http://localhost:5222/";
if (!apiBaseUrl.EndsWith('/')) apiBaseUrl += "/";

// En Blazor Server "Scoped" = una instancia por usuario conectado (circuito).
// Así cada usuario tiene su propia sesión y su propio token.
builder.Services.AddScoped<AuthSession>();
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
builder.Services.AddScoped<ApiClient>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
