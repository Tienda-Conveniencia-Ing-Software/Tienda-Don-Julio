namespace Api.DTOs;

public record LoginResponseDto(string Token, DateTime ExpiresAt, int Id, string Nombre, string Email, string Role);
