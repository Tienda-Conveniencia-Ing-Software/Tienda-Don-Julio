using TiendaBarrio.Core.Models;
using TiendaBarrio.Persistence.Interfaces;

namespace TiendaBarrio.Core.Services;

public class AuthService
{
    private readonly IClienteRepository _clienteRepository;

    public AuthService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<Cliente?> LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return null;

        var clientes = await _clienteRepository.GetAllAsync();

        // Buscar el usuario ignorando mayúsculas/minúsculas
        var cliente = clientes.FirstOrDefault(c =>
            c.Email.Trim().Equals(email.Trim(), StringComparison.OrdinalIgnoreCase));

        if (cliente == null)
            return null;

        // Verificar hash
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
}