namespace PetProject.StoreManagement.Domain.ThirdPartyServices.BulkActions
{
    public interface IBulkActions
    {
        void BulkInsert(IEnumerable<object> data, IEnumerable<string> columnNames);

        void BulkUpdate(IEnumerable<object> data, IEnumerable<string> columnNames);

        void BulkMerge(IEnumerable<object> data, IEnumerable<string> columnNames);

        void BulkDelete(IEnumerable<object> data, IEnumerable<string> columnNames);
    }
}
