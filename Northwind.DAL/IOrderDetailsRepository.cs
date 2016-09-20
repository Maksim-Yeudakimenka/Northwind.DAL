using System.Collections.Generic;

namespace Northwind.DAL
{
  public interface IOrderDetailsRepository
  {
    IEnumerable<OrderDetail> GetOrderDetailsByOrderId(int id);

    void AddOrderDetailToOrder(OrderDetail orderDetail);
  }
}