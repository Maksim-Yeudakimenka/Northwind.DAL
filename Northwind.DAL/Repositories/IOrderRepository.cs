using System.Collections.Generic;
using Northwind.DAL.Models;

namespace Northwind.DAL.Repositories
{
  public interface IOrderRepository
  {
    IEnumerable<Order> GetOrders();

    Order GetOrderById(int id);

    Order CreateOrder(Order order);

    Order UpdateOrder(Order order);

    void DeleteOrder(Order order);

    Order MarkOrdered(Order order);

    Order MarkShipped(Order order);

    IEnumerable<CustomerProductTotal> GetCustomerOrdersHistory(string customerId);

    IEnumerable<CustomerOrderDetail> GetCustomerOrderDetails(int orderId);
  }
}