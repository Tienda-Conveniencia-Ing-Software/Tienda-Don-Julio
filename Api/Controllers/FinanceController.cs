using Microsoft.AspNetCore.Mvc;
using TiendaBarrio.Core.Services;
using TiendaBarrio.Persistence.Interfaces;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FinanceController : ControllerBase
{
    private readonly FinanceService _financeService;
    private readonly ICashRepository _cashRepository;

    public FinanceController(FinanceService financeService, ICashRepository cashRepository)
    {
        _financeService = financeService;
        _cashRepository = cashRepository;
    }

    [HttpGet("balance")]
    public IActionResult GetBalance()
    {
        return Ok(_financeService.GetBalance());
    }

    [HttpGet("movements")]
    public IActionResult GetMovements()
    {
        return Ok(_cashRepository.LoadMovements()); // <-- Cambiado a LoadMovements()
    }
}