using System.Data;
using System.Text;

namespace PetProject.OrderManagement.Persistence.SqlServer.Extensions
{
    public static class DataTableExtensions
    {
        public static string ToSqlQueryCreatingTable(this DataTable dataTable, string sqlTableName)
        {
            var sqlQuery = new StringBuilder();

            sqlQuery.AppendLine($"CREATE TABLE [{sqlTableName}] (");

            foreach (DataColumn column in dataTable.Columns)
            {
                var propName = column.ColumnName;
                var propType = column.DataType.ToSqlType();

                sqlQuery.AppendLine($"\n\t[{propName}] {propType} NULL,");
            }

            sqlQuery.AppendLine("\n);");

            return sqlQuery.ToString();
        }
    }
}
