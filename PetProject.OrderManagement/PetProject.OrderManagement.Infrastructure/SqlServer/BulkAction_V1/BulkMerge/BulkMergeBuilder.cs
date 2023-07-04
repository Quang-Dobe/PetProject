using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using PetProject.OrderManagement.CrossCuttingConcerns.Extensions;
using PetProject.OrderManagement.Infrastructure.SqlServer.Extensions;

namespace PetProject.OrderManagement.Infrastructure.SqlServer.BulkAction_V1.BulkMerge
{
    public class BulkMergeBuilder<T> : BulkBase<T>
    {
        #region Constructor

        public BulkMergeBuilder(SqlConnection connection)
        {
            _connection = connection;
        }

        public BulkMergeBuilder(SqlTransaction transaction)
        {
            _transaction = transaction;
        }

        public BulkMergeBuilder(SqlConnection connection, SqlTransaction transaction = null)
        {
            _connection = connection;
            _transaction = transaction;
        }

        #endregion

        public void Excute()
        {
            var tempTableName = "#" + Guid.NewGuid();

            var existedColumns = new List<string>();
            var dataTableColumns = new List<string>();
            dataTableColumns.AddRange(_idColumns);
            dataTableColumns.AddRange(_columnNames);

            var dataTable = _data.ToDataTable(dataTableColumns);
            var sqlQueryCreatingTempTable = dataTable.ToSqlQueryCreatingTable(tempTableName);
            var sqlQueryGetColumnsOfCurrentTable = GenerateSqlQueryGetColumnsOfCurrentTable();

            using(var createTempTable = _connection.CreateTextCommand(_transaction, sqlQueryCreatingTempTable))
            {
                createTempTable.ExecuteNonQuery();
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

            var sqlQueryInsertOrUpdateExistedTable = GenerateSqlQueryInsertOrUpdateExistedTable(tempTableName, existedColumns);
            
            using(var insertOrUpdateExistedTable = _connection.CreateTextCommand(_transaction, sqlQueryInsertOrUpdateExistedTable))
            {
                insertOrUpdateExistedTable.ExecuteNonQuery();
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

        private string GenerateSqlQueryInsertOrUpdateExistedTable(string tempTableName, IEnumerable<string> columns)
        {
            var sqlQuery = new StringBuilder();

            var existedColumns = new List<string>();
            existedColumns.AddRange(_idColumns.Where(x => columns.Contains(x)));
            existedColumns.AddRange(_columnNames.Where(x => columns.Contains(x)));

            // Generate join condition
            var joinCondition = string.Join(" and ", existedColumns.Select(x => 
            {
                var columnName = GetDbColumnName(x);

                return $"a.[{columnName}]=b.[{columnName}]";
            }));

            // Generate insert statement
            var insertStatementFrom = string.Join(", ", existedColumns.Select(x => 
            {
                var columnName = GetDbColumnName(x);

                return $"a.[{columnName}]";
            }));
            var insertedColumnsTo = string.Join(", ", existedColumns.Select(x => GetDbColumnName(x)));

            // Generate update statement
            var updateStatement = string.Join(", ", _columnNames.Select(x =>
            {
                var columnName = GetDbColumnName(x);

                return $"{columnName}=a.[{columnName}]";
            }));

            // Generate merge statement
            var mergeStatement = 
            sqlQuery.AppendLine($"MERGE [{_tableNamePrefix}][{_tableName}] AS b");
            sqlQuery.AppendLine($"USING {tempTableName} AS a");
            sqlQuery.AppendLine($"ON {joinCondition}");
            sqlQuery.AppendLine($"WHEN NOT MATCHED BY TARGET THEN");
            sqlQuery.AppendLine($"INSERT ({insertedColumnsTo}) VALUES ({insertStatementFrom})");
            sqlQuery.AppendLine($"WHEN MATCHED BY TARGET THEN");
            sqlQuery.AppendLine($"UPDATE SET {updateStatement};");

            return sqlQuery.ToString();
        }
    }
}