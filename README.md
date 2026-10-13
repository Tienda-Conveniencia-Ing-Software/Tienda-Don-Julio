# Tienda-Don-Julio
🛒 Tienda Don Julio - Sistema de Gestión ComercialSistema de gestión para tiendas de barrio desarrollado con .NET 10 bajo una arquitectura multicapa desacoplada. La solución integra una API RESTful, un cliente interactivo en Blazor WebApp, persistencia con Entity Framework Core, soporte para SQLite (desarrollo local) y PostgreSQL (producción en Docker).🏗️ Arquitectura del SistemaLa solución está estructurada en capas independientes para garantizar mantenibilidad, escalabilidad y separación de responsabilidades:┌─────────────────────────────────────────────────────────┐
│              TiendaBarrio.Web (Blazor UI)               │
│                  http://localhost:5142                  │
└───────────────────────────┬─────────────────────────────┘
                            │  HTTP / JSON (HttpClient)
                            ▼
┌─────────────────────────────────────────────────────────┐
│                    Api (ASP.NET Core)                   │
│                  http://localhost:5222                  │
└───────────────────────────┬─────────────────────────────┘
                            │  EF Core
                            ▼
┌─────────────────────────────────────────────────────────┐
│          TiendaBarrio.Core & Persistence                │
│    (Modelos, Servicios, Repositorios, DbContext)        │
└───────────────────────────┬─────────────────────────────┘
                            │  SQL Queries
                            ▼
┌─────────────────────────────────────────────────────────┐
│          PostgreSQL / SQLite Database                   │
│                    Port: 5432                           │
└─────────────────────────────────────────────────────────┘
Proyectos de la SoluciónTiendaBarrio.Core: Definición de modelos de dominio (Cliente, Producto, MovimientoCaja) e interfaces de servicios.TiendaBarrio.Persistence: Implementación del contexto de Entity Framework Core (AppDbContext) y repositorios de datos.Api: Web API RESTful que expone los endpoints de autenticación, inventario y finanzas.TiendaBarrio.Web: Aplicación Blazor Web Interactive (Server/Client) que consume la API REST.🛠️ Tecnologías UtilizadasFramework: .NET 10 (C# 13)API Framework: ASP.NET Core Web APIFrontend: Blazor Web App (.NET 10) + Bootstrap 5ORM: Entity Framework Core 9.0.4Bases de Datos: PostgreSQL 16 (Docker) / SQLite (Local)Seguridad: BCrypt.Net para hashing de contraseñasContenedores: Docker & Docker Compose🚀 Requisitos PreviosAsegúrate de contar con los siguientes componentes instalados en tu sistema:.NET 10 SDKDocker DesktopGit⚡ Guía de Instalación y EjecuciónOpción 1: Ejecución Completa con Docker Compose (Recomendada)Clonar el repositorio:git clone https://github.com/tu-usuario/Tienda-Don-Julio.git
cd Tienda-Don-Julio
Construir y levantar los contenedores de PostgreSQL y Web API:docker compose -f Docker/docker-compose.yml up --build -d
Verificar que los contenedores estén corriendo (tiendabarrio_db y tiendabarrio_api):docker ps
Ejecutar el cliente Blazor Web:dotnet run --project TiendaBarrio.Web/TiendaBarrio.Web.csproj
Abrir el navegador en: http://localhost:5142Opción 2: Ejecución Local en Modo Desarrollo (SQLite)Iniciar la API REST en la primera terminal:dotnet run --project Api/Api.csproj
La API iniciará en http://localhost:5222.Iniciar el cliente Blazor Web en una segunda terminal:dotnet run --project TiendaBarrio.Web/TiendaBarrio.Web.csproj
El cliente iniciará en http://localhost:5142.🔑 Credenciales de Acceso (Data Seeding)El sistema inicializa automáticamente la base de datos con una cuenta de usuario administrador:Correo: admin@donjulio.comContraseña: admin123Rol: Trabajador / Administrador📡 Documentación de la API REST1. Autenticación (/api/auth)MétodoEndpointDescripciónBody (JSON)POST/api/auth/loginValida credenciales y retorna la entidad Cliente.{ "email": "admin@donjulio.com", "password": "admin123" }2. Inventario (/api/inventory)MétodoEndpointDescripciónBody (JSON)GET/api/inventoryObtiene la lista completa de productos.N/APOST/api/inventoryRegistra un nuevo producto en el catálogo.{ "nombre": "Pan Tajado", "precioVenta": 4000, "precioCosto": 3000, "stock": 20 }DELETE/api/inventory/{id}Elimina un producto por su ID.N/A3. Finanzas (/api/finance)MétodoEndpointDescripciónBody (JSON)GET/api/finance/balanceRetorna el balance total del flujo de caja.N/AGET/api/finance/movementsRetorna el historial de ingresos y egresos.N/A🐳 Estructura de Contenedores DockerEl archivo Docker/docker-compose.yml orquesta dos servicios principales en la misma red de contenedores:version: '3.8'

services:
  db:
    image: postgres:16-alpine
    container_name: tiendabarrio_db
    ports:
      - "5432:5432"
    environment:
      POSTGRES_DB: tiendabarrio
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres

  api:
    build:
      context: ..
      dockerfile: Api/Dockerfile
    container_name: tiendabarrio_api
    ports:
      - "5222:8080"
    environment:
      - ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=tiendabarrio;Username=postgres;Password=postgres
    depends_on:
      db:
        condition: service_healthy
✒️ LicenciaEste proyecto está bajo la Licencia MIT. Consulta el archivo LICENSE para más detalles.