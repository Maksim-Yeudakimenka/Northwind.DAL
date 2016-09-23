using System;
using System.Data.Common;

namespace Northwind.DAL.Helpers
{
  public static class DbCommandExtensions
  {
    public static DbCommand AddParameter(this DbCommand command, string parameterName, object value)
    {
      var parameter = command.CreateParameter();
      parameter.ParameterName = parameterName;
      parameter.Value = value ?? DBNull.Value;

      command.Parameters.Add(parameter);

      return command;
    }
  }
}