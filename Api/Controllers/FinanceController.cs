using Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Core.Services;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Trabajador")]
public class FinanceController : ControllerBase
{
    private readonly FinanceService _financeService;

    public FinanceController(FinanceService financeService)
    {
        _financeService = financeService;
    }

    /// <summary>Ingresos, gastos y saldo actual de caja.</summary>
    [HttpGet("balance")]
    [ProducesResponseType(typeof(BalanceResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetBalance()
    {
        return Ok(BalanceResponseDto.From(_financeService.GetBalance()));
    }

    /// <summary>Historial de movimientos de caja.</summary>
    [HttpGet("movements")]
    [ProducesResponseType(typeof(IEnumerable<MovementResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetMovements()
    {
        return Ok(_financeService.GetHistory().Select(MovementResponseDto.From));
    }

    /// <summary>Registra un ingreso o gasto manual.</summary>
    [HttpPost("movements")]
    [ProducesResponseType(typeof(MovementResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult RegisterMovement([FromBody] MovementDto dto)
    {
        CashMovement movement;
        switch (dto.Type)
        {
            case MovementType.Ingreso:
                movement = _financeService.RegisterIncome(dto.Amount, dto.Description);
                break;
            case MovementType.Gasto:
                movement = _financeService.RegisterExpense(dto.Amount, dto.Description);
                break;
            default:
                return BadRequest(new MessageDto("Solo se pueden registrar manualmente movimientos de tipo Ingreso o Gasto."));
        }

        return CreatedAtAction(nameof(GetMovements), null, MovementResponseDto.From(movement));
    }
}
