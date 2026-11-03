using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TiendaBarrio.Core.DTOs;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Core.Services;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly SaleService _saleService;

    public OrdersController(OrderService orderService, SaleService saleService)
    {
        _orderService = orderService;
        _saleService = saleService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto request)
    {
        var clienteId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        request.ClienteId = clienteId;

        var order = await _orderService.CreateOrderAsync(request);
        return Ok(order);
    }

    [HttpGet("mine")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMyOrders()
    {
        var clienteId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var orders = await _orderService.GetOrdersByClienteAsync(clienteId);
        return Ok(orders);
    }

    [HttpGet]
    [Authorize(Roles = "Trabajador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("pending-count")]
    [Authorize(Roles = "Trabajador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPendingCount()
    {
        var count = await _orderService.GetPendingOrdersCountAsync();
        return Ok(new { pendingCount = count });
    }

    [HttpPost("{id}/confirm")]
    [Authorize(Roles = "Trabajador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ConfirmOrder(int id)
    {
        var trabajadorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var sale = await _saleService.ConfirmOrderAndCreateSaleAsync(id, trabajadorId);
        return Ok(sale);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Trabajador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        if (!Enum.TryParse<OrderStatus>(request.NewStatus, true, out var newStatus))
        {
            return BadRequest(new { message = "Estado inválido." });
        }

        // No permitir Completado por esta vía
        if (newStatus == OrderStatus.Completado)
        {
            return BadRequest(new { message = "Para completar un pedido, use el endpoint POST /api/orders/{id}/confirm." });
        }

        await _orderService.UpdateOrderStatusAsync(id, newStatus);
        return Ok();
    }
}
