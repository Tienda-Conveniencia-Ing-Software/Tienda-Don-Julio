# Contratos y endpoints de la API (Issue #43)

Base URL local: `http://localhost:5222` · Swagger: `http://localhost:5222/swagger`
Formato: JSON (`Content-Type: application/json`). Errores de negocio: `{ "message": "..." }`.

> Estado: los endpoints marcados **[Implementado]** existen hoy en `Api/Controllers`.
> Los marcados **[Pendiente]** son el contrato acordado para la issue #53.

## Auth

### POST `/api/auth/login` [Implementado]
Request:
```json
{ "email": "admin@donjulio.com", "password": "********" }
```
| Código | Significado | Body |
|---|---|---|
| 200 | Credenciales válidas | Datos del cliente (ver nota de seguridad) |
| 400 | Email o contraseña vacíos | `{ "message": "El correo y la contraseña son obligatorios." }` |
| 401 | Credenciales inválidas | `{ "message": "Credenciales inválidas." }` |
| 500 | Error inesperado | - |

> Nota de seguridad: hoy la respuesta 200 devuelve el objeto `Cliente` completo, incluido `PasswordHash`. Debe cambiarse por un DTO (`id`, `nombre`, `email`, `role`) o un token.

## Inventario

### GET `/api/inventory` [Implementado]
Lista todos los productos.
| Código | Body |
|---|---|
| 200 | `[ { "id": 1, "name": "Arroz 1kg", "price": 4500, "purchasePrice": 3500, "stock": 50 } ]` |
| 500 | - |

### POST `/api/inventory` [Implementado]
Registra un producto. Request:
```json
{ "name": "Leche 1L", "price": 3800, "purchasePrice": 3000, "stock": 30 }
```
| Código | Significado | Body |
|---|---|---|
| 200 | Producto registrado | `{ "message": "Producto registrado correctamente." }` |
| 400 | Datos inválidos o sin permiso | `{ "message": "..." }` |
| 401 | Sin sesión (cuando exista autenticación) | - |
| 500 | Error inesperado | - |

### DELETE `/api/inventory/{id}` [Implementado]
| Código | Significado | Body |
|---|---|---|
| 200 | Producto eliminado | `{ "message": "Producto eliminado correctamente." }` |
| 400 | No existe o sin permiso | `{ "message": "..." }` |
| 401 | Sin sesión | - |
| 500 | Error inesperado | - |

### PUT `/api/inventory/{id}` [Pendiente]
Modifica nombre y precios. Request igual a POST. Respuestas: 200, 400, 401, 404, 500.

### GET `/api/inventory/low-stock` [Pendiente]
Productos con stock bajo (umbral definido en `InventoryService`). Respuestas: 200, 500.

### POST `/api/inventory/{id}/stock` [Pendiente]
Aumenta o disminuye stock. Request: `{ "quantity": 10 }` (negativo = disminuir). Respuestas: 200, 400, 401, 404, 500.

## Finanzas

### GET `/api/finance/balance` [Implementado]
| Código | Body |
|---|---|
| 200 | `{ "totalIncome": 0, "totalExpense": 0, "currentBalance": 0 }` |
| 500 | - |

### GET `/api/finance/movements` [Implementado]
| Código | Body |
|---|---|
| 200 | `[ { "id": 1, "type": "Ingreso", "amount": 1000, "description": "Venta #1", "date": "2026-10-12T10:00:00" } ]` |
| 500 | - |

`type` puede ser `Ingreso`, `Gasto` o `CompraInventario`.

### POST `/api/finance/movements` [Pendiente]
Registra un ingreso o gasto manual. Request:
```json
{ "type": "Ingreso", "amount": 5000, "description": "Aporte de caja" }
```
Respuestas: 200/201, 400 (monto <= 0), 401, 500.

## Ventas, pedidos y clientes [Pendiente, fuera de #53]
`POST /api/sales` (checkout), `GET /api/sales`, `GET /api/orders`, `POST /api/clients`. Se definirán cuando Persona 3 cierre la limpieza de `Order` vs `Sale`.

## Códigos de error comunes
| Código | Uso |
|---|---|
| 400 | Request mal formado o regla de negocio incumplida |
| 401 | Falta autenticación o credenciales inválidas |
| 404 | Recurso inexistente |
| 500 | Error no controlado del servidor |
