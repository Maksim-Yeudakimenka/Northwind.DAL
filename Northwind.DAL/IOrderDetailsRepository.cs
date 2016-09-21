using System.Collections.Generic;

namespace Northwind.DAL
{
  public interface IOrderDetailsRepository
  {
    IEnumerable<OrderDetail> GetOrderDetailsByOrderId(int id);

    void AddOrderDetails(Order order);

    void UpdateOrderDetails(Order order);

    void DeleteOrderDetails(Order order);
  }
}