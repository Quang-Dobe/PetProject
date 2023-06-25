using Microsoft.Data.SqlClient;
using PetProject.OrderManagement.CrossCuttingConcerns.Extensions;
using PetProject.OrderManagement.Infrastructure.SqlServer.Extensions;
using System.Data;
using System.Text;

namespace PetProject.OrderManagement.Infrastructure.SqlServer.BulkUpdate
{
    public class BulkUpdateBuilder<T>
    {
        private IEnumerable<T> _data;

        private IEnumerable<string> _idColumns;

        private IEnumerable<string> _columnNames;

        private IDictionary<string, string> _dbColumnMappings;

        private BulkOptions _options;

        private string _tableName;

        private string _tableNamePrefix;

        private readonly SqlConnection _connection;

        private readonly SqlTransaction _transaction;

        #region Constructor

        public BulkUpdateBuilder(SqlConnection connection)
        {
            _connection = connection;
        }

        public BulkUpdateBuilder(SqlTransaction transaction)
        {
            _transaction = transaction;
            _connection = transaction.Connection;
        }

        public BulkUpdateBuilder(SqlConnection connection, SqlTransaction transaction = null)
        {
            _connection = connection;
            _transaction = transaction;
        }

        #endregion

        public BulkUpdateBuilder<T> WithData(IEnumerable<T> data)
        {
            _data = data;

            return this;
        }

        public BulkUpdateBuilder<T> WithIdColumns(IEnumerable<string> idColumns)
        {
            _idColumns = idColumns;

            return this;
        }

        public BulkUpdateBuilder<T> WithColumnNames(IEnumerable<string> columnNames)
        {
            _columnNames = columnNames;

            return this;
        }

        public BulkUpdateBuilder<T> WithDbColumnMappings(IDictionary<string, string> columnMappings)
        {
            _dbColumnMappings = columnMappings;

            return this;
        }

        public BulkUpdateBuilder<T> WithTable(string tableName)
        {
            _tableName = tableName;

            return this;
        }

        public BulkUpdateBuilder<T> WithTablePrefix(string tableNamePrefix = "dbo")
        {
            _tableNamePrefix = tableNamePrefix;

            return this;
        }

        public BulkUpdateBuilder<T> WithConfigureBulkOptions(Action<BulkOptions> action)
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
            dataTableColumns.AddRange(_columnNames.ToList());
            dataTableColumns.AddRange(_idColumns.ToList());

            var dataTable = _data.ToDataTable(dataTableColumns.ToArray());
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

            GenerateDataForTempTable(dataTable, tempTableName, _dbColumnMappings, _connection, _transaction);
            
            using (var updateExistedTableCommand = _connection.CreateTextCommand(_transaction, sqlQueryUpdatingExitsedTable))
            {
                updateExistedTableCommand.ExecuteNonQuery();
            }
        }

        #region Private methods

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
            sqlQuery.AppendLine($"FROM {_tableName} a JOIN [{tempTableName}] b ON " + joinCondition);

            return sqlQuery.ToString();
        }

        private string GetDbColumnName(string columnName)
        {
            return _dbColumnMappings == null ? columnName : _dbColumnMappings[columnName];
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
                TimeOut = 30,
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
