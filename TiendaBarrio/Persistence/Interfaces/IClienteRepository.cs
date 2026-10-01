namespace TiendaBarrio.Persistence.Interfaces;

using System.Collections.Generic;
using TiendaBarrio.Core.Models;

public interface IClienteRepository
{
    List<Cliente> LoadClientes();
    void SaveClientes(List<Cliente> clientes);
    Cliente? GetByEmail(string email);
    Cliente? GetByCedula(string cedula);
    void AddCliente(Cliente cliente);
}