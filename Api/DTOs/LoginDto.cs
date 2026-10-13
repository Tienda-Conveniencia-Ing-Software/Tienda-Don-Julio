namespace Api.DTOs;

using System.ComponentModel.DataAnnotations;

public class LoginDto
{
    [Required(ErrorMessage = "El correo es obligatorio.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string Password { get; set; } = string.Empty;
}
