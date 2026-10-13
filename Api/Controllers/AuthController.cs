using Api.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaBarrio.Core.Services;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly JwtTokenService _tokenService;

    public AuthController(AuthService authService, JwtTokenService tokenService)
    {
        _authService = authService;
        _tokenService = tokenService;
    }

    /// <summary>Valida credenciales y devuelve un token JWT.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        var cliente = _authService.Login(dto.Email, dto.Password);
        if (cliente == null)
        {
            return Unauthorized(new MessageDto("Credenciales inválidas."));
        }

        var (token, expiresAt) = _tokenService.CreateToken(cliente);
        return Ok(new LoginResponseDto(token, expiresAt, cliente.Id, cliente.Nombre, cliente.Email, cliente.Role));
    }
}
