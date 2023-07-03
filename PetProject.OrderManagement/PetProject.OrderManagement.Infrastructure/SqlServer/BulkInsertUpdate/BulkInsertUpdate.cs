using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using PetProject.OrderManagement.CrossCuttingConcerns.Extensions;
using PetProject.OrderManagement.Infrastructure.SqlServer.Extensions;

namespace PetProject.OrderManagement.Infrastructure.SqlServer.BulkInsertUpdate
{
    public class BulkInsertUpdateBuilder<T>
    {
        private IEnumerable<T> _data;

        private IEnumerable<string> _idColumns;

        private IEnumerable<string> _columnNames;

        private IDictionary<string, string> _dbColumnMappings;

        private string _tableName;

        private string _tableNamePrefix;

        private BulkOptions _options;

        private SqlConnection _connection;

        private SqlTransaction _transaction;

        #region Constructor

        public BulkInsertUpdateBuilder(SqlConnection connection)
        {
            _connection = connection;
        }

        public BulkInsertUpdateBuilder(SqlTransaction transaction)
        {
            _transaction = transaction;
        }

        public BulkInsertUpdateBuilder(SqlConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        #endregion

        public BulkInsertUpdateBuilder<T> WithData(IEnumerable<T> data)
        {
            _data = data;

            return this;
        }

        public BulkInsertUpdateBuilder<T> WithIdColumns(IEnumerable<string> idColumns)
        {
            _idColumns = idColumns;

            return this;
        }

        public BulkInsertUpdateBuilder<T> WithColumnNames(IEnumerable<string> columnNames)
        {
            _columnNames = columnNames;

            return this;
        }

        public BulkInsertUpdateBuilder<T> WithDbColumnMappings(IDictionary<string, string> dbColumnMappings)
        {
            _dbColumnMappings = dbColumnMappings;

            return this;
        }

        public BulkInsertUpdateBuilder<T> WithConfigureBulkOption(Action<BulkOptions> action)
        {
            if (action != null)
            {
                action(_options);
            }

            return this;
        }

        public void Excute()
        {
            var tempTableName = "#" + Guid.NewGuid();

            var dataTableColumns = new List<string>();
            dataTableColumns.AddRange(_idColumns);
            dataTableColumns.AddRange(_columnNames);

            var dataTable = _data.ToDataTable(dataTableColumns);
            var sqlQueryCreatingTempTable = dataTable.ToSqlQueryCreatingTable(tempTableName);
            var sqlQueryInsertOrUpdateExistedTable = GenerateSqlQueryInsertOrUpdateExistedTable(tempTableName, dataTableColumns);

            using(var createTempTable = _connection.CreateTextCommand(_transaction, sqlQueryCreatingTempTable))
            {
                createTempTable.ExecuteNonQuery();
            }

            GenerateDataForTempTable(dataTable, tempTableName, _dbColumnMappings, _connection, _transaction, _options);

            using(var insertOrUpdateExistedTable = _connection.CreateTextCommand(_transaction, sqlQueryInsertOrUpdateExistedTable))
            {
                insertOrUpdateExistedTable.ExecuteNonQuery();
            }
        }

        #region Private methods

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

        private string GetDbColumnName(string idColumn)
        {
            if (_dbColumnMappings != null && _dbColumnMappings.ContainsKey(idColumn))
            {
                return _dbColumnMappings[idColumn];
            }

            return idColumn;
        }

        private void GenerateDataForTempTable(DataTable dataTable,
            string tempTableName,
            IDictionary<string, string> dbColumnMappings,
            SqlConnection connection,
            SqlTransaction transaction,
            BulkOptions options = null)
        {
            options ??= new BulkOptions()
            {
                BatchSize = 0,
                TimeOut = 30
            };

            var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction)
            {
                BatchSize = options.BatchSize,
                BulkCopyTimeout = options.TimeOut,
                DestinationTableName = $"{tempTableName}"
            };

            foreach (DataColumn column in dataTable.Columns)
            {
                if (dbColumnMappings != null && dbColumnMappings.ContainsKey(column.ColumnName))
                {
                    bulkCopy.ColumnMappings.Add(column.ColumnName, dbColumnMappings[column.ColumnName]);
                }
                else
                {
                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }
            }

            bulkCopy.WriteToServer(dataTable);
        }

        #endregion
    }
}