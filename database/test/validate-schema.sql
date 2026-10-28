-- Validación de humo del esquema y los datos iniciales de Tienda Don Julio.
-- Ejecutar contra una base de datos LOCAL de pruebas después de docker compose up.
-- No modifica ni elimina datos.

DO $$
DECLARE
    required_table TEXT;
    table_names TEXT[] := ARRAY[
        'cliente', 'producto', 'pedido', 'carrito_items',
        'venta', 'detalle_venta', 'movimiento'
    ];
    product_count INTEGER;
    client_count INTEGER;
BEGIN
    FOREACH required_table IN ARRAY table_names LOOP
        IF to_regclass(format('public.%I', required_table)) IS NULL THEN
            RAISE EXCEPTION 'Falta la tabla requerida: %', required_table;
        END IF;
    END LOOP;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public' AND table_name = 'pedido' AND column_name = 'id_cliente'
    ) THEN
        RAISE EXCEPTION 'Falta pedido.id_cliente';
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public' AND table_name = 'carrito_items' AND column_name = 'id_pedido'
    ) THEN
        RAISE EXCEPTION 'Falta carrito_items.id_pedido';
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public' AND table_name = 'venta' AND column_name = 'id_usuario'
    ) THEN
        RAISE EXCEPTION 'Falta venta.id_usuario';
    END IF;

    SELECT COUNT(*) INTO product_count FROM producto;
    SELECT COUNT(*) INTO client_count FROM cliente;

    IF product_count = 0 THEN
        RAISE EXCEPTION 'El seed no insertó productos';
    END IF;
    IF client_count = 0 THEN
        RAISE EXCEPTION 'El seed no insertó clientes/usuarios';
    END IF;

    RAISE NOTICE 'OK: tablas, columnas y datos iniciales verificados. Productos: %, cuentas: %',
        product_count, client_count;
END $$;

-- Resumen útil para revisar manualmente el seed.
SELECT 'cliente' AS tabla, COUNT(*) AS registros FROM cliente
UNION ALL SELECT 'producto', COUNT(*) FROM producto
UNION ALL SELECT 'pedido', COUNT(*) FROM pedido
UNION ALL SELECT 'venta', COUNT(*) FROM venta
UNION ALL SELECT 'movimiento', COUNT(*) FROM movimiento
ORDER BY tabla;
