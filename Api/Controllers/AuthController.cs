using Microsoft.AspNetCore.Mvc;
using TiendaBarrio.Core.Services;
using Api.DTOs;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest(new { message = "El correo y la contraseña son obligatorios." });
        }

        try
        {
            var cliente = _authService.Login(dto.Email, dto.Password);
            if (cliente == null)
            {
                return Unauthorized(new { message = "Credenciales inválidas." });
            }
            return Ok(cliente);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}