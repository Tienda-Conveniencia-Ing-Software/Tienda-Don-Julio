# Tienda-Don-Julio

Sistema de gestión para tienda de conveniencia desarrollado en C# con .NET.  

---

## Arquitectura del Proyecto

El proyecto está organizado en capas para separar responsabilidades:

| Capa | Descripción | Archivos |
| :--- | :--- | :--- |
| **Modelos (Core/Models)** | Define la entidad del negocio. | `Product.cs` |
| **Utilidades (Core/Utils)** | Funciones auxiliares (pausa, mostrar productos). | `ConsoleHelper.cs`, `InventoryMenu.cs` |
| **Repositorios (Persistence)** | Gestiona la persistencia de datos en archivo de texto. | `ProductRepository.cs` |
| **Servicios (Modules/Inventario)** | Contiene la lógica de negocio del inventario. | `InventoryService.cs` |
| **Interfaz de Usuario (UI)** | Menús e interacción con el usuario. | `MainMenu.cs`, `SalesMenu.cs` |
| **Punto de entrada** | Archivo principal que inicia la aplicación. | `Program.cs` |

---

### Flujo de datos actual
1. El usuario interactúa con el menú principal (`MainMenu.cs`).
2. Según la opción, se llama a `InventoryMenu.cs` (ver stock), `SalesMenu.cs` (comprar) o `InventoryService.cs` (agregar stock/producto).
3. Los servicios utilizan `ProductRepository.cs` para leer/escribir en el archivo `Data/productos.txt`.
4. El repositorio trabaja con el modelo `Product` (`Core/Models/Product.cs`).

---

## Componentes del Proyecto

| Componente | Ubicación | Responsabilidad | Métodos principales |
| :--- | :--- | :--- | :--- |
| **Product** | `Core/Models/Product.cs` | Define la entidad Producto. | `ReduceStock()`, `IncreaseStock()` |
| **ConsoleHelper** | `Core/Utils/ConsoleHelper.cs` | Pausa la ejecución hasta que el usuario presione una tecla. | `pause()` |
| **ShowProducts** | `Core/Utils/InventoryMenu.cs` | Muestra el listado de productos en consola. | `StockMenu()` |
| **ProductRepository** | `Persistence/ProductRepository.cs` | CRUD de productos en archivo de texto (`Data/productos.txt`). | `LoadProducts()`, `SaveProducts()` |
| **InventoryService** | `Modules/Inventario/InventoryService.cs` | Lógica de negocio para el inventario. | `FoundProduct()`, `AddStock()` |
| **MainMenu** | `UI/MainMenu.cs` | Menú principal. Inicializa la aplicación. | `Start()`, `ShopMenu()` |
| **SalesMenu** | `UI/SalesMenu.cs` | Lógica de compra de productos. | `BuyStock()` |
| **Program** | `Program.cs` | Punto de entrada. Crea el menú y lo ejecuta. | `Main()` |

---

## Funcionalidades Implementadas 

| Funcionalidad | ¿Cómo funciona? | Archivo responsable |
| :--- | :--- | :--- |
| **Ver stock** | Muestra todos los productos con ID, nombre, precio y stock. | `ShowProducts.StockMenu()` |
| **Comprar producto** | Busca un producto por ID, valida que haya stock suficiente y reduce el stock. | `SalesMenu.BuyStock()` |
| **Agregar stock** | Permite agregar stock a un producto existente o crear uno nuevo. | `InventoryService.AddStock()` |
| **Persistencia** | Los productos se guardan en el archivo `Data/productos.txt` con formato `ID;Nombre;Precio;Stock`. | `ProductRepository.SaveProducts()` |

---

### Formato del archivo `productos.txt`
12; CocaCola 250ml ;2500;88
13; Agua 1L ;1000;200

---

## Validaciones Actuales 

| Validación | ¿Está implementada? | Dónde |
| :--- | :--- | :--- |
| **Stock suficiente al comprar** | Sí | `SalesMenu.BuyStock()` |
| **Cantidad positiva al agregar stock** | Sí | `Product.IncreaseStock()` |
| **Cantidad positiva al comprar** | Sí | `Product.ReduceStock()` |
| **Manejo de errores (try-catch)** | Parcial | En `AddStock()`, `BuyStock()`, `StockMenu()` |
| **Validación de precio negativo** | No | `Product` no valida que `Price` sea > 0 |
| **Validación de stock negativo** | No | `Product` no valida que `Stock` sea >= 0 |
| **Validación de entrada (números)** | No | Usa `int.TryParse` pero no valida si es 0 o negativo |
| **Validación de ID existente** | Sí | `InventoryService.FoundProduct()` |

---

## Criterios de Aceptación Generales

Para considerar una funcionalidad como **completada**, debe cumplir con:

1. **Funcionalidad**: La característica funciona según lo especificado.
2. **Validaciones**: Los datos de entrada son validados (precios > 0, stock >= 0, stock suficiente).
3. **Manejo de errores**: La aplicación no se cae ante entradas incorrectas; muestra mensajes amigables.
4. **Persistencia**: Los datos se guardan correctamente en `Data/productos.txt`.
5. **Documentación**: El código está comentado y el README actualizado.

---

## Flujo de Ramas, Issues y Pull Requests (PR)

### Ramas
- **`main`**: Rama principal y estable. Solo recibe cambios a través de Pull Requests aprobados.
- **`feature/#[numero]-nombre`**: Ramas individuales para cada Issue.

### Proceso para contribuir
1. **Asignar Issue**: Cada miembro se asigna un Issue antes de empezar a trabajar.
2. **Crear rama**: `git checkout -b feature/#[numero]-descripcion`
3. **Desarrollar**: Hacer commits pequeños con mensajes claros.
4. **Abrir Pull Request (PR)**:
   - Título: `[Issue #N] Descripción breve`
   - Descripción: Explicar qué se hizo y cómo probarlo.
   - Vincular el Issue: `Closes #N`
5. **Revisión**: Al menos un compañero debe revisar y aprobar el PR.
6. **Fusionar (merge)**: Una vez aprobado, se fusiona a `main`.

---

## Integrantes del Equipo

- Valentina Gutierrez
- Tomas Lopez 
- Miker Gomez 
