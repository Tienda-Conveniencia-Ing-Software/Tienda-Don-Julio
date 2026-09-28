using System;
using System.Collections.Generic;
using System.Text;
namespace TiendaBarrio.Persistence.Interfaces;

using TiendaBarrio.Core.Models;

public interface IProductRepository
{
    List<Product> LoadProducts();
    void SaveProducts(List<Product> products);
}
