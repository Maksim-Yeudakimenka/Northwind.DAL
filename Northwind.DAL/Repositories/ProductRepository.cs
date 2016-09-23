using System;
using System.Data;
using System.Data.Common;
using Northwind.DAL.Models;

namespace Northwind.DAL.Repositories
{
  public class ProductRepository : IProductRepository
  {
    private readonly DbProviderFactory _providerFactory;
    private readonly string _connectionString;

    public ProductRepository(string provider, string connectionString)
    {
      _providerFactory = DbProviderFactories.GetFactory(provider);
      _connectionString = connectionString;
    }

    public Product GetProductById(int id)
    {
      const string commandText =
        "SELECT ProductID, ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued FROM dbo.Products WHERE ProductID = @id";

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
              throw new ArgumentException("No Product with provided id was found", nameof(id));
            }

            reader.Read();

            var product = new Product
            {
              ProductId = (int) reader["ProductID"],
              ProductName = (string) reader["ProductName"],
              SupplierId = reader["SupplierID"] == DBNull.Value ? null : (int?) reader["SupplierID"],
              CategoryId = reader["CategoryID"] == DBNull.Value ? null : (int?) reader["CategoryID"],
              QuantityPerUnit = reader["QuantityPerUnit"] == DBNull.Value ? null : (string) reader["QuantityPerUnit"],
              UnitPrice = reader["UnitPrice"] == DBNull.Value ? null : (decimal?) reader["UnitPrice"],
              UnitsInStock = reader["UnitsInStock"] == DBNull.Value ? null : (short?) reader["UnitsInStock"],
              UnitsOnOrder = reader["UnitsOnOrder"] == DBNull.Value ? null : (short?) reader["UnitsOnOrder"],
              ReorderLevel = reader["ReorderLevel"] == DBNull.Value ? null : (short?) reader["ReorderLevel"],
              Discontinued = (bool) reader["Discontinued"]
            };

            return product;
          }
        }
      }
    }
  }
}