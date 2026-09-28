namespace TiendaBarrio.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using TiendaBarrio.Core.Models;
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

    public Sale? ProcessSale(CartService cart, List<Product> products)
    {
        if (cart == null || cart.Items.Count == 0)
        {
            return null;
        }

        var sale = new Sale();
        sale.Id = _saleRepository.GetNextSaleId();

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

        return sale;
    }
}