namespace PetProject.OrderManagement.Persistence.SqlServer.Extensions
{
    public static class TypeExtensions
    {
        private static Dictionary<Type, string> _mappings = new Dictionary<Type, string>
        {
            {typeof(bool), "bit"},
            {typeof(DateTime), "datetime2"},
            {typeof(DateTimeOffset), "datetimeoffset"},
            {typeof(decimal), "decimal(38, 20)"},
            {typeof(double), "double"},
            {typeof(Guid), "uniqueidentifier"},
            {typeof(short), "smallint"},
            {typeof(int), "int"},
            {typeof(long), "bigint"},
            {typeof(float), "single"},
            {typeof(string), "nvarchar(max)"},
        };

        public static string ToSqlType(this Type type)
        {
            return _mappings.ContainsKey(type) ? _mappings[type] : "nvarchar(max)";
        }
    }
}
