using Microsoft.Extensions.Caching.Distributed;
using PetProject.IdentityServer.CrossCuttingConcerns.Extensions;
using PetProject.IdentityServer.Domain.ThirdPartyServices.Caching;
using System.Text.Json;

namespace PetProject.IdentityServer.Infrastructure.CachingService
{
    public class Caching : ICaching
    {
        private readonly IDistributedCache _distributedCache;
        public Caching(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public T? GetData<T>(string key)
        {
            var result = _distributedCache.GetString(key) ?? "";

            if (!result.IsNullOrEmpty())
            {
                return JsonSerializer.Deserialize<T>(result);
            }

            return default;
        }

        public async Task<T> GetDataAsync<T>(string key)
        {
            var result = await _distributedCache.GetStringAsync(key) ?? "";

            if (!result.IsNullOrEmpty())
            {
                return JsonSerializer.Deserialize<T>(result);
            }

            return default;
        }

        public bool SetData<T>(string key, T value)
        {
            try
            {
                _distributedCache.SetString(key, JsonSerializer.Serialize(value));

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SetDataAsync<T>(string key, T value)
        {
            try
            {
                await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(value));

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveData(string key)
        {
            try
            {
                var isExistKey = !_distributedCache.GetString(key).IsNullOrEmpty();

                if (isExistKey)
                {
                    _distributedCache.Remove(key);

                    return true;
                }

                return false;
            }
            catch 
            {
                return false;
            }
        }

        public async Task<bool> RemoveDataAsync(string key)
        {
            try
            {
                var isExistKey = !(await _distributedCache.GetStringAsync(key)).IsNullOrEmpty();

                if (isExistKey)
                {
                    await _distributedCache.RemoveAsync(key);

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
