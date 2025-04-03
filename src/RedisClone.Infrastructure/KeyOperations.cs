using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using RedisClone.Core.Models;

namespace RedisClone.Infrastructure
{
    public class KeyOperations : BaseRedisStore
    {
        public KeyOperations(ConcurrentDictionary<string, RedisValue> store) : base(store)
        {
        }

        public async Task<bool> SetAsync(string key, string value, TimeSpan? expiration = null)
        {
            var redisValue = new RedisValue
            {
                Type = RedisValueType.String,
                StringValue = value,
                ExpirationTime = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : null
            };

            return _store.TryAdd(key, redisValue) || _store.TryUpdate(key, redisValue, _store[key]);
        }

        public async Task<string?> GetAsync(string key)
        {
            if (TryGetValue(key, out var value))
            {
                return value.StringValue;
            }
            return null;
        }

        public async Task<bool> DeleteAsync(string key)
        {
            return _store.TryRemove(key, out _);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return TryGetValue(key, out _);
        }

        public async Task<TimeSpan?> TTLAsync(string key)
        {
            if (_store.TryGetValue(key, out var value) && value.ExpirationTime.HasValue)
            {
                var ttl = value.ExpirationTime.Value - DateTime.UtcNow;
                return ttl.TotalSeconds > 0 ? ttl : null;
            }
            return null;
        }

        public async Task<bool> SetExpirationAsync(string key, TimeSpan expiration)
        {
            if (_store.TryGetValue(key, out var value))
            {
                value.ExpirationTime = DateTime.UtcNow.Add(expiration);
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveExpirationAsync(string key)
        {
            if (_store.TryGetValue(key, out var value))
            {
                value.ExpirationTime = null;
                return true;
            }
            return false;
        }
    }
} 