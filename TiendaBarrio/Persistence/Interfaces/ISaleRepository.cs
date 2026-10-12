namespace TiendaBarrio.Persistence.Interfaces;

using System.Collections.Generic;
using TiendaBarrio.Core.Models;

public interface ISaleRepository
{
    List<Sale> LoadSales();
    void SaveSale(Sale sale);
    int GetNextSaleId();
}
