using System.Data;
using System.Text;
using PetProject.OrderManagement.CrossCuttingConcerns.Extensions;
using PetProject.OrderManagement.Infrastructure.SqlServer.Extensions;

namespace PetProject.OrderManagement.Infrastructure.SqlServer.BulkAction_V1.BulkInsert
{
    public class BulkInsertBuilder<T> : BulkBase<T>
    {
        #region Constructor

        public BulkInsertBuilder(IDbConnection connection)
        {
            _connection = connection;
        }

        public BulkInsertBuilder(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        public BulkInsertBuilder(IDbConnection connection, IDbTransaction transaction = null)
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
            dataTableColumns.AddRange(_idColumns);
            dataTableColumns.AddRange(_columnNames);

            var dataTable = _data.ToDataTable(dataTableColumns);
            var sqlQueryCreatingTempTable = dataTable.ToSqlQueryCreatingTable(tempTableName);
            var sqlQueryGetColumnsOfCurrentTable = GenerateSqlQueryGetColumnsOfCurrentTable();

            // WorkFlow:
            // + Open connection to database and Keep connection
            // + Create TempTable
            // + Insert data to ExistedTable
            // + Merge data from TempTable into ExistedTable (Ignore duplicated data)
            //   If not matched => Insert new datarow

            _connection.EnsureOpen();

            using (var createTempTable = _connection.CreateTextCommand(_transaction, sqlQueryCreatingTempTable))
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

            var sqlQueryInsertingExistedTable = GenerateSqlQueryInsertingExistedTable(tempTableName, existedColumns);
            
            using (var insertExistedTable = _connection.CreateTextCommand(_transaction, sqlQueryInsertingExistedTable))
            {
                insertExistedTable.ExecuteNonQuery();
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

        private string GenerateSqlQueryInsertingExistedTable(string tempTableName, IEnumerable<string> columns)
        {
            var sqlQuery = new StringBuilder();

            var existedIdColumns = _idColumns.Where(x => columns.Contains(x));
            var existedColumnNames = _columnNames.Where(x => columns.Contains(x));

            // Generate join condition
            var joinCondition = string.Join(" and ", _idColumns.Where(x => existedIdColumns.Contains(x)).Select(x => 
            {
                var columnName = GetDbColumnName(x);

                return $"a.[{columnName}]=b.[{columnName}]";
            }));

            // Generate insert statement
            var insertStatementFrom = string.Join(", ", existedColumnNames.Select(x => 
            {
                var columnName = GetDbColumnName(x);

                return $"a.[{columnName}]";
            }));
            var insertedColumnsTo = string.Join(", ", existedColumnNames.Select(x => GetDbColumnName(x)));

            // Generate merge statement
            var mergeStatement = 
            sqlQuery.AppendLine($"MERGE [{_tableNamePrefix}][{_tableName}] AS b");
            sqlQuery.AppendLine($"USING {tempTableName} AS a");
            sqlQuery.AppendLine($"ON {joinCondition}");
            sqlQuery.AppendLine($"WHEN NOT MATCHED BY TARGET THEN");
            sqlQuery.AppendLine($"INSERT ({insertedColumnsTo}) VALUES ({insertStatementFrom});");

            return sqlQuery.ToString();
        }
    }
}