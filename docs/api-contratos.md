# Contratos y endpoints de la API 

Base URL local: `http://localhost:5222` · Swagger: `http://localhost:5222/swagger`
Formato: JSON (`Content-Type: application/json`). Errores de negocio: `{ "message": "..." }`.

> Estado: los endpoints marcados **[Implementado]** existen hoy en `Api/Controllers`.
> Seguridad: el login devuelve un token JWT. Los endpoints protegidos requieren `Authorization: Bearer <token>`. Si falta el token: 401; si el rol no alcanza: 403.

## Auth

### POST `/api/auth/login` [Implementado]
Request:
```json
{ "email": "admin@donjulio.com", "password": "********" }
```

| Código | Significado | Body |
|---|---|---|
| 200 | Credenciales válidas | `{ "token": "...", "expiresAt": "...", "id": 1, "nombre": "...", "email": "...", "role": "Trabajador" }` |
| 400 | Email o contraseña vacíos | ProblemDetails con los campos inválidos |
| 401 | Credenciales inválidas | `{ "message": "Credenciales inválidas." }` |
| 500 | Error inesperado | - |

### POST `/api/auth/register` [Implementado]
Registra un nuevo cliente. Request:
```json
{ "nombre": "Cliente Test", "email": "cliente@test.com", "password": "123", "cedula": "12345678", "telefono": "3001234567" }
```

| Código | Significado | Body |
|---|---|---|
| 200 | Cliente registrado | `{ "token": "...", "expiresAt": "...", "id": 2, "nombre": "Cliente Test", "email": "cliente@test.com", "role": "Cliente" }` |
| 400 | Datos inválidos (email ya existe, campos vacíos) | `{ "message": "El correo electrónico ya se encuentra registrado." }` |
| 500 | Error inesperado | - |

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
| 201 | Producto registrado | Producto creado (`id`, `name`, `price`, `purchasePrice`, `stock`) |
| 400 | Datos inválidos o sin permiso | `{ "message": "..." }` |
| 401 | Sin token | - |
| 403 | Rol insuficiente | - |
| 500 | Error inesperado | - |

### DELETE `/api/inventory/{id}` [Implementado]

| Código | Significado | Body |
|---|---|---|
| 200 | Producto eliminado | `{ "message": "Producto eliminado correctamente." }` |
| 404 | No existe | `{ "message": "Producto no encontrado." }` |
| 401 | Sin token | - |
| 403 | Rol insuficiente | - |
| 500 | Error inesperado | - |

### PUT `/api/inventory/{id}` [Implementado]
Modifica nombre y precios. Request igual a POST. Respuestas: 200, 400, 401, 403, 404, 500.

### GET `/api/inventory/low-stock` [Implementado]
Productos con stock bajo (umbral definido en `InventoryService`). Respuestas: 200, 500.

### POST `/api/inventory/{id}/stock` [Implementado]
Aumenta o disminuye stock. Request: `{ "quantity": 10 }` (negativo = disminuir). Respuestas: 200, 400, 401, 403, 404, 500.

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

### POST `/api/finance/movements` [Implementado]
Registra un ingreso o gasto manual. Request:
```json
{ "type": "Ingreso", "amount": 5000, "description": "Aporte de caja" }
```
Solo `Ingreso` o `Gasto`. Respuestas: 201, 400 (monto <= 0 o tipo no permitido), 401, 403, 500.

## Pedidos (Orders)

### POST `/api/orders` [Implementado]
Crea un pedido para el cliente autenticado. El `clienteId` se toma automáticamente del token, no del body.
Request:
```json
{ "items": [ { "productId": 1, "quantity": 2 } ] }
```

| Código | Significado | Body |
|---|---|---|
| 200 | Pedido creado exitosamente (devuelve el objeto Order) | |
| 400 | Producto no existe o stock insuficiente | `{ "message": "Stock insuficiente..." }` |
| 401 | Sin token | - |
| 500 | Error inesperado | - |

### GET `/api/orders/mine` [Implementado]
Devuelve la lista de pedidos del cliente autenticado (basado en el token).

| Código | Body |
|---|---|
| 200 | `[ { "id": 1, "clienteId": 1, "clienteNombre": "...", "total": 10000, "status": "Pendiente", "createdAt": "...", "items": [...] } ]` |
| 401 | - |

### GET `/api/orders` [Implementado - Solo Trabajador]
Devuelve todos los pedidos del sistema. Misma estructura que `/mine`.

| Código | Significado |
|---|---|
| 200 | Lista de todos los pedidos |
| 401 | Sin token |
| 403 | El usuario no es Trabajador |

### GET `/api/orders/pending-count` [Implementado - Solo Trabajador]
Devuelve el número de pedidos en estado "Pendiente".

| Código | Body |
|---|---|
| 200 | `{ "pendingCount": 5 }` |
| 401 | - |
| 403 | - |

### PUT `/api/orders/{id}/status` [Implementado - Solo Trabajador]
Cambia el estado de un pedido. Request:
```json
{ "newStatus": "EnProceso" }
```
Estados válidos: `Pendiente`, `EnProceso`, `Completado`, `Cancelado`. (Nota: se debe usar `/confirm` para `Completado`).

| Código | Significado | Body |
|---|---|---|
| 200 | Estado actualizado | |
| 400 | Estado inválido o pedido no encontrado | `{ "message": "Estado inválido." }` |
| 401 | Sin token | - |
| 403 | Rol insuficiente | - |

### POST `/api/orders/{id}/confirm` [Implementado - Solo Trabajador]
Confirma el pedido: descuenta stock, crea la Venta y registra el movimiento en caja.

| Código | Significado | Body |
|---|---|---|
| 200 | Pedido confirmado y venta creada | Objeto Sale con detalles |
| 400 | El pedido no está Pendiente o stock insuficiente | `{ "message": "El pedido ya no está en estado Pendiente." }` |
| 401 | Sin token | - |
| 403 | Rol insuficiente | - |
| 500 | Error inesperado | - |

## Reportes

### GET `/api/reports/top-products` [Implementado - Solo Trabajador]
Devuelve los 5 productos más vendidos.

| Código | Body |
|---|---|
| 200 | `[ { "productId": 8, "productName": "Aceite", "totalQuantitySold": 1, "totalRevenue": 9000 } ]` |
| 401 | - |
| 403 | - |
| 500 | - |

## Códigos de error comunes

| Código | Uso |
|---|---|
| 400 | Datos inválidos en la petición |
| 401 | Token no enviado o expirado |
| 403 | El usuario no tiene el rol necesario |
| 404 | Recurso no encontrado |
| 500 | Error interno del servidor |
