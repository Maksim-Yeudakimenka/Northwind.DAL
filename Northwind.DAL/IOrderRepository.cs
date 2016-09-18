using System.Collections.Generic;

namespace Northwind.DAL
{
  public interface IOrderRepository
  {
    IEnumerable<Order> GetOrders();
  }
}