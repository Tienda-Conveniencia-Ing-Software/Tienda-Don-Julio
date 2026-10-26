# Tienda Don Julio

Sistema de gestión para una tienda de barrio, desarrollado en **.NET 10** con arquitectura por capas:
una **API REST** con autenticación JWT, una **web en Blazor** que la consume, persistencia con
**Entity Framework Core** sobre **PostgreSQL 16**, y una **app de consola** original que comparte la misma lógica de negocio.

Proyecto de Ingeniería de Software (Sprint 4).

## Arquitectura

```
  Blazor (TiendaBarrio.Web)  ──HTTP/JSON──▶  API REST (Api)  ──▶  Services (Core)  ──▶  Repositorios  ──▶  PostgreSQL
        http://localhost:5142                http://localhost:5222        (TiendaBarrio/)                      localhost:5432
                                                                                  ▲
                                                   App de consola ────────────────┘
```

| Proyecto | Carpeta | Qué hace |
|---|---|---|
| Core, Persistence y consola | `TiendaBarrio/` | Modelos, servicios de negocio, repositorios, `AppDbContext` y la app de consola |
| API | `Api/` | Controllers, DTOs, JWT y Swagger |
| Web | `TiendaBarrio.Web/` | Interfaz Blazor; solo habla con la API por HTTP (no toca la base de datos) |
| Pruebas | `TiendaBarrio.Tests/` | Pruebas unitarias (xUnit) del dominio |
| Base de datos | `database/` | Scripts SQL de inicialización y modelo relacional |
| Docker | `Docker/` | `docker-compose.yml` (PostgreSQL + API) |
| Documentación | `docs/` | Contratos de la API y colección de Postman |

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Git

## Cómo ejecutar

### 1. Levantar la base de datos y la API (Docker)

```bash
cd Docker
docker compose up --build
```

La primera vez, PostgreSQL ejecuta automáticamente los scripts de `database/init/`
(crean las tablas y cargan los datos de prueba). La API queda en:

- Swagger: http://localhost:5222/swagger
- PostgreSQL: `localhost:5432` (base `tiendabarrio`, usuario `postgres`, clave `postgres`)

### 2. Levantar la web (en otra terminal, desde la raíz del repo)

```bash
dotnet run --project TiendaBarrio.Web
```

Abrir http://localhost:5142 (si no se abre solo).

> La web todavía no está incluida en el `docker-compose.yml`; se ejecuta con `dotnet run`.

### 3. (Opcional) App de consola

```bash
dotnet run --project TiendaBarrio/Tienda-Don-Julio.csproj
```

Usa la misma base de PostgreSQL del contenedor (`localhost:5432`).

### Ejecutar la API sin Docker para la API

Con la base de datos del compose arriba (`docker compose up db`), puedes correr la API desde el IDE o con:

```bash
dotnet run --project Api
```

En desarrollo usa `Api/appsettings.Development.json` (apunta a `localhost:5432`, base `tiendabarrio`).
**Todo el equipo usa la base del contenedor**; no se necesita un PostgreSQL local propio.

## Usuarios de prueba

| Correo | Contraseña | Rol |
|---|---|---|
| `admin@donjulio.com` | `admin123` | Trabajador |
| `carlos@test.com` | `test123` | Trabajador |
| `juan@test.com` | `test123` | Cliente |
| `maria@test.com` | `test123` | Cliente |
| `ana@test.com` | `test123` | Cliente |
| `pedro@test.com` | `test123` | Cliente |

Son solo para desarrollo. Los clientes nuevos se registran en `/registro`.

## Base de datos

El esquema y los datos iniciales **no se definen en C#**: viven en scripts SQL.

| Archivo | Contenido |
|---|---|
| `database/init/01-schema.sql` | Tablas, llaves foráneas, restricciones e índices |
| `database/init/02-seed.sql` | Cuentas (hash BCrypt), productos, ventas, pedidos y caja de ejemplo |
| `database/modelo-relacional.md` | Descripción y diagrama del modelo |

EF Core solo **mapea** las entidades a esas tablas (`TiendaBarrio/Persistence/Data/AppDbContext.cs`).
Si cambias una tabla o columna, cambia el script **y** el mapeo, y actualiza `modelo-relacional.md`.

