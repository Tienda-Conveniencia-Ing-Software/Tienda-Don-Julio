using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TiendaBarrio.Core.DTOs;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Persistence.Data;

namespace TiendaBarrio.Core.Services;

public class OrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Crea un nuevo pedido en estado Pendiente sin descontar stock ni tocar finanzas.
    /// </summary>
    public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto)
    {
        if (dto.Items == null || !dto.Items.Any())
        {
            throw new ArgumentException("El pedido debe contener al menos un producto.");
        }

        var cliente = await _context.Clientes.FindAsync(dto.ClienteId)
            ?? throw new KeyNotFoundException($"El cliente con ID {dto.ClienteId} no existe.");

        var cartItems = new List<CartItem>();

        foreach (var item in dto.Items)
        {
            var product = await _context.Productos.FindAsync(item.ProductId)
                ?? throw new KeyNotFoundException($"Producto con ID {item.ProductId} no encontrado.");

            if (product.Stock < item.Quantity)
            {
                throw new InvalidOperationException($"Stock insuficiente para '{product.Name}'. Disponible: {product.Stock}, Solicitado: {item.Quantity}.");
            }

            cartItems.Add(new CartItem
            {
                ProductId = product.ID,
                Product = product,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });
        }

        var order = new Order
        {
            ClienteId = cliente.Id,
            Items = cartItems,
            Total = cartItems.Sum(i => i.Quantity * i.UnitPrice),
            Status = OrderStatus.Pendiente,
            CreatedAt = DateTime.Now
        };

        _context.Pedidos.Add(order);
        await _context.SaveChangesAsync();

        return MapToResponseDto(order, cliente.Nombre);
    }

    /// <summary>
    /// Obtiene los pedidos creados por un cliente específico (para HU-10 Mis Pedidos).
    /// </summary>
    public async Task<List<OrderResponseDto>> GetOrdersByClienteAsync(int clienteId)
    {
        var cliente = await _context.Clientes.FindAsync(clienteId);
        string nombreCliente = cliente?.Nombre ?? "Cliente";

        var orders = await _context.Pedidos
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.ClienteId == clienteId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(o => MapToResponseDto(o, nombreCliente)).ToList();
    }

    /// <summary>
    /// Obtiene todos los pedidos registrados en el sistema (para el Panel del Trabajador - HU-06).
    /// </summary>
    public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
    {
        var orders = await _context.Pedidos
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        var clientesDict = await _context.Clientes
            .ToDictionaryAsync(c => c.Id, c => c.Nombre);

        return orders.Select(o => MapToResponseDto(o, clientesDict.GetValueOrDefault(o.ClienteId, "Desconocido"))).ToList();
    }

    /// <summary>
    /// Obtiene el conteo de pedidos pendientes (para la alerta de notificaciones en Blazor/API).
    /// </summary>
    public async Task<int> GetPendingOrdersCountAsync()
    {
        return await _context.Pedidos.CountAsync(o => o.Status == OrderStatus.Pendiente);
    }

    /// <summary>
    /// Actualiza el estado de un pedido directamente.
    /// </summary>
    public async Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
    {
        var order = await _context.Pedidos.FindAsync(orderId)
            ?? throw new KeyNotFoundException($"Pedido con ID {orderId} no encontrado.");

        order.AdvanceStatus(newStatus);
        await _context.SaveChangesAsync();
    }

    private static OrderResponseDto MapToResponseDto(Order order, string clienteNombre)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            ClienteId = order.ClienteId,
            ClienteNombre = clienteNombre,
            Total = order.Total,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(i => new OrderItemResponseDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? string.Empty,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
    }
}