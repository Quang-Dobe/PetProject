using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using PetProject.OrderManagement.CrossCuttingConcerns.Extensions;
using PetProject.OrderManagement.Infrastructure.SqlServer.Extensions;

namespace PetProject.OrderManagement.Infrastructure.SqlServer.BulkAction_V1.BulkDelete
{
    public class BulkDeleteBuilder<T> : BulkBase<T>
    {
        #region Constructor

        public BulkDeleteBuilder(SqlConnection connection)
        {
            _connection = connection;
        }

        public BulkDeleteBuilder(SqlTransaction transaction)
        {
            _transaction = transaction;
        }

        public BulkDeleteBuilder(SqlConnection connection, SqlTransaction transaction = null)
        {
            _connection = connection;
            _transaction = transaction;
        }

        #endregion

        public void Excute()
        {
            var tempTableName = "#" + Guid.NewGuid();

            var dataTableColumns = new List<string>();
            dataTableColumns.AddRange(_idColumns);
            dataTableColumns.AddRange(_columnNames);

            var dataTable = _data.ToDataTable(dataTableColumns);
            var sqlQueryCreatingTempTable = dataTable.ToSqlQueryCreatingTable(tempTableName);
            var sqlQueryDeletingDataFromTable = GenerateSqlQueryDeletingDataFromTable(tempTableName);

            _connection.EnsureOpen();

            using (var createTempTableCommand = _connection.CreateTextCommand(_transaction, sqlQueryCreatingTempTable))
            {
                createTempTableCommand.ExecuteNonQuery();
            }

            GenerateDataForTempTable(dataTable, tempTableName, _dbColumnMappings, _connection, _transaction, _options);

            using (var deleteDataFromTable = _connection.CreateTextCommand(_transaction, sqlQueryDeletingDataFromTable))
            {
                deleteDataFromTable.ExecuteNonQuery();
            }
        }

        private string GenerateSqlQueryDeletingDataFromTable(string tempTableName)
        {
            var sqlQuery = new StringBuilder();

            // Generate join conditions
            var joinCondition = string.Join(" and ", _idColumns.Select(x => 
            {
                var columnName = GetDbColumnName(x);

                return $"a.[{columnName}] = b.[{columnName}]";
            }));

            sqlQuery.AppendLine("DELETE a");
            sqlQuery.AppendLine($"FROM [{_tableNamePrefix}][{_tableName}] a JOIN {tempTableName} b ON " + joinCondition);

            return sqlQuery.ToString();
        }
    }
}