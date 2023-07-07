using System.Data;
using System.Text;
using PetProject.OrderManagement.CrossCuttingConcerns.Extensions;
using PetProject.OrderManagement.Infrastructure.SqlServer.Extensions;

namespace PetProject.OrderManagement.Infrastructure.SqlServer.BulkAction_V1.BulkUpdate
{
    public class BulkUpdateBuilder<T> : BulkBase<T>
    {
        #region Constructor

        public BulkUpdateBuilder(IDbConnection connection)
        {
            _connection = connection;
        }

        public BulkUpdateBuilder(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        public BulkUpdateBuilder(IDbConnection connection, IDbTransaction transaction = null)
        {
            _connection = connection;
            _transaction = transaction;
        }

        #endregion

        public override void Excute()
        {
            var tempTableName = "#" + Guid.NewGuid();

            var existedColumns = new List<string>();
            var dataTableColumns = new List<string>();
            dataTableColumns.AddRange(_columnNames);
            dataTableColumns.AddRange(_idColumns);

            var dataTable = _data.ToDataTable(dataTableColumns);
            var sqlQueryCreatingTempTable = dataTable.ToSqlQueryCreatingTable(tempTableName);
            var sqlQueryGetColumnsOfCurrentTable = GenerateSqlQueryGetColumnsOfCurrentTable();

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

            using (var getColumnsOfCurrentTable = _connection.CreateTextCommand(_transaction, sqlQueryGetColumnsOfCurrentTable))
            {
                using (var reader = getColumnsOfCurrentTable.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        existedColumns.Add(reader[Constants.Constants.Sql_GetColumnName].ToString());
                    }
                }
            }

            var sqlQueryUpdatingExitsedTable = GenerateSqlQueryUpdatingExistedTable(tempTableName, existedColumns);

            using (var updateExistedTableCommand = _connection.CreateTextCommand(_transaction, sqlQueryUpdatingExitsedTable))
            {
                updateExistedTableCommand.ExecuteNonQuery();
            }
        }

        private string GenerateSqlQueryGetColumnsOfCurrentTable()
        {
            var sqlQuery = new StringBuilder();
            sqlQuery.AppendLine("SELECT COLUMN_NAME");
            sqlQuery.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
            sqlQuery.AppendLine($"WHERE TABLE_NAME = N'[{_tableNamePrefix}][{_tableName}]'");

            return sqlQuery.ToString();
        }

        private string GenerateSqlQueryUpdatingExistedTable(string tempTableName, IEnumerable<string> columns)
        {
            var sqlQuery = new StringBuilder();
            var existedIdColumns = _idColumns.Where(x => columns.Contains(x));
            var existedColumnNames = _columnNames.Where(x => columns.Contains(x));

            // Generate join conditions
            var joinCondition = string.Join(" and ", existedIdColumns.Select(x =>
            {
                var columnName = GetDbColumnName(x);

                return $"a.[{columnName}] = b.[{x}]";
            }));

            // Generate set statements
            var setStatement = string.Join(", ", existedColumnNames.Select(x =>
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