### Reiniciar la base de datos desde cero

PostgreSQL solo ejecuta los scripts cuando el volumen está vacío, por eso hay que borrarlo:

```bash
cd Docker
docker compose down -v
docker compose up --build
```

Para revisar la base directamente:

```bash
docker exec -it tiendabarrio_db psql -U postgres -d tiendabarrio -c "\dt"
```

## Reglas del negocio

- El cliente crea un **pedido** (`Pendiente`); no se toca inventario ni caja.
- Cuando el trabajador lo **confirma**, en una sola transacción se valida y descuenta el stock,
  se crea la **venta** con su detalle, se registra el **ingreso** en caja y el pedido pasa a `Completado`.
- Estados del pedido: `Pendiente`, `EnProceso`, `Completado`, `Cancelado`.
- Roles: `Cliente` (compra y consulta sus pedidos) y `Trabajador` (inventario, pedidos, finanzas y reportes).

## Endpoints de la API

Documentación interactiva en `/swagger`. Se autentica con `Authorization: Bearer <token>` (el token sale de `POST /api/auth/login`).

| Método | Ruta | Acceso |
|---|---|---|
| POST | `/api/auth/login` | Público |
| POST | `/api/auth/register` | Público |
| GET | `/api/inventory`, `/api/inventory/{id}` | Público |
| GET | `/api/inventory/low-stock` | Trabajador |
| POST / PUT / DELETE | `/api/inventory`, `/api/inventory/{id}` | Trabajador |
| POST | `/api/inventory/{id}/stock` | Trabajador (cantidad positiva aumenta, negativa disminuye) |
| POST | `/api/orders` | Autenticado (el cliente sale del token) |
| GET | `/api/orders/mine` | Autenticado |
| GET | `/api/orders`, `/api/orders/pending-count` | Trabajador |
| POST | `/api/orders/{id}/confirm` | Trabajador |
| PUT | `/api/orders/{id}/status` | Trabajador |
| GET | `/api/finance/balance`, `/api/finance/movements` | Trabajador |
| POST | `/api/finance/movements` | Trabajador |
| GET | `/api/reports/top-products` | Trabajador |

Contratos detallados: `docs/api-contratos.md`. Colección de Postman: `docs/postman/`.

## Páginas de la web

| Ruta | Para quién |
|---|---|
| `/` | Todos |
| `/login`, `/registro` | Todos |
| `/catalogo` | Cliente (catálogo, carrito y checkout) |
| `/mis-pedidos` | Cliente |
| `/inventario` | Lectura para todos; edición para Trabajador |
| `/orders` | Trabajador (panel de pedidos con aviso de pendientes) |
| `/finanzas`, `/top-products` | Trabajador |

## Pruebas

```bash
dotnet test
```

Las pruebas unitarias cubren las reglas del dominio (productos, ventas, movimientos de caja,
cuentas, pedidos y carrito) y comprueban que los estados, tipos y roles de C# coinciden con los
`CHECK` de `database/init/01-schema.sql`.

## Variables de entorno

| Variable | Dónde | Para qué |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | API | Cadena de conexión a PostgreSQL |
| `Jwt__Key` | API | Clave secreta del JWT (mínimo 32 caracteres) |
| `Api__BaseUrl` | Web | URL de la API (por defecto `http://localhost:5222/`) |
| `POSTGRES_PASSWORD`, `JWT_KEY` | `Docker/.env` | Valores para el compose (copia `Docker/.env.example`) |

En desarrollo funcionan sin configurar nada. **Para cualquier entorno real, cambia las contraseñas y la clave JWT.**

## Problemas frecuentes

| Síntoma | Solución |
|---|---|
| La API da error de tabla inexistente | La base se creó sin los scripts: `docker compose down -v` y vuelve a levantar |
| `port is already allocated` en 5432 | Hay otro PostgreSQL local ocupando el puerto; deténlo o cambia el puerto en el compose |
| La web dice "No se pudo conectar con la API" | Verifica que `docker ps` muestre `tiendabarrio_api` y que http://localhost:5222/swagger abra |
| Al recargar la web se pierde la sesión | Es una limitación conocida: hay que iniciar sesión otra vez |
| La terminal queda en `(END)` o `:` | Presiona `q` |
