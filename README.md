# Tienda Don Julio

Sistema de gestión de una tienda de barrio desarrollado con .NET 10, Blazor, API REST, Entity Framework Core y PostgreSQL 16. La base de datos se inicializa con scripts SQL nativos.

## Requisitos

- .NET 10 SDK para ejecutar la solución y las pruebas.
- Docker Desktop con Docker Compose.
- Git.

## Estructura principal

- `TiendaBarrio/`: modelos, servicios, persistencia y aplicación de consola heredada.
- `Api/`: API REST, autenticación JWT y controladores.
- `TiendaBarrio.Web/`: interfaz web Blazor.
- `TiendaBarrio.Tests/`: pruebas automatizadas con xUnit.
- `database/init/01-schema.sql`: esquema de PostgreSQL.
- `database/init/02-seed.sql`: datos iniciales de desarrollo.
- `database/tests/validate-schema.sql`: validación de humo del esquema y el seed.
- `Docker/docker-compose.yml`: servicios `db`, `api` y `web`.

## Arranque completo con Docker

Desde la raíz del repositorio:

```bash
cd Docker
```

Si es la primera ejecución, crea un archivo `.env` a partir de `.env.example` y configura los valores locales de desarrollo. Después ejecuta:

```bash
docker compose up --build -d
docker compose ps
```

Servicios publicados en el equipo local:

- Web Blazor: http://localhost:5142
- API: http://localhost:5222
- Swagger en entorno de desarrollo: http://localhost:5222/swagger
- PostgreSQL: `localhost:5432`

Compose ejecuta `database/init/01-schema.sql` y `database/init/02-seed.sql` automáticamente únicamente cuando PostgreSQL inicializa un volumen vacío. Si los scripts se modifican después de haber creado el volumen, no se vuelven a ejecutar solos.

Para ver los logs:

```bash
docker compose logs --tail=100 db api web
```

Para detener los servicios sin borrar los datos:

```bash
docker compose down
```

### Reiniciar la base de datos desde cero

> **Advertencia:** `down -v` elimina el volumen de PostgreSQL y todos los datos almacenados en él. Úsalo solamente en un entorno local de pruebas.

Desde la carpeta `Docker/`:

```bash
docker compose down -v
docker compose up --build -d
docker compose ps
```

## Pruebas automatizadas

Desde la raíz del repositorio:

```bash
dotnet restore TiendaBarrio.sln
dotnet test TiendaBarrio.sln
```

Las pruebas de `TiendaBarrio.Tests` comprueban reglas de entidades y el contrato entre nombres de tablas/columnas de EF Core y el esquema SQL. Estas pruebas no sustituyen las pruebas de integración contra PostgreSQL.

## Validar la base de datos real

Con Compose arriba, desde la carpeta `Docker/` ejecuta el script de validación:

```bash
docker exec -i tiendabarrio_db psql -v ON_ERROR_STOP=1 -U postgres -d tiendabarrio < ../database/tests/validate-schema.sql
```

Si en `Docker/.env` cambiaste `POSTGRES_USER` o `POSTGRES_DB`, sustituye `postgres` y `tiendabarrio` por los valores configurados. El script verifica que existan las tablas principales, algunas columnas importantes y datos iniciales. No modifica datos.

También puedes listar tablas manualmente:

```bash
docker exec -it tiendabarrio_db psql -U postgres -d tiendabarrio -c "\\dt"
```

## Flujo de negocio que debe verificarse

1. Un cliente se registra o inicia sesión.
2. El cliente consulta el catálogo y crea un pedido.
3. El pedido queda `Pendiente`; crear el pedido no debe descontar stock ni registrar un ingreso.
4. Un trabajador confirma el pedido con `POST /api/orders/{id}/confirm`.
5. La confirmación debe realizar, en una sola transacción, el descuento de stock, la creación de la venta y sus detalles, el registro del ingreso en caja y el cambio del pedido a `Completado`.
6. Si la confirmación falla, no deben persistir cambios parciales. Confirmar nuevamente el mismo pedido no debe crear ventas o ingresos duplicados.
7. El historial de pedidos y el reporte de productos más vendidos deben reflejar la operación.

## Prueba de integración manual (API)

Con los tres servicios activos:

1. Abre Swagger en `http://localhost:5222/swagger` o importa `docs/postman/TiendaDonJulio.postman_collection.json` en Postman.
2. Inicia sesión con una cuenta de desarrollo que exista en el seed y copia el JWT.
3. Autoriza las peticiones protegidas con `Authorization: Bearer <token>`.
4. Consulta inventario y anota el stock de un producto de prueba.
5. Inicia sesión como cliente y crea un pedido usando `POST /api/orders` con productos existentes y cantidades pequeñas.
6. Comprueba que el pedido se creó como `Pendiente` y que todavía no se descontó el stock.
7. Inicia sesión como trabajador y llama a `POST /api/orders/{id}/confirm` con el ID del pedido recién creado.
8. Consulta nuevamente el inventario, los pedidos, los movimientos de caja y `/api/reports/top-products`.
9. Confirma que el stock bajó por la cantidad comprada, existe una venta, hay un ingreso por el total de la venta y el pedido está `Completado`.
10. Intenta confirmar otra vez el mismo pedido: debe rechazarse sin duplicar venta ni ingreso.
11. Repite con una cantidad superior al stock disponible: la confirmación debe fallar sin dejar cambios parciales.

Haz esta prueba en una base local de desarrollo. No uses pedidos reales ni datos de producción. Los nombres exactos de campos del JSON se deben contrastar con los DTO actuales y `docs/api-contratos.md`.

## Seguridad y configuración

- No subas `Docker/.env`, claves JWT reales ni contraseñas reales al repositorio.
- `Docker/.env.example` es una plantilla de desarrollo; configura valores locales propios.
- Las credenciales de seed son únicamente para pruebas locales y no deben reutilizarse en producción.
- Swagger debe estar limitado al entorno de desarrollo si así lo exige la política de despliegue.
- Configura CORS con los orígenes autorizados antes de desplegar.

## Documentación de API

- Contratos: `docs/api-contratos.md`.
- Colección Postman: `docs/postman/TiendaDonJulio.postman_collection.json`.
- Verifica ambos artefactos contra los controladores y DTO actuales antes de cada entrega.

## Solución de problemas

- **Falta una tabla:** revisa `docker compose logs db`. Si el volumen se creó antes de añadir los scripts, reinicia la BD local con `docker compose down -v` y vuelve a levantarla.
- **La API no conecta con PostgreSQL:** verifica que `db` esté healthy y que la cadena de conexión dentro de Compose use `Host=db`.
- **El puerto 5432, 5222 o 5142 está ocupado:** detén el servicio que lo utiliza o cambia el mapeo de puertos.
- **La web no conecta con la API:** revisa los logs de `web` y `api`; dentro de Compose, la web debe usar la dirección interna de la API configurada en `Api__BaseUrl`.
- **Se pierde la sesión al recargar:** verifica que la persistencia del JWT en el navegador esté implementada y probada; no des por resuelto el problema solo porque el login funciona.
