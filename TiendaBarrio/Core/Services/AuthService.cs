namespace TiendaBarrio.Core.Services;

using System;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Persistence.Interfaces;

public class AuthService
{
    private readonly IClienteRepository _clienteRepository;

    public AuthService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository ?? throw new ArgumentNullException(nameof(clienteRepository));
    }

    // Registro de nuevo usuario (por defecto Cliente)
    public Cliente Register(string nombre, string cedula, string telefono, string email, string password, string role = "Cliente")
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            throw new ArgumentException("La contraseña debe tener al menos 6 caracteres.");
        }

        if (_clienteRepository.GetByEmail(email) != null)
        {
            throw new InvalidOperationException("Ya existe un usuario registrado con este correo electrónico.");
        }

        if (_clienteRepository.GetByCedula(cedula) != null)
        {
            throw new InvalidOperationException("Ya existe un usuario registrado con esta cédula.");
        }

        // Hashear la contraseña usando BCrypt
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        int nextId = _clienteRepository.LoadClientes().Count > 0
            ? _clienteRepository.LoadClientes().Max(c => c.Id) + 1
            : 1;

        var nuevoCliente = new Cliente(nextId, nombre, cedula, telefono, email, passwordHash, role);
        _clienteRepository.AddCliente(nuevoCliente);

        return nuevoCliente;
    }

    // Inicio de sesión / Validación de credenciales
    public Cliente Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("El correo y la contraseña son obligatorios.");
        }

        var cliente = _clienteRepository.GetByEmail(email);
        if (cliente == null)
        {
            throw new UnauthorizedAccessException("Credenciales inválidas.");
        }

        // Verificar el hash de la contraseña ingresada contra la guardada
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, cliente.PasswordHash);
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Credenciales inválidas.");
        }

        return cliente;
    }
}