using TiendaBarrio.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// CONFIGURACIÓN DE SERVICIOS - PERSONA 3
// ============================================================================

// TODO: Persona 1 (API) - Cuando los Controllers de la API estén listos,
// cambiar la inyección directa de AuthService por el HttpClient configurado
// apuntando a la URL base de la API de Docker (ej. http://localhost:5000/api/).

builder.Services.AddScoped<TiendaBarrio.Persistence.Interfaces.IClienteRepository, TiendaBarrio.Persistence.ClienteRepository>();
builder.Services.AddScoped<TiendaBarrio.Core.Services.AuthService>();

// Cliente HTTP listo para cuando Persona 1 levante los Endpoints de la API
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5000/") // TODO: Persona 1 - Ajustar puerto/host de Docker
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
