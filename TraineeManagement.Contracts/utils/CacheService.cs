using System.Text.Json;
using StackExchange.Redis;

namespace TraineeManagement.Contracts.CacheServices;
 
    public class CacheService : ICacheService
    {
        private readonly IDatabase _db;
       
        public CacheService(IConnectionMultiplexer connection)
        {
            _db = connection.GetDatabase();
        }
 
        public async Task<T?> GetAsync<T>(string key)
        {
            RedisValue value = await _db.StringGetAsync(key);
            if (value.IsNullOrEmpty) return default;
            return JsonSerializer.Deserialize<T>(value!);
        }
 
        public async Task SetAsync<T>(string key, T value, int ttl)
        {
            RedisValue json = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, json, TimeSpan.FromMinutes(ttl));
        }
 
        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }
    }
