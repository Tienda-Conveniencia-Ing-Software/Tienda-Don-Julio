using System.Text;
using System.Text.Json.Serialization;
using Api.Middleware;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TiendaBarrio.Core.Config;
using TiendaBarrio.Persistence.Data;
using TiendaBarrio.Persistence;
using TiendaBarrio.Persistence.Interfaces;
using TiendaBarrio.Core.Services;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection (variable de entorno ConnectionStrings__DefaultConnection).");
}

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// Registrar repositorios y servicios del Core (definido en Core/Config)
builder.Services.AddTiendaCore();

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory = ctx =>
    {
        // Validaciones de DTO (400) con el mismo formato de error que el resto de la API
        var msg = string.Join(" ", ctx.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        return new BadRequestObjectResult(new ErrorResponse(400, "Bad Request", msg, ctx.HttpContext.TraceIdentifier));
    });

// Autenticacion JWT
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Falta la configuracion Jwt:Key (variable de entorno Jwt__Key).");

builder.Services.AddSingleton<JwtTokenService>();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tienda Don Julio API", Version = "v1" });

    // Boton "Authorize" en Swagger para pegar el token JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pegue el token devuelto por POST /api/auth/login (sin la palabra Bearer)."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

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

// Errores no controlados -> 400/403/404/409/500 con JSON uniforme
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Respuestas vacías (401 sin token, 403, 404) -> también JSON uniforme
app.UseStatusCodePages(ctx => ErrorWriter.WriteAsync(
    ctx.HttpContext,
    ctx.HttpContext.Response.StatusCode,
    ctx.HttpContext.Response.StatusCode switch
    {
        401 => "No autorizado: falta el token o expiró.",
        403 => "No tienes permisos para esta acción.",
        404 => "Recurso no encontrado.",
        _ => "Solicitud no válida."
    }));

// Aplicar migraciones automáticamente al iniciar
//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    dbContext.Database.Migrate();
//}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowBlazor");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
