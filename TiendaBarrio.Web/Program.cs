using TiendaBarrio.Web.Components;
using Microsoft.EntityFrameworkCore;
using TiendaBarrio.Data;
using TiendaBarrio.Persistence;
using TiendaBarrio.Persistence.Interfaces;
using TiendaBarrio.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// CONFIGURACIÓN DE SERVICIOS - PERSONA 3
// ============================================================================

// 0. DbContext (los repositorios ya no lo crean con new)
builder.Services.AddDbContext<TiendaDbContext>();

// 1. Repositorios
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICashRepository, CashRepository>(); // <-- Agregado para resolver FinanceService

// 2. Servicios de Negocio (Core)
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<FinanceService>();                 // <-- Agregado para resolver InventoryService
builder.Services.AddScoped<InventoryService>();

// 3. Cliente HTTP listo para cuando Persona 1 levante la API en Docker
// TODO: Persona 1 (API) - Cuando los Controllers estén listos, cambiar las inyecciones
// directas por las llamadas HTTP correspondientes usando este HttpClient.
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5222/")
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