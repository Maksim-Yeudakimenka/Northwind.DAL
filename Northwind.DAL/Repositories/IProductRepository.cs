using Northwind.DAL.Models;

namespace Northwind.DAL.Repositories
{
  public interface IProductRepository
  {
    Product GetProductById(int id);
  }
}