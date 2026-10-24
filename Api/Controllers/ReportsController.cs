using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaBarrio.Core.Services;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Trabajador")]
public class ReportsController : ControllerBase
{
    private readonly SaleService _saleService;

    public ReportsController(SaleService saleService)
    {
        _saleService = saleService;
    }

    [HttpGet("top-products")]
    public async Task<IActionResult> GetTopProducts()
    {
        var topProducts = await _saleService.GetTopSellingProductsAsync();
        return Ok(topProducts);
    }
}
