-- =====================================================================
-- Tienda Don Julio - Datos iniciales de prueba
-- Contraseñas (solo desarrollo):
--   admin@donjulio.com  -> admin123   (Trabajador)
--   los demás usuarios  -> test123
-- Los hashes son BCrypt; nunca se guarda texto plano.
-- =====================================================================

BEGIN;

-- ---------------------------------------------------------------------
-- Cuentas
-- ---------------------------------------------------------------------
INSERT INTO cliente (id_cliente, nombre_cliente, cedula, telefono, email, contrasena_hash, rol) VALUES
 (1, 'Administrador Don Julio', '123456789', '3001234567', 'admin@donjulio.com',
     '$2b$11$rLjyHkVUytKYZbxncw/63eKyr2MuYGtdDzIT6gY/0JN3RK3qZPVwe', 'Trabajador'),
 (2, 'Juan Perez',   '12345678', '3001234567', 'juan@test.com',
     '$2b$11$calnlnE2nMpqpTyMN89Ul.G3YX2fnqG1YsL3pc/KflTvxRKcmW.1e', 'Cliente'),
 (3, 'Maria Lopez',  '87654321', '3007654321', 'maria@test.com',
     '$2b$11$calnlnE2nMpqpTyMN89Ul.G3YX2fnqG1YsL3pc/KflTvxRKcmW.1e', 'Cliente'),
 (4, 'Carlos Ruiz',  '11223344', '3001122334', 'carlos@test.com',
     '$2b$11$calnlnE2nMpqpTyMN89Ul.G3YX2fnqG1YsL3pc/KflTvxRKcmW.1e', 'Trabajador'),
 (5, 'Ana Torres',   '44332211', '3004433221', 'ana@test.com',
     '$2b$11$calnlnE2nMpqpTyMN89Ul.G3YX2fnqG1YsL3pc/KflTvxRKcmW.1e', 'Cliente'),
 (6, 'Pedro Gomez',  '55667788', '3005566778', 'pedro@test.com',
     '$2b$11$calnlnE2nMpqpTyMN89Ul.G3YX2fnqG1YsL3pc/KflTvxRKcmW.1e', 'Cliente');

-- ---------------------------------------------------------------------
-- Catálogo base
-- ---------------------------------------------------------------------
INSERT INTO producto (id_producto, nombre_producto, precio_ventas, precio_compra, stock) VALUES
 (1, 'Arroz',   2500, 1800, 100),
 (2, 'Leche',   3500, 2800,  50),
 (3, 'Pan',     2000, 1500,  80),
 (4, 'Huevos',   500,  350, 200),
 (5, 'Café',    8000, 6000,  30),
 (6, 'Azúcar',  3000, 2200,  60),
 (7, 'Sal',     1500, 1000, 100),
 (8, 'Aceite',  9000, 7000,  40);

-- ---------------------------------------------------------------------
-- Historial de ventas (alimenta "productos más vendidos" y las finanzas)
-- id_usuario = trabajador que procesó la venta (1 = admin, 4 = Carlos)
-- ---------------------------------------------------------------------
-- id_cliente = comprador (NULL = venta de mostrador); la venta #2 viene del pedido #1 de Juan.
INSERT INTO venta (id_venta, id_cliente, id_usuario, fecha_venta, total, total_costo, ganancia) VALUES
 (1, NULL, 1, now() - interval '12 days', 20500, 15200, 5300),
 (2, 2,    4, now() - interval '9 days',  27000, 20200, 6800),
 (3, NULL, 1, now() - interval '6 days',  24500, 18000, 6500),
 (4, NULL, 4, now() - interval '3 days',  29500, 21900, 7600),
 (5, NULL, 1, now() - interval '1 day',   22000, 16600, 5400);

INSERT INTO detalle_venta (id_detalle_venta, id_venta, id_producto, nombre_producto, cantidad, precio_unitario, costo_unitario) VALUES
 ( 1, 1, 1, 'Arroz',   3, 2500, 1800),
 ( 2, 1, 2, 'Leche',   2, 3500, 2800),
 ( 3, 1, 4, 'Huevos', 12,  500,  350),
 ( 4, 2, 5, 'Café',    2, 8000, 6000),
 ( 5, 2, 6, 'Azúcar',  1, 3000, 2200),
 ( 6, 2, 3, 'Pan',     4, 2000, 1500),
 ( 7, 3, 1, 'Arroz',   5, 2500, 1800),
 ( 8, 3, 8, 'Aceite',  1, 9000, 7000),
 ( 9, 3, 7, 'Sal',     2, 1500, 1000),
 (10, 4, 4, 'Huevos', 30,  500,  350),
 (11, 4, 2, 'Leche',   3, 3500, 2800),
 (12, 4, 3, 'Pan',     2, 2000, 1500),
 (13, 5, 1, 'Arroz',   2, 2500, 1800),
 (14, 5, 5, 'Café',    1, 8000, 6000),
 (15, 5, 8, 'Aceite',  1, 9000, 7000);

