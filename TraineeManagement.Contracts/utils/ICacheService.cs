namespace TraineeManagement.Contracts.CacheServices;

public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, int ttl);
        Task RemoveAsync(string key);
    }