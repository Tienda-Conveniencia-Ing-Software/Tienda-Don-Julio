namespace TiendaBarrio.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Persistence.Interfaces;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly FinanceService _financeService;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        FinanceService financeService)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _financeService = financeService;
    }

    public async Task<Order?> ConfirmOrderAsync(CartService cart, List<Product> products)
    {
        if (cart.Items.Count == 0)
        {
            return null;
        }

        var order = new Order(0, new List<CartItem>(cart.Items));

        foreach (var item in order.Items)
        {
            item.Product.ReduceStock(item.Quantity);
        }

        // ✅ Actualizar stock de los productos
        foreach (var item in order.Items)
        {
            await _productRepository.UpdateAsync(item.Product);
        }
        await _productRepository.SaveChangesAsync();

        order.AdvanceStatus(OrderStatus.Confirmado);

        // ✅ Guardar el pedido
        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        // ✅ Registrar el ingreso
        await _financeService.RegisterIncomeAsync(order.Total, $"Venta pedido #{order.Id}");

        cart.Clear();

        return order;
    }

    public async Task<List<Order>> GetHistoryAsync()
    {
        return await _orderRepository.GetAllAsync();
    }
}