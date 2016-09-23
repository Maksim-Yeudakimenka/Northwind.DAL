using System;
using System.Configuration;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Northwind.DAL.Models;
using Northwind.DAL.Repositories;

namespace Northwind.DAL.Tests
{
  [TestClass]
  public class OrderRepositoryTests
  {
    private static OrderRepository _orderRepository;

    [ClassInitialize]
    public static void Initialize(TestContext context)
    {
      var connectionStringItem = ConfigurationManager.ConnectionStrings["NorthwindConnection"];
      var providerName = connectionStringItem.ProviderName;
      var connectionString = connectionStringItem.ConnectionString;

      _orderRepository = new OrderRepository(providerName, connectionString);
    }

    [TestMethod]
    public void GetOrders_ShouldSetCorrectOrderStatus()
    {
      var orders = _orderRepository.GetOrders();

      foreach (var order in orders)
      {
        if (order.OrderDate == null)
        {
          Assert.AreEqual(OrderStatus.New, order.Status);
        }
        else if (order.ShippedDate == null)
        {
          Assert.AreEqual(OrderStatus.Ordered, order.Status);
        }
        else
        {
          Assert.AreEqual(OrderStatus.Shipped, order.Status);
        }
      }
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetOrderById_WithNotExistingId_ShouldThrow()
    {
      var orderId = -1;

      _orderRepository.GetOrderById(orderId);
    }

    [TestMethod]
    public void GetOrderById_WithExistingId_ShouldGetOrderDetails()
    {
      var orderId = 10383;

      var order = _orderRepository.GetOrderById(orderId);

      Assert.IsNotNull(order.OrderDetails);

      foreach (var orderDetail in order.OrderDetails)
      {
        Assert.IsNotNull(orderDetail.Product);
        Assert.IsNotNull(orderDetail.Product.ProductId);
        Assert.IsNotNull(orderDetail.Product.ProductName);
      }
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdateOrder_WithOrderedStatus_ShouldThrow()
    {
      var order = _orderRepository.GetOrders().First(o => o.Status == OrderStatus.Ordered);

      _orderRepository.UpdateOrder(order);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdateOrder_WithShippedStatus_ShouldThrow()
    {
      var order = _orderRepository.GetOrders().First(o => o.Status == OrderStatus.Shipped);

      _orderRepository.UpdateOrder(order);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdateOrder_WithOrderDateChanged_ShouldThrow()
    {
      var order = _orderRepository.GetOrders().First(o => o.Status == OrderStatus.New);

      order.OrderDate = DateTime.UtcNow;

      _orderRepository.UpdateOrder(order);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdateOrder_WithShippedDateChanged_ShouldThrow()
    {
      var order = _orderRepository.GetOrders().First(o => o.Status == OrderStatus.New);

      order.ShippedDate = DateTime.UtcNow;

      _orderRepository.UpdateOrder(order);
    }

    [TestMethod]
    public void UpdateOrder_WithNewStatus_ShouldThrow()
    {
      var order = _orderRepository.GetOrders().First(o => o.Status == OrderStatus.New);

      order.RequiredDate = DateTime.Today;
      order.ShipVia = 1;
      order.Freight = 81.57M;

      _orderRepository.UpdateOrder(order);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void DeleteOrder_WithShippedStatus_ShouldThrow()
    {
      var order = _orderRepository.GetOrders().First(o => o.Status == OrderStatus.Shipped);

      _orderRepository.DeleteOrder(order);
    }

    [TestMethod]
    public void DeleteOrder_WithOrderedStatus_ShouldWork()
    {
      var order = _orderRepository.GetOrders().First(o => o.Status == OrderStatus.Ordered);

      _orderRepository.DeleteOrder(order);
    }

    [TestMethod]
    public void MarkOrdered_ShouldSetOrderedStatus()
    {
      var order = _orderRepository.GetOrders().First(o => o.Status == OrderStatus.New);

      var updatedOrder = _orderRepository.MarkOrdered(order);

      Assert.AreEqual(OrderStatus.Ordered, updatedOrder.Status);
    }

    [TestMethod]
    public void MarkShipped_ShouldSetShippedStatus()
    {
      var order = _orderRepository.GetOrders().First(o => o.Status == OrderStatus.Ordered);

      var updatedOrder = _orderRepository.MarkShipped(order);

      Assert.AreEqual(OrderStatus.Shipped, updatedOrder.Status);
    }
  }
}