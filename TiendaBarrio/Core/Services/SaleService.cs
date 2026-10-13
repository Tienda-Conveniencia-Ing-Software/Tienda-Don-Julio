namespace TiendaBarrio.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Persistence.Interfaces;

public class SaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly FinanceService _financeService;

    public SaleService(
        ISaleRepository saleRepository,
        IProductRepository productRepository,
        FinanceService financeService)
    {
        _saleRepository = saleRepository ?? throw new ArgumentNullException(nameof(saleRepository));
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _financeService = financeService ?? throw new ArgumentNullException(nameof(financeService));
    }

    public async Task<Sale?> ProcessSaleAsync(CartService cart, List<Product> products)
    {
        if (cart == null || cart.Items.Count == 0)
        {
            return null;
        }

        // Validar stock de todos los productos ANTES de procesar
        foreach (var item in cart.Items)
        {
            var product = products.FirstOrDefault(p => p.ID == item.Product.ID);

            if (product == null)
            {
                return null;
            }

            if (product.Stock < item.Quantity)
            {
                return null;
            }
        }

        var sale = new Sale();

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

        // Guardar la venta con EF Core (el Id lo asigna la BD)
        await _saleRepository.AddAsync(sale);
        await _saleRepository.SaveChangesAsync();

        // Actualizar el stock de los productos
        foreach (var item in cart.Items)
        {
            var product = products.First(p => p.ID == item.Product.ID);
            await _productRepository.UpdateAsync(product);
        }
        await _productRepository.SaveChangesAsync();

        // Registrar el ingreso en finanzas
        await _financeService.RegisterIncomeAsync(sale.Total, $"Venta #{sale.Id}");

        cart.Items.Clear();

        return sale;
    }
}