-- ---------------------------------------------------------------------
-- Pedidos de ejemplo
--   #1 completado (su venta es la #2), #2 y #3 pendientes para la demo del
--   panel del trabajador (HU-06) y "Mis pedidos" (HU-10), #4 cancelado.
--   Estados: Pendiente | EnProceso | Completado | Cancelado
-- ---------------------------------------------------------------------
INSERT INTO pedido (id_pedido, id_cliente, fecha, estado, total) VALUES
 (1, 2, now() - interval '9 days',  'Completado',  27000),
 (2, 3, now() - interval '5 hours', 'Pendiente',   10000),
 (3, 5, now() - interval '2 hours', 'Pendiente',   14000),
 (4, 6, now() - interval '2 days',  'Cancelado',    9500);

INSERT INTO carrito_items (id_carrito_items, id_pedido, id_producto, cantidad, precio_unitario) VALUES
 (1, 1, 5,  2, 8000),   -- Café x2
 (2, 1, 6,  1, 3000),   -- Azúcar x1
 (3, 1, 3,  4, 2000),   -- Pan x4
 (4, 2, 2,  2, 3500),   -- Leche x2
 (5, 2, 4,  6,  500),   -- Huevos x6
 (6, 3, 5,  1, 8000),   -- Café x1
 (7, 3, 6,  2, 3000),   -- Azúcar x2
 (8, 4, 7,  3, 1500),   -- Sal x3
 (9, 4, 4, 10,  500);   -- Huevos x10

-- ---------------------------------------------------------------------
-- Libro de caja (balance inicial resultante: 213.500)
--   300.000 + 123.500 (ventas) - 150.000 (compra) - 60.000 (servicios)
-- ---------------------------------------------------------------------
INSERT INTO movimiento (id_movimiento, id_usuario, fecha_movimiento, tipo, descripcion, monto) VALUES
 (1, 1, now() - interval '14 days', 'Ingreso',          'Capital inicial de caja',               300000),
 (2, 1, now() - interval '13 days', 'CompraInventario', 'Compra de inventario: surtido general', 150000),
 (3, 1, now() - interval '12 days', 'Ingreso',          'Venta #1',                               20500),
 (4, 4, now() - interval '9 days',  'Ingreso',          'Venta #2',                               27000),
 (5, 1, now() - interval '7 days',  'Gasto',            'Pago de servicios públicos',             60000),
 (6, 1, now() - interval '6 days',  'Ingreso',          'Venta #3',                               24500),
 (7, 4, now() - interval '3 days',  'Ingreso',          'Venta #4',                               29500),
 (8, 1, now() - interval '1 day',   'Ingreso',          'Venta #5',                               22000);

-- ---------------------------------------------------------------------
-- Como se insertaron IDs explícitos, hay que avanzar las secuencias para
-- que el siguiente INSERT sin ID no choque con los datos de arriba.
-- ---------------------------------------------------------------------
SELECT setval(pg_get_serial_sequence('cliente',       'id_cliente'),       (SELECT MAX(id_cliente)       FROM cliente));
SELECT setval(pg_get_serial_sequence('producto',      'id_producto'),      (SELECT MAX(id_producto)      FROM producto));
SELECT setval(pg_get_serial_sequence('pedido',        'id_pedido'),        (SELECT MAX(id_pedido)        FROM pedido));
SELECT setval(pg_get_serial_sequence('carrito_items', 'id_carrito_items'), (SELECT MAX(id_carrito_items) FROM carrito_items));
SELECT setval(pg_get_serial_sequence('venta',         'id_venta'),         (SELECT MAX(id_venta)         FROM venta));
SELECT setval(pg_get_serial_sequence('detalle_venta', 'id_detalle_venta'), (SELECT MAX(id_detalle_venta) FROM detalle_venta));
SELECT setval(pg_get_serial_sequence('movimiento',    'id_movimiento'),    (SELECT MAX(id_movimiento)    FROM movimiento));

COMMIT;
