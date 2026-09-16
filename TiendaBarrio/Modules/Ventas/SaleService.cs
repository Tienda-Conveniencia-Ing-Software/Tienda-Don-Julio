namespace TiendaBarrio.Modules.Ventas;

using System;
using System.Collections.Generic;
using System.Linq;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Core.Services;
using TiendaBarrio.Persistence;

public class SaleService
{
    private readonly SaleRepository _saleRepository;
    private readonly ProductRepository _productRepository;

    public SaleService()
    {
        _saleRepository = new SaleRepository();
        _productRepository = new ProductRepository();
    }

    /// <summary>
    /// Procesa una venta a partir del carrito de compras.
    /// Valida stock, calcula totales, disminuye inventario y guarda la venta.
    /// </summary>
    public Sale ProcessSale(CartService cart, List<Product> products)
    {
        // VALIDACIÓN: El carrito no puede estar vacío
        if (cart == null || cart.Items.Count == 0)
        {
            Console.WriteLine("❌ Error: El carrito está vacío. No se puede procesar la venta.");
            return null;
        }

        var sale = new Sale();
        sale.Id = _saleRepository.GetNextSaleId();

        // VALIDACIÓN: Verificar stock de todos los productos ANTES de procesar
        foreach (var item in cart.Items)
        {
            var product = products.FirstOrDefault(p => p.ID == item.Product.ID);

            if (product == null)
            {
                Console.WriteLine($"❌ Error: Producto '{item.Product.Name}' no encontrado en el inventario.");
                return null;
            }

            if (product.Stock < item.Quantity)
            {
                Console.WriteLine($"❌ Error: Stock insuficiente para '{product.Name}'. Disponible: {product.Stock}, solicitado: {item.Quantity}.");
                return null;
            }
        }

        // Si todas las validaciones pasan, procesamos la venta
        foreach (var item in cart.Items)
        {
            var product = products.First(p => p.ID == item.Product.ID);

            // Disminuir stock
            product.ReduceStock(item.Quantity);

            // Crear detalle de venta
            var detail = new SaleDetail(
                product.ID,
                product.Name,
                item.Quantity,
                product.Price,           // Precio de venta
                product.PurchasePrice    // Precio de compra
            );

            sale.Details.Add(detail);
        }

        // Calcular totales (total, costo, ganancia)
        sale.CalculateTotals();

        // Guardar la venta en el archivo
        _saleRepository.SaveSale(sale);

        // Guardar los productos actualizados (con el stock disminuido)
        _productRepository.SaveProducts(products);

        // Limpiar el carrito después de la venta
        cart.Items.Clear();

        Console.WriteLine($"Venta #{sale.Id} procesada exitosamente.");
        Console.WriteLine($"   Total: {sale.Total}$ | Costo: {sale.TotalCost}$ | Ganancia: {sale.Profit}$");

        return sale;
    }
}
