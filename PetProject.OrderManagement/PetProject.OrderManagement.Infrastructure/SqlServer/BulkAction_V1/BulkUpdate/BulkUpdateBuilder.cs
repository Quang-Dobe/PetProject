using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using PetProject.OrderManagement.CrossCuttingConcerns.Extensions;
using PetProject.OrderManagement.Infrastructure.SqlServer.Extensions;

namespace PetProject.OrderManagement.Infrastructure.SqlServer.BulkAction_V1.BulkUpdate
{
    public class BulkUpdateBuilder<T> : BulkBase<T>
    {
        #region Constructor

        public BulkUpdateBuilder(SqlConnection connection)
        {
            _connection = connection;
        }

        public BulkUpdateBuilder(SqlTransaction transaction)
        {
            _transaction = transaction;
        }

        public BulkUpdateBuilder(SqlConnection connection, SqlTransaction transaction = null)
        {
            _connection = connection;
            _transaction = transaction;
        }

        #endregion

        public void Excute()
        {
            var tempTableName = "#" + Guid.NewGuid();

            var dataTableColumns = new List<string>();
            dataTableColumns.AddRange(_columnNames);
            dataTableColumns.AddRange(_idColumns);

            var dataTable = _data.ToDataTable(dataTableColumns);
            var sqlQueryCreatingTempTable = dataTable.ToSqlQueryCreatingTable(tempTableName);
            var sqlQueryUpdatingExitsedTable = GenerateSqlQueryUpdatingExistedTable(tempTableName);

            // WorkFlow:
            // + Open connection to database and Keep connection
            // + Create TempTable
            // + Insert data to TempTable
            // + Update ExistedTable

            _connection.EnsureOpen();
            
            using (var createTempTableCommand = _connection.CreateTextCommand(_transaction, sqlQueryCreatingTempTable))
            {
                createTempTableCommand.ExecuteNonQuery();
            }

            GenerateDataForTempTable(dataTable, tempTableName, _dbColumnMappings, _connection, _transaction, _options);
            
            using (var updateExistedTableCommand = _connection.CreateTextCommand(_transaction, sqlQueryUpdatingExitsedTable))
            {
                updateExistedTableCommand.ExecuteNonQuery();
            }
        }

        private string GenerateSqlQueryUpdatingExistedTable(string tempTableName)
        {
            var sqlQuery = new StringBuilder();

            // Generate join conditions
            var joinCondition = string.Join(" and ", _idColumns.Select(x =>
            {
                var columnName = GetDbColumnName(x);

                return $"a.[{columnName}] = b.[{x}]";
            }));

            // Generate set statements
            var setStatement = string.Join(", ", _columnNames.Select(x =>
            {
                var columnName = GetDbColumnName(x);

                return $"a.[{columnName}] = b.[{columnName}]";
            }));

            sqlQuery.AppendLine("UPDATE a");
            sqlQuery.AppendLine($"SET {setStatement}");
            sqlQuery.AppendLine($"FROM [{_tableNamePrefix}][{_tableName}] a JOIN {tempTableName} b ON " + joinCondition);

            return sqlQuery.ToString();
        }
    }
}