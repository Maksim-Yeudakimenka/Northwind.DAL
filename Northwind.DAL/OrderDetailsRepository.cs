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
  }
}