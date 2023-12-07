using System.ComponentModel;
using System.Data;

namespace PetProject.OrderManagement.CrossCuttingConcerns.Extensions
{
    public static class IEnumerableExtensions
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> data, IEnumerable<string> propertyNames, bool isAddId = false)
        {
            // Get exact type of Generic type T
            var properties = TypeDescriptor.GetProperties(typeof(T));

            var dataTableProps = new List<PropertyDescriptor>();
            // Get all props that T has and propertyNames has
            foreach (PropertyDescriptor prop in properties)
            {
                if (propertyNames.Contains(prop.Name))
                {
                    dataTableProps.Add(prop);
                }
            }

            var dataTable = new DataTable();
            var index = 0;

            if (isAddId)
            {
                dataTable.Columns.Add("ID", typeof(long));
            }

            // Add properties for dataTable based on dataTableProps variable
            foreach (PropertyDescriptor prop in dataTableProps)
            {
                dataTable.Columns.Add(prop.Name, prop.PropertyType);
            }

            // Add data for dataTable
            foreach (T item in data)
            {
                var row = dataTable.NewRow();

                if (isAddId)
                {
                    row["ID"] = index++;
                }

                foreach(PropertyDescriptor prop in dataTableProps)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}
