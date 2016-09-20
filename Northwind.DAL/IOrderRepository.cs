using System.Collections.Generic;

namespace Northwind.DAL
{
  public interface IOrderRepository
  {
    IEnumerable<Order> GetOrders();

    Order GetOrderById(int id);

    Order CreateOrder(Order order);
  }
}