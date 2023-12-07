using System.Data;
using System.Text;
using PetProject.StoreManagement.CrossCuttingConcerns.Extensions;
using PetProject.StoreManagement.Persistence.SqlServer.Extensions;

namespace PetProject.StoreManagement.Persistence.SqlServer.BulkAction_V1.BulkDelete
{
    public class BulkDeleteBuilder<T> : BulkBase<T>
    {
        #region Constructor

        public BulkDeleteBuilder(IDbConnection connection)
        {
            _connection = connection;
        }

        public BulkDeleteBuilder(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        public BulkDeleteBuilder(IDbConnection connection, IDbTransaction transaction = null)
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
            // + Insert data to TempTable
            // + Delete Datarows from ExistedTable

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

            var sqlQueryDeletingDataFromTable = GenerateSqlQueryDeletingDataFromTable(tempTableName, existedColumns);

            using (var deleteDataFromTable = _connection.CreateTextCommand(_transaction, sqlQueryDeletingDataFromTable))
            {
                deleteDataFromTable.ExecuteNonQuery();
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

        private string GenerateSqlQueryDeletingDataFromTable(string tempTableName, IEnumerable<string> columns)
        {
            var sqlQuery = new StringBuilder();
            var existedIdColumns = _idColumns.Where(x => columns.Contains(x));

            // Generate join conditions
            var joinCondition = string.Join(" and ", existedIdColumns.Select(x => 
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