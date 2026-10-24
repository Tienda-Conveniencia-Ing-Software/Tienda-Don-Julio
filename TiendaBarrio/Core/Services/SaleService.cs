using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TiendaBarrio.Core.DTOs;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Persistence.Data;

namespace TiendaBarrio.Core.Services;

public class SaleService
{
    private readonly AppDbContext _context;

    public SaleService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Confirma un pedido de forma atómica (Transacción SQL): descuenta stock, crea la Venta y registra el Movimiento de Caja.
    /// </summary>
    public async Task<Sale> ConfirmOrderAndCreateSaleAsync(int orderId, int trabajadorId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var order = await _context.Pedidos
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId)
                ?? throw new KeyNotFoundException($"Pedido con ID {orderId} no encontrado.");

            if (order.Status != OrderStatus.Pendiente)
            {
                throw new InvalidOperationException($"El pedido #{orderId} ya no se encuentra en estado Pendiente.");
            }

            var trabajador = await _context.Clientes.FindAsync(trabajadorId);
            if (trabajador == null || !trabajador.EsTrabajador())
            {
                throw new UnauthorizedAccessException("El usuario especificado no tiene rol de Trabajador.");
            }

            var details = new List<SaleDetail>();

            foreach (var item in order.Items)
            {
                var product = await _context.Productos.FindAsync(item.ProductId)
                    ?? throw new KeyNotFoundException($"Producto con ID {item.ProductId} no encontrado.");

                if (product.Stock < item.Quantity)
                {
                    throw new InvalidOperationException($"Stock insuficiente para el producto '{product.Name}'. Stock disponible: {product.Stock}, solicitado: {item.Quantity}.");
                }

                // 1. Descuento de stock en entidad
                product.ReduceStock(item.Quantity);

                // 2. Detalle de venta con costos históricos
                var saleDetail = new SaleDetail(
                    product.ID,
                    product.Name,
                    item.Quantity,
                    product.Price,
                    product.PurchasePrice
                );
                details.Add(saleDetail);
            }

            // 3. Creación de la Entidad Venta y cálculo de ganancias
            var sale = new Sale
            {
                Date = DateTime.Now,
                Details = details,
                ClienteId = order.ClienteId,
                UsuarioId = trabajadorId
            };
            sale.CalculateTotals();

            _context.Ventas.Add(sale);

            // 4. Registro en Movimientos de Caja
            var cashMovement = new CashMovement
            {
                Type = MovementType.Ingreso,
                Amount = sale.Total,
                Description = $"Ingreso por Venta del Pedido #{order.Id}",
                Date = DateTime.Now,
                UsuarioId = trabajadorId    
            };
            _context.Movimientos.Add(cashMovement);

            // 5. Cambio de Estado del Pedido
            order.AdvanceStatus(OrderStatus.Completado);

            // Persistir todos los cambios juntos en la BD
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return sale;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    // Compatibilidad para la interfaz de consola anterior
    public async Task<Sale> ProcessSaleAsync(int orderId, int trabajadorId)
    {
        return await ConfirmOrderAndCreateSaleAsync(orderId, trabajadorId);
    }

    /// <summary>
    /// Reporte de Productos Más Vendidos para HU-14.
    /// </summary>
    public async Task<List<TopProductDto>> GetTopSellingProductsAsync(int count = 5)
    {
        var topProducts = await _context.DetalleVentas
            .GroupBy(d => new { d.ProductId, d.ProductName })
            .Select(g => new TopProductDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.ProductName,
                TotalQuantitySold = g.Sum(d => d.Quantity),
                TotalRevenue = g.Sum(d => d.Quantity * d.UnitPrice)
            })
            .OrderByDescending(p => p.TotalQuantitySold)
            .Take(count)
            .ToListAsync();

        return topProducts;
    }
    //// <summary>
    /// Sobrecarga para mantener compatibilidad con la interfaz de consola legada (SalesMenu.cs).
    /// </summary>
    public async Task<Sale?> ProcessSaleAsync(CartService cartService, List<Product> products)
    {
        if (cartService == null || !cartService.Items.Any())
        {
            return null;
        }

        var details = new List<SaleDetail>();
        foreach (var item in cartService.Items)
        {
            var p = products.FirstOrDefault(pr => pr.ID == item.ProductId || (item.Product != null && pr.ID == item.Product.ID));
            if (p != null)
            {
                p.ReduceStock(item.Quantity);
                details.Add(new SaleDetail(p.ID, p.Name, item.Quantity, p.Price, p.PurchasePrice));
            }
        }

        var sale = new Sale
        {
            Date = DateTime.Now,
            Details = details
        };
        sale.CalculateTotals();

        _context.Ventas.Add(sale);
        await _context.SaveChangesAsync();
        cartService.Clear();

        return sale;
    }
}