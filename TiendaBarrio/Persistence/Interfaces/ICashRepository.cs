namespace TiendaBarrio.Persistence.Interfaces;

using System.Collections.Generic;
using TiendaBarrio.Core.Models;

public interface ICashRepository
{
    List<CashMovement> LoadMovements();
    void SaveMovement(CashMovement movement);
}