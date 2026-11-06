# Diagramas de Secuencia UML

Este directorio contiene los diagramas de secuencia que modelan los flujos principales del sistema Tienda Don Julio.

## 1. Login (`Login.png`)
Muestra el proceso de autenticación de un usuario. Incluye el flujo normal (200 OK) y el flujo alterno cuando las credenciales son inválidas (401 Unauthorized).

## 2. Crear Pedido (`CrearPedido.png`)
Muestra el proceso cuando un cliente finaliza una compra. Incluye la validación de stock y el flujo alterno cuando no hay suficiente inventario (400 Bad Request).

## 3. Confirmar Venta (`ConfirmarVenta.png`)
Muestra el proceso cuando un trabajador confirma un pedido. Incluye el descuento de stock, la creación de la venta y el registro en caja. El flujo alterno muestra qué pasa si el pedido ya fue procesado.
