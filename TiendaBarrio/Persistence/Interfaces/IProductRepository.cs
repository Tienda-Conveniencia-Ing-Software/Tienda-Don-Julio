using System;
using System.Collections.Generic;
using System.Text;

namespace TiendaBarrio.Persistence.Interfaces;

using TiendaBarrio.Core.Models;

public interface IOrderRepository
{
    void SaveOrder(Order order);
    List<string> LoadOrderLines();
}