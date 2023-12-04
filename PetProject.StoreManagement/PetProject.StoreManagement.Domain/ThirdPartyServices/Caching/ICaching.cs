namespace PetProject.StoreManagement.Domain.ThirdPartyServices.Caching
{
    public interface ICaching
    {
        T? GetData<T>(string key);

        bool SetData<T>(string key, T value);

        bool RemoveData(string key);

        Task<T?> GetDataAsync<T>(string key);

        Task<bool> SetDataAsync<T>(string key, T value);

        Task<bool> RemoveDataAsync(string key);
    }
}
