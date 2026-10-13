using Api.DTOs;
using Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaBarrio.Core.Services;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly InventoryService _inventoryService;

    public InventoryController(InventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    /// <summary>Lista todos los productos (publico).</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ProductResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetProducts()
    {
        var products = _inventoryService.GetProducts();
        return Ok(_inventoryService.GetAllProducts(products).Select(ProductResponseDto.From));
    }

    /// <summary>Obtiene un producto por id (publico).</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetProduct(int id)
    {
        var product = _inventoryService.FindProduct(_inventoryService.GetProducts(), id);
        if (product == null)
        {
            return NotFound(new MessageDto("Producto no encontrado."));
        }
        return Ok(ProductResponseDto.From(product));
    }

    /// <summary>Productos con stock bajo (solo Trabajador).</summary>
    [HttpGet("low-stock")]
    [Authorize(Roles = "Trabajador")]
    [ProducesResponseType(typeof(IEnumerable<ProductResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetLowStock()
    {
        var low = _inventoryService.GetLowStockProducts(_inventoryService.GetProducts());
        return Ok(low.Select(ProductResponseDto.From));
    }

    /// <summary>Registra un producto nuevo (solo Trabajador).</summary>
    [HttpPost]
    [Authorize(Roles = "Trabajador")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult RegisterProduct([FromBody] ProductDto dto)
    {
        var product = _inventoryService.RegisterProduct(
            User.ToCliente(), _inventoryService.GetProducts(),
            dto.Name, dto.Price, dto.PurchasePrice, dto.Stock);

        return CreatedAtAction(nameof(GetProduct), new { id = product.ID }, ProductResponseDto.From(product));
    }

    /// <summary>Modifica nombre y/o precios de un producto (solo Trabajador).</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Trabajador")]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UpdateProduct(int id, [FromBody] UpdateProductDto dto)
    {
        if (dto.Name == null && dto.Price == null && dto.PurchasePrice == null)
        {
            return BadRequest(new MessageDto("Debe enviar al menos un campo para modificar."));
        }

        var updated = _inventoryService.UpdateProduct(
            User.ToCliente(), _inventoryService.GetProducts(),
            id, dto.Name, dto.Price, dto.PurchasePrice);

        return updated
            ? Ok(new MessageDto("Producto modificado correctamente."))
            : NotFound(new MessageDto("Producto no encontrado."));
    }

    /// <summary>Elimina un producto (solo Trabajador).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Trabajador")]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult DeleteProduct(int id)
    {
        var deleted = _inventoryService.DeleteProduct(
            User.ToCliente(), _inventoryService.GetProducts(), id);

        return deleted
            ? Ok(new MessageDto("Producto eliminado correctamente."))
            : NotFound(new MessageDto("Producto no encontrado."));
    }

    /// <summary>Aumenta (cantidad positiva) o disminuye (negativa) el stock (solo Trabajador).</summary>
    [HttpPost("{id:int}/stock")]
    [Authorize(Roles = "Trabajador")]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ChangeStock(int id, [FromBody] StockChangeDto dto)
    {
        if (dto.Quantity == 0)
        {
            return BadRequest(new MessageDto("La cantidad no puede ser 0."));
        }

        var products = _inventoryService.GetProducts();
        if (_inventoryService.FindProduct(products, id) == null)
        {
            return NotFound(new MessageDto("Producto no encontrado."));
        }

        var user = User.ToCliente();
        var ok = dto.Quantity > 0
            ? _inventoryService.IncreaseStock(user, products, id, dto.Quantity)
            : _inventoryService.DecreaseStock(user, products, id, -dto.Quantity);

        return ok
            ? Ok(new MessageDto("Stock actualizado correctamente."))
            : BadRequest(new MessageDto("Cantidad inválida o stock insuficiente."));
    }
}
