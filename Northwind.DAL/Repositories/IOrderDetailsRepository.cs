using System.Collections.Generic;
using Northwind.DAL.Models;

namespace Northwind.DAL.Repositories
{
  public interface IOrderDetailsRepository
  {
    IEnumerable<OrderDetail> GetOrderDetailsByOrderId(int id);

    void AddOrderDetails(Order order);

    void UpdateOrderDetails(Order order);

    void DeleteOrderDetails(Order order);
  }
}