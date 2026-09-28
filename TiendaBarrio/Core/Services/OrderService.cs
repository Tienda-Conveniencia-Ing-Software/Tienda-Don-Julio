namespace TiendaBarrio.Core.Services;

using TiendaBarrio.Core.Models;
using TiendaBarrio.Persistence.Interfaces;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly FinanceService _financeService;
    private int _nextId = 1;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        FinanceService financeService)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _financeService = financeService;
    }

    public Order? ConfirmOrder(CartService cart, List<Product> products)
    {
        if (cart.Items.Count == 0)
        {
            return null;
        }

        var order = new Order(_nextId, new List<CartItem>(cart.Items));
        _nextId++;

        foreach (var item in order.Items)
        {
            item.Product.ReduceStock(item.Quantity);
        }

        _productRepository.SaveProducts(products);

        order.AdvanceStatus(OrderStatus.Confirmado);

        _orderRepository.SaveOrder(order);

        _financeService.RegisterIncome(order.Total, $"Venta pedido #{order.Id}");

        cart.Clear();

        return order;
    }

    public List<string> GetHistory()
    {
        return _orderRepository.LoadOrderLines();
    }
}