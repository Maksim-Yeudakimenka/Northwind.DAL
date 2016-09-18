namespace Northwind.DAL
{
  public interface IProductRepository
  {
    Product GetProductById(int id);
  }
}