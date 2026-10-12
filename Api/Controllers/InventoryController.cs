using Microsoft.AspNetCore.Mvc;
using TiendaBarrio.Core.Services;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Persistence.Interfaces;
using Api.DTOs;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly InventoryService _inventoryService;
    private readonly IProductRepository _productRepository;

    public InventoryController(InventoryService inventoryService, IProductRepository productRepository)
    {
        _inventoryService = inventoryService;
        _productRepository = productRepository;
    }

    [HttpGet]
    public IActionResult GetProducts()
    {
        var products = _productRepository.LoadProducts();
        return Ok(products);
    }

    [HttpPost]
    public IActionResult RegisterProduct([FromBody] ProductDto dto)
    {
        try
        {
            var products = _productRepository.LoadProducts();
            var workerUser = new Cliente { Role = "Trabajador" };
            _inventoryService.RegisterProduct(workerUser, products, dto.Name, dto.Price, dto.PurchasePrice, dto.Stock);
            return Ok(new { message = "Producto registrado correctamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteProduct(int id)
    {
        try
        {
            var products = _productRepository.LoadProducts();
            var workerUser = new Cliente { Role = "Trabajador" };
            _inventoryService.DeleteProduct(workerUser, products, id);
            return Ok(new { message = "Producto eliminado correctamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}