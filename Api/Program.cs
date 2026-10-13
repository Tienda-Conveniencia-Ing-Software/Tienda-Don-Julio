using Microsoft.EntityFrameworkCore;
using TiendaBarrio.Core.Config;
using TiendaBarrio.Persistence.Data;
using TiendaBarrio.Persistence;
using TiendaBarrio.Persistence.Interfaces;
using TiendaBarrio.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext con la cadena de conexión de PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (connectionString != null && connectionString.Contains("Host="))
    {
        options.UseNpgsql(connectionString);
    }
    else
    {
        options.UseSqlite(connectionString ?? "Data Source=tienda.db");
    }

    // Ignorar la advertencia de cambios pendientes para desarrollo
    options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});
// Registrar repositorios y servicios del Core (definido en Core/Config)
builder.Services.AddTiendaCore();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar CORS para permitir peticiones desde Blazor
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Aplicar migraciones automáticamente al iniciar
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowBlazor");
app.UseAuthorization();
app.MapControllers();

app.Run();