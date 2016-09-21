using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Northwind.DAL
{
  public class OrderRepository : IOrderRepository
  {
    private readonly DbProviderFactory _providerFactory;
    private readonly string _connectionString;

    private readonly IOrderDetailsRepository _orderDetailsRepository;
    private readonly IProductRepository _productRepository;

    public OrderRepository(string provider, string connectionString)
    {
      _providerFactory = DbProviderFactories.GetFactory(provider);
      _connectionString = connectionString;
      _orderDetailsRepository = new OrderDetailsRepository(provider, connectionString);
      _productRepository = new ProductRepository(provider, connectionString);
    }

    public IEnumerable<Order> GetOrders()
    {
      const string commandText =
        "SELECT OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM dbo.Orders";

      using (var connection = _providerFactory.CreateConnection())
      {
        connection.ConnectionString = _connectionString;
        connection.Open();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = commandText;
          command.CommandType = CommandType.Text;

          using (var reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              var order = new Order
              {
                OrderId = (int) reader["OrderID"],
                CustomerId = reader["CustomerID"] == DBNull.Value ? null : (string) reader["CustomerID"],
                EmployeeId = reader["EmployeeID"] == DBNull.Value ? null : (int?) reader["EmployeeID"],
                OrderDate = reader["OrderDate"] == DBNull.Value ? null : (DateTime?) reader["OrderDate"],
                RequiredDate = reader["RequiredDate"] == DBNull.Value ? null : (DateTime?) reader["RequiredDate"],
                ShippedDate = reader["ShippedDate"] == DBNull.Value ? null : (DateTime?) reader["ShippedDate"],
                ShipVia = reader["ShipVia"] == DBNull.Value ? null : (int?) reader["ShipVia"],
                Freight = reader["Freight"] == DBNull.Value ? null : (decimal?) reader["Freight"],
                ShipName = reader["ShipName"] == DBNull.Value ? null : (string) reader["ShipName"],
                ShipAddress = reader["ShipAddress"] == DBNull.Value ? null : (string) reader["ShipAddress"],
                ShipCity = reader["ShipCity"] == DBNull.Value ? null : (string) reader["ShipCity"],
                ShipRegion = reader["ShipRegion"] == DBNull.Value ? null : (string) reader["ShipRegion"],
                ShipPostalCode = reader["ShipPostalCode"] == DBNull.Value ? null : (string) reader["ShipPostalCode"],
                ShipCountry = reader["ShipCountry"] == DBNull.Value ? null : (string) reader["ShipCountry"],
                Status =
                  reader["OrderDate"] == DBNull.Value
                    ? OrderStatus.New
                    : reader["ShippedDate"] == DBNull.Value
                      ? OrderStatus.Ordered
                      : OrderStatus.Shipped
              };

              yield return order;
            }
          }
        }
      }
    }

    public Order GetOrderById(int id)
    {
      const string commandText =
        "SELECT OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry FROM dbo.Orders WHERE OrderID = @id";

      using (var connection = _providerFactory.CreateConnection())
      {
        connection.ConnectionString = _connectionString;
        connection.Open();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = commandText;
          command.CommandType = CommandType.Text;

          var paramId = command.CreateParameter();
          paramId.ParameterName = "@id";
          paramId.Value = id;
          command.Parameters.Add(paramId);

          using (var reader = command.ExecuteReader())
          {
            if (!reader.HasRows)
            {
              throw new ArgumentException("No Order with provided id was found", nameof(id));
            }

            reader.Read();

            var order = new Order
            {
              OrderId = (int) reader["OrderID"],
              CustomerId = reader["CustomerID"] == DBNull.Value ? null : (string) reader["CustomerID"],
              EmployeeId = reader["EmployeeID"] == DBNull.Value ? null : (int?) reader["EmployeeID"],
              OrderDate = reader["OrderDate"] == DBNull.Value ? null : (DateTime?) reader["OrderDate"],
              RequiredDate = reader["RequiredDate"] == DBNull.Value ? null : (DateTime?) reader["RequiredDate"],
              ShippedDate = reader["ShippedDate"] == DBNull.Value ? null : (DateTime?) reader["ShippedDate"],
              ShipVia = reader["ShipVia"] == DBNull.Value ? null : (int?) reader["ShipVia"],
              Freight = reader["Freight"] == DBNull.Value ? null : (decimal?) reader["Freight"],
              ShipName = reader["ShipName"] == DBNull.Value ? null : (string) reader["ShipName"],
              ShipAddress = reader["ShipAddress"] == DBNull.Value ? null : (string) reader["ShipAddress"],
              ShipCity = reader["ShipCity"] == DBNull.Value ? null : (string) reader["ShipCity"],
              ShipRegion = reader["ShipRegion"] == DBNull.Value ? null : (string) reader["ShipRegion"],
              ShipPostalCode = reader["ShipPostalCode"] == DBNull.Value ? null : (string) reader["ShipPostalCode"],
              ShipCountry = reader["ShipCountry"] == DBNull.Value ? null : (string) reader["ShipCountry"],
              Status =
                reader["OrderDate"] == DBNull.Value
                  ? OrderStatus.New
                  : reader["ShippedDate"] == DBNull.Value
                    ? OrderStatus.Ordered
                    : OrderStatus.Shipped
            };

            foreach (var orderDetail in _orderDetailsRepository.GetOrderDetailsByOrderId(id))
            {
              orderDetail.Order = order;
              orderDetail.Product = _productRepository.GetProductById(orderDetail.Product.ProductId);
              order.OrderDetails.Add(orderDetail);
            }

            return order;
          }
        }
      }
    }

    public Order CreateOrder(Order order)
    {
      const string commandText =
        "INSERT INTO dbo.Orders (CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry) " +
        "VALUES (@CustomerID, @EmployeeID, @OrderDate, @RequiredDate, @ShippedDate, @ShipVia, @Freight, @ShipName, @ShipAddress, @ShipCity, @ShipRegion, @ShipPostalCode, @ShipCountry); " +
        "SELECT SCOPE_IDENTITY() AS OrderID";

      using (var connection = _providerFactory.CreateConnection())
      {
        connection.ConnectionString = _connectionString;
        connection.Open();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = commandText;
          command.CommandType = CommandType.Text;

          command.AddParameter("@CustomerID", order.CustomerId);
          command.AddParameter("@EmployeeID", order.EmployeeId);
          command.AddParameter("@OrderDate", DBNull.Value);
          command.AddParameter("@RequiredDate", order.RequiredDate);
          command.AddParameter("@ShippedDate", DBNull.Value);
          command.AddParameter("@ShipVia", order.ShipVia);
          command.AddParameter("@Freight", order.Freight);
          command.AddParameter("@ShipName", order.ShipName);
          command.AddParameter("@ShipAddress", order.ShipAddress);
          command.AddParameter("@ShipCity", order.ShipCity);
          command.AddParameter("@ShipRegion", order.ShipRegion);
          command.AddParameter("@ShipPostalCode", order.ShipPostalCode);
          command.AddParameter("@ShipCountry", order.ShipCountry);

          order.OrderId = Convert.ToInt32(command.ExecuteScalar());

          _orderDetailsRepository.AddOrderDetails(order);

          return GetOrderById(order.OrderId);
        }
      }
    }

    public Order UpdateOrder(Order order)
    {
      const string commandText =
        "UPDATE dbo.Orders SET CustomerID = @CustomerID, EmployeeID = @EmployeeID, RequiredDate = @RequiredDate, ShipVia = @ShipVia, Freight = @Freight, ShipName = @ShipName, ShipAddress = @ShipAddress, ShipCity = @ShipCity, ShipRegion = @ShipRegion, ShipPostalCode = @ShipPostalCode, ShipCountry = @ShipCountry WHERE OrderID = @id";

      if (order.Status == OrderStatus.Ordered || order.Status == OrderStatus.Shipped)
      {
        throw new ArgumentException("Can't update ordered or shipped order", nameof(order.Status));
      }

      if (order.OrderDate != null)
      {
        throw new ArgumentException("Can't change order date of already ordered order", nameof(order.OrderDate));
      }

      if (order.ShippedDate != null)
      {
        throw new ArgumentException("Can't change shipped date of already shipped order", nameof(order.ShippedDate));
      }

      using (var connection = _providerFactory.CreateConnection())
      {
        connection.ConnectionString = _connectionString;
        connection.Open();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = commandText;
          command.CommandType = CommandType.Text;

          command.AddParameter("@id", order.OrderId);
          command.AddParameter("@CustomerID", order.CustomerId);
          command.AddParameter("@EmployeeID", order.EmployeeId);
          command.AddParameter("@RequiredDate", order.RequiredDate);
          command.AddParameter("@ShipVia", order.ShipVia);
          command.AddParameter("@Freight", order.Freight);
          command.AddParameter("@ShipName", order.ShipName);
          command.AddParameter("@ShipAddress", order.ShipAddress);
          command.AddParameter("@ShipCity", order.ShipCity);
          command.AddParameter("@ShipRegion", order.ShipRegion);
          command.AddParameter("@ShipPostalCode", order.ShipPostalCode);
          command.AddParameter("@ShipCountry", order.ShipCountry);

          command.ExecuteNonQuery();

          _orderDetailsRepository.UpdateOrderDetails(order);

          return GetOrderById(order.OrderId);
        }
      }
    }

    public void DeleteOrder(Order order)
    {
      const string commandText =
        "DELETE FROM dbo.Orders WHERE OrderID = @id";

      if (order.Status == OrderStatus.Shipped)
      {
        throw new ArgumentException("Can't delete shipped order", nameof(order.Status));
      }

      _orderDetailsRepository.DeleteOrderDetails(order);

      using (var connection = _providerFactory.CreateConnection())
      {
        connection.ConnectionString = _connectionString;
        connection.Open();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = commandText;
          command.CommandType = CommandType.Text;

          command.AddParameter("@id", order.OrderId);

          command.ExecuteNonQuery();
        }
      }
    }
  }
}