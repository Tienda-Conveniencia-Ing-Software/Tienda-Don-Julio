using Microsoft.EntityFrameworkCore;
using TiendaBarrio.Core.Models;
using TiendaBarrio.Data;
using TiendaBarrio.Persistence.Interfaces;

namespace TiendaBarrio.Persistence;

public class CashRepository : ICashRepository
{
    private readonly TiendaDbContext _context;

    // Constructor sin parámetros (para compatibilidad con UI Consola y Servicios)
    public CashRepository()
    {
        _context = new TiendaDbContext();
    }

    // Constructor para Inyección de Dependencias
    public CashRepository(TiendaDbContext context)
    {
        _context = context;
    }

    public List<CashMovement> LoadMovements()
    {
        return _context.Movimientos.AsNoTracking().ToList();
    }

    public void SaveMovement(CashMovement movement)
    {
        _context.Movimientos.Add(movement);
        _context.SaveChanges();
    }

    public List<CashMovement> GetMovements()
    {
        return LoadMovements();
    }

    public void AddMovement(CashMovement movement)
    {
        SaveMovement(movement);
    }
}