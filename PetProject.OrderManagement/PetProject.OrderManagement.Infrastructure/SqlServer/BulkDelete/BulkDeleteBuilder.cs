using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using PetProject.OrderManagement.CrossCuttingConcerns.Extensions;
using PetProject.OrderManagement.Infrastructure.SqlServer.Extensions;

namespace PetProject.OrderManagement.Infrastructure.SqlServer.BulkDelete
{
    public class BulkDeleteBuilder<T>
    {
        private IEnumerable<T> _data;

        private IEnumerable<string> _idColumns;

        private IEnumerable<string> _columnNames;

        private IDictionary<string, string> _dbColumnMappings;

        private BulkOptions _options;

        private string _tableName;

        private string _tableNamePrefix;

        private SqlConnection _connection;

        private SqlTransaction _transaction;

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

        public BulkDeleteBuilder<T> WithData(IEnumerable<T> data)
        {
            _data = data;

            return this;
        }

        public BulkDeleteBuilder<T> WithIdColumns(IEnumerable<string> idColumns)
        {
            _idColumns = idColumns;

            return this;
        }

        public BulkDeleteBuilder<T> WithColumnNames(IEnumerable<string> columnNames)
        {
            _columnNames = columnNames;

            return this;
        }

        public BulkDeleteBuilder<T> WithDbColumnMappings(IDictionary<string, string> dbColumnMappings)
        {
            _dbColumnMappings = dbColumnMappings;

            return this;
        }

        public BulkDeleteBuilder<T> WithTable(string tableName)
        {
            _tableName = tableName;

            return this;
        }

        public BulkDeleteBuilder<T> WithTablePrefix(string tableNamePrefix = "dbo")
        {
            _tableNamePrefix = tableNamePrefix;

            return this;
        }

        public BulkDeleteBuilder<T> WithConfigureBulkOptions(Action<BulkOptions> action)
        {
            _options = new BulkOptions();

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

        #region Private methods

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

        private string GetDbColumnName(string idColumn)
        {
            return _dbColumnMappings.ContainsKey(idColumn) ? _dbColumnMappings[idColumn] : idColumn;
        }

        private void GenerateDataForTempTable(DataTable dataTable,
            string tempTableName,
            IDictionary<string, string> dbColumnMappings,
            SqlConnection connection,
            SqlTransaction transaction,
            BulkOptions options)
        {
            options ??= new BulkOptions()
            {
                BatchSize = _options.BatchSize,
                TimeOut = _options.TimeOut
            };

            var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction)
            {
                BatchSize = options.BatchSize,
                BulkCopyTimeout = options.TimeOut,
                DestinationTableName = $"[{tempTableName}]"
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