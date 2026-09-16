namespace TiendaBarrio.Persistence;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Core.Config;

public class ClienteRepository
{
    // Ruta centralizada en DataConfig
    private string RutaClientes => DataConfig.ClientesFile;

    public List<Cliente> LoadClientes()
    {
        var clientes = new List<Cliente>();

        // Asegura que la carpeta Data exista
        DataConfig.EnsureDataFolderExists();

        if (!File.Exists(RutaClientes))
            return clientes;

        foreach (string line in File.ReadLines(RutaClientes))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Formato: Id;Nombre;Cedula;Telefono;Email
            string[] parts = line.Split(';');
            if (parts.Length < 5)
                continue;

            if (!int.TryParse(parts[0], out int id))
                continue;

            string nombre = parts[1];
            string cedula = parts[2];
            string telefono = parts[3];
            string email = parts[4];

            try
            {
                var c = new Cliente(id, nombre, cedula, telefono, email);
                clientes.Add(c);
            }
            catch (ArgumentException)
            {
                // Si el cliente tiene datos inválidos, se ignora
                continue;
            }
        }

        return clientes;
    }

    public void SaveClientes(List<Cliente> clientes)
    {
        // Asegura que la carpeta Data exista
        DataConfig.EnsureDataFolderExists();

        string[] lines = clientes
            .Select(c => $"{c.Id};{c.Nombre};{c.Cedula};{c.Telefono};{c.Email}")
            .ToArray();

        File.WriteAllLines(RutaClientes, lines);
    }

    public void AddCliente(Cliente cliente)
    {
        var clientes = LoadClientes();
        clientes.Add(cliente);
        SaveClientes(clientes);
    }

    public void UpdateCliente(Cliente cliente)
    {
        var clientes = LoadClientes();
        var index = clientes.FindIndex(c => c.Id == cliente.Id);

        if (index >= 0)
        {
            clientes[index] = cliente;
            SaveClientes(clientes);
        }
        else
        {
            Console.WriteLine($"No se encontró el cliente con ID {cliente.Id}.");
        }
    }

    public void DeleteCliente(int id)
    {
        var clientes = LoadClientes();
        var cliente = clientes.FirstOrDefault(c => c.Id == id);

        if (cliente != null)
        {
            clientes.Remove(cliente);
            SaveClientes(clientes);
        }
        else
        {
            Console.WriteLine($"No se encontró el cliente con ID {id}.");
        }
    }

    public Cliente GetClienteById(int id)
    {
        return LoadClientes().FirstOrDefault(c => c.Id == id);
    }

    public int GetNextClienteId()
    {
        var clientes = LoadClientes();
        return clientes.Count > 0 ? clientes.Max(c => c.Id) + 1 : 1;
    }
}
