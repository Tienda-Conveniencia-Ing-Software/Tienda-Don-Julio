# Modelo Relacional — Tienda Don Julio

## Diagrama Entidad-Relación

![Modelo Relacional]

## Tablas

### Producto
- Id_Producto (PK)
- Nombre_producto
- Precio_ventas
- Precio_compra
- Stock

### Cliente
- Id_Cliente (PK)
- Nombre_cliente
- Cedula
- Telefono
- Email

### Venta
- Id_Venta (PK)
- Fecha
- Total
- Total_costo
- Ganancia

### Detalle_Venta
- Id_Detalle_Venta (PK)
- Id_Venta (FK → Venta)
- Id_Producto (FK → Producto)
- Cantidad
- Precio_Unitario
- Costo_Unitario

### Pedido
- Id_Pedido (PK)
- Id_Cliente (FK → Cliente)
- Fecha
- Estado
- Total

### Detalle_Pedido
- Id_Detalle_Pedidos (PK)
- Id_Pedido (FK → Pedido)
- Id_Producto (FK → Producto)
- Cantidad
- Precio_Unitario

### Movimiento
- Id_Movimiento (PK)
- Fecha_movimiento
- Tipo
- Descripcion
- Monto

## Relaciones

| Relación | Cardinalidad |
| :--- | :--- |
| Venta → Detalle_Venta | 1 a N |
| Producto → Detalle_Venta | 1 a N |
| Cliente → Pedido | 1 a N |
| Pedido → Detalle_Pedido | 1 a N |
| Producto → Detalle_Pedido | 1 a N |
