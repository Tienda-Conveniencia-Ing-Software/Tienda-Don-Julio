namespace TiendaBarrio.Core.Services;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TiendaBarrio.Core.DTOs;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Persistence.Data;

public class AuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Inicia sesión verificando correo y hash de contraseña.
    /// </summary>
    public async Task<Cliente?> LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return null;

        var normalizedEmail = email.Trim().ToLower();

        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.Email.ToLower() == normalizedEmail);

        if (cliente == null)
            return null;

        bool isValid = false;
        try
        {
            isValid = BCrypt.Net.BCrypt.Verify(password, cliente.PasswordHash);
        }
        catch
        {
            isValid = false;
        }

        return isValid ? cliente : null;
    }

    /// <summary>
    /// Registra un nuevo cliente con contraseña hasheada (Requerido para Sprint 4 / Persona 3).
    /// </summary>
    public async Task<Cliente> RegisterAsync(RegisterClientDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ArgumentException("El correo electrónico es obligatorio.");

        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("La contraseña es obligatoria.");

        var normalizedEmail = dto.Email.Trim().ToLower();

        var exists = await _context.Clientes.AnyAsync(c => c.Email.ToLower() == normalizedEmail);
        if (exists)
            throw new InvalidOperationException("El correo electrónico ya se encuentra registrado.");

        // Validar cédula única (si la proporcionaron)
        if (!string.IsNullOrWhiteSpace(dto.Cedula) && dto.Cedula.Trim() != "00000000")
        {
            var cedulaExists = await _context.Clientes.AnyAsync(c => c.Cedula == dto.Cedula.Trim());
            if (cedulaExists)
                throw new InvalidOperationException("La cédula ya se encuentra registrada.");
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var nuevoCliente = new Cliente(
            id: 0,
            nombre: dto.Nombre,
            cedula: string.IsNullOrWhiteSpace(dto.Cedula) ? "00000000" : dto.Cedula.Trim(),
            telefono: string.IsNullOrWhiteSpace(dto.Telefono) ? "0000000000" : dto.Telefono.Trim(),
            email: normalizedEmail,
            passwordHash: passwordHash,
            role: "Cliente"
        );

        _context.Clientes.Add(nuevoCliente);
        await _context.SaveChangesAsync();

        return nuevoCliente;
    }
}