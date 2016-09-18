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

    public OrderRepository(string provider, string connectionString)
    {
      _providerFactory = DbProviderFactories.GetFactory(provider);
      _connectionString = connectionString;
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
  }
}