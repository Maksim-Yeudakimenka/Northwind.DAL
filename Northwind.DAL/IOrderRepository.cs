﻿using System.Collections.Generic;

namespace Northwind.DAL
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