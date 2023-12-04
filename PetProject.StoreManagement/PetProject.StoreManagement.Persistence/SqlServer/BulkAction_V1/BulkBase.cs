using System.Data;
using Microsoft.Data.SqlClient;

namespace PetProject.StoreManagement.Persistence.SqlServer.BulkAction_V1
{
    public abstract class BulkBase<T>
    {
        #region Base properties

        protected IEnumerable<T> _data;

        protected IEnumerable<string> _idColumns;

        protected IEnumerable<string> _columnNames;

        protected IDictionary<string, string> _dbColumnMappings;

        protected BulkOptions _options;

        protected string _tableName;

        protected string _tableNamePrefix;

        protected IDbConnection _connection;

        protected IDbTransaction _transaction;

        #endregion

        #region Base methods

        public BulkBase<T> WithData(IEnumerable<T> data)
        {
            _data = data;

            return this;
        }

        public BulkBase<T> WithIdColumns(IEnumerable<string> idColumns)
        {
            _idColumns = idColumns;

            return this;
        }

        public BulkBase<T> WithColumnNames(IEnumerable<string> columnNames)
        {
            _columnNames = columnNames;

            return this;
        }

        public BulkBase<T> WithDbColumnMappings(IDictionary<string, string> dbColumnMappings)
        {
            _dbColumnMappings = dbColumnMappings;

            return this;
        }

        public BulkBase<T> WithTable(string tableName)
        {
            _tableName = tableName;

            return this;
        }

        public BulkBase<T> WithTablePrefix(string tableNamePrefix = "dbo")
        {
            _tableNamePrefix = tableNamePrefix;

            return this;
        }

        public BulkBase<T> WithConfigureBulkOptions(Action<BulkOptions> action)
        {
            _options = new BulkOptions();

            if (action != null)
            {
                action(_options);
            }

            return this;
        }

        protected string GetDbColumnName(string idColumn)
        {
            if (_dbColumnMappings != null && _dbColumnMappings.ContainsKey(idColumn))
            {
                return _dbColumnMappings[idColumn];
            }
            
            return idColumn;
        }

        protected void GenerateDataForTempTable(DataTable dataTable,
            string tempTableName,
            IDictionary<string, string> dbColumnMappings,
            IDbConnection connection,
            IDbTransaction transaction,
            BulkOptions options)
        {
            options ??= new BulkOptions()
            {
                BatchSize = _options.BatchSize,
                TimeOut = _options.TimeOut
            };

            var bulkCopy = new SqlBulkCopy((SqlConnection)connection, SqlBulkCopyOptions.Default, (SqlTransaction)transaction)
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

        public virtual void Excute()
        { }
    }
}