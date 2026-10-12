namespace TiendaBarrio.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
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