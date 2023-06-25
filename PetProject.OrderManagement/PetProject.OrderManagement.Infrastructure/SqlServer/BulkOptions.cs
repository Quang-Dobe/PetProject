namespace PetProject.OrderManagement.Infrastructure.SqlServer
{
    public class BulkOptions
    {
        public int BatchSize { get; set; }

        public int TimeOut { get; set; }

        public BulkOptions()
        {
            TimeOut = 30;
        }
    }
}
