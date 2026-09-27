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
    private readonly FinanceService _financeService;

    public SaleService(FinanceService financeService)
    {
        _saleRepository = new SaleRepository();
        _productRepository = new ProductRepository();
        _financeService = financeService;
    }

    public Sale ProcessSale(CartService cart, List<Product> products)
    {
        if (cart == null || cart.Items.Count == 0)
        {
            Console.WriteLine("❌ Error: El carrito está vacío. No se puede procesar la venta.");
            return null;
        }

        var sale = new Sale();
        sale.Id = _saleRepository.GetNextSaleId();

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

        foreach (var item in cart.Items)
        {
            var product = products.First(p => p.ID == item.Product.ID);

            product.ReduceStock(item.Quantity);

            var detail = new SaleDetail(
                product.ID,
                product.Name,
                item.Quantity,
                product.Price,
                product.PurchasePrice
            );

            sale.Details.Add(detail);
        }

        sale.CalculateTotals();

        _saleRepository.SaveSale(sale);
        _productRepository.SaveProducts(products);

        _financeService.RegisterIncome(sale.Total, $"Venta #{sale.Id}");

        cart.Items.Clear();

        Console.WriteLine($"Venta #{sale.Id} procesada exitosamente.");
        Console.WriteLine($"   Total: {sale.Total}$ | Costo: {sale.TotalCost}$ | Ganancia: {sale.Profit}$");

        return sale;
    }
}