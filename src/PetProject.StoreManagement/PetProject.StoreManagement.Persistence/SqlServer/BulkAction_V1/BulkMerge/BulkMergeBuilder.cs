using System.Data;
using System.Text;
using PetProject.StoreManagement.CrossCuttingConcerns.Extensions;
using PetProject.StoreManagement.Persistence.SqlServer.Extensions;

namespace PetProject.StoreManagement.Persistence.SqlServer.BulkAction_V1.BulkMerge
{
    public class BulkMergeBuilder<T> : BulkBase<T>
    {
        #region Constructor

        public BulkMergeBuilder(IDbConnection connection)
        {
            _connection = connection;
        }

        public BulkMergeBuilder(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        public BulkMergeBuilder(IDbConnection connection, IDbTransaction transaction = null)
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

            var existedIdColumns = _idColumns.Where(x => columns.Contains(x));
            var existedColumnNames = _columnNames.Where(x => columns.Contains(x));

            // Generate join condition
            var joinCondition = string.Join(" and ", existedIdColumns.Select(x => 
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

            // Generate update statement
            var updateStatement = string.Join(", ", existedColumnNames.Select(x =>
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