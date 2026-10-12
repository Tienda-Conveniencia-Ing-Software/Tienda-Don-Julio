using Microsoft.EntityFrameworkCore;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Data;
using TiendaBarrio.Persistence.Interfaces;

namespace TiendaBarrio.Persistence;

public class ClienteRepository : IClienteRepository
{
    private readonly TiendaDbContext _context;

    // Constructor sin parámetros (para compatibilidad con UI Consola y Servicios)
    public ClienteRepository()
    {
        _context = new TiendaDbContext();
    }

    // Constructor para Inyección de Dependencias
    public ClienteRepository(TiendaDbContext context)
    {
        _context = context;
    }

    public List<Cliente> LoadClientes()
    {
        return _context.Clientes.AsNoTracking().ToList();
    }

    public void SaveClientes(List<Cliente> clientes)
    {
        foreach (var cliente in clientes)
        {
            AddCliente(cliente);
        }
    }

    public Cliente? GetByEmail(string email)
    {
        return _context.Clientes
            .AsNoTracking()
            .FirstOrDefault(c => c.Email.ToLower() == email.ToLower());
    }

    public Cliente? GetByCedula(string cedula)
    {
        return _context.Clientes
            .AsNoTracking()
            .FirstOrDefault(c => c.Cedula == cedula);
    }

    public void AddCliente(Cliente cliente)
    {
        var existing = _context.Clientes.FirstOrDefault(c => c.Id == cliente.Id);
        if (existing != null)
        {
            _context.Entry(existing).CurrentValues.SetValues(cliente);
        }
        else
        {
            _context.Clientes.Add(cliente);
        }
        _context.SaveChanges();
    }

    public void SaveCliente(Cliente cliente)
    {
        AddCliente(cliente);
    }
}