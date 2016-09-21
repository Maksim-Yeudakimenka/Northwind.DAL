using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Northwind.DAL
{
  public class OrderDetailsRepository : IOrderDetailsRepository
  {
    private readonly DbProviderFactory _providerFactory;
    private readonly string _connectionString;

    public OrderDetailsRepository(string provider, string connectionString)
    {
      _providerFactory = DbProviderFactories.GetFactory(provider);
      _connectionString = connectionString;
    }

    public IEnumerable<OrderDetail> GetOrderDetailsByOrderId(int id)
    {
      const string commandText =
        "SELECT OrderID, ProductID, UnitPrice, Quantity, Discount FROM dbo.[Order Details] WHERE OrderID = @id";

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
              throw new ArgumentException("No Order Details with provided id was found", nameof(id));
            }

            while (reader.Read())
            {
              var orderDetail = new OrderDetail
              {
                Order = new Order { OrderId = (int) reader["OrderID"] },
                Product = new Product { ProductId = (int) reader["ProductID"] },
                UnitPrice = (decimal) reader["UnitPrice"],
                Quantity = (short) reader["Quantity"],
                Discount = (float) reader["Discount"]
              };

              yield return orderDetail;
            }
          }
        }
      }
    }

    public void AddOrderDetails(Order order)
    {
      const string commandText =
        "INSERT INTO dbo.[Order Details] (OrderID, ProductID, UnitPrice, Quantity, Discount) " +
        "VALUES (@OrderID, @ProductID, @UnitPrice, @Quantity, @Discount)";

      using (var connection = _providerFactory.CreateConnection())
      {
        connection.ConnectionString = _connectionString;
        connection.Open();

        foreach (var orderDetail in order.OrderDetails)
        {
          using (var command = connection.CreateCommand())
          {
            command.CommandText = commandText;
            command.CommandType = CommandType.Text;

            command.AddParameter("@OrderID", order.OrderId);
            command.AddParameter("@ProductID", orderDetail.Product.ProductId);
            command.AddParameter("@UnitPrice", orderDetail.UnitPrice);
            command.AddParameter("@Quantity", orderDetail.Quantity);
            command.AddParameter("@Discount", orderDetail.Discount);

            command.ExecuteNonQuery();
          }
        }
      }
    }

    public void UpdateOrderDetails(Order order)
    {
      DeleteOrderDetails(order);
      AddOrderDetails(order);
    }

    public void DeleteOrderDetails(Order order)
    {
      const string commandText =
        "DELETE FROM dbo.[Order Details] WHERE OrderID = @id";

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