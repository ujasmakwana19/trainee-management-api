using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace TraineeManagement.Data.CacheServices;
 
    public class CacheService : ICacheService
    {
        private readonly IDatabase _db;
        private readonly ILogger<CacheService> _logger;
       
        public CacheService(IConnectionMultiplexer connection, ILogger<CacheService> logger)
        {
            _db = connection.GetDatabase();
            _logger = logger;
        }
 
        public async Task<T?> GetAsync<T>(string key)
        {
            try{    
                RedisValue value = await _db.StringGetAsync(key);
                if (value.IsNullOrEmpty) 
                    return default;
                return JsonSerializer.Deserialize<T>(value!);
            }
            catch (Exception ex)
            {
                _logger.LogError("Cache Service is down:{ex}", ex.Message);
            }
            return default;
        }
 
        public async Task SetAsync<T>(string key, T value, int ttl)
        {
            try{
                RedisValue json = JsonSerializer.Serialize(value);
                await _db.StringSetAsync(key, json, TimeSpan.FromMinutes(ttl));
            }
            catch (Exception ex)
            {
                _logger.LogError("Cache Service is down:{ex}", ex.Message);
            }
            return;
        }
 
        public async Task RemoveAsync(string key)
        {
            try
            {
                await _db.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError("Cache Service is down:{ex}", ex.Message);
            }
            return;
        }
    }
