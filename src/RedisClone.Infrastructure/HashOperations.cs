using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using RedisClone.Core.Models;

namespace RedisClone.Infrastructure
{
    public class HashOperations : BaseRedisStore
    {
        public HashOperations(ConcurrentDictionary<string, RedisValue> store) : base(store)
        {
        }

        public async Task<bool> HSetAsync(string key, string field, string value)
        {
            var redisValue = _store.GetOrAdd(key, _ => new RedisValue { Type = RedisValueType.Hash });
            
            lock (redisValue.HashValue)
            {
                redisValue.HashValue[field] = value;
                return true;
            }
        }

        public async Task<string?> HGetAsync(string key, string field)
        {
            if (TryGetValue(key, out var value) && value.Type == RedisValueType.Hash)
            {
                return value.HashValue.TryGetValue(field, out var result) ? result : null;
            }
            return null;
        }

        public async Task<Dictionary<string, string>> HGetAllAsync(string key)
        {
            if (TryGetValue(key, out var value) && value.Type == RedisValueType.Hash)
            {
                return new Dictionary<string, string>(value.HashValue);
            }
            return new Dictionary<string, string>();
        }

        public async Task<bool> HDelAsync(string key, string field)
        {
            if (TryGetValue(key, out var value) && value.Type == RedisValueType.Hash)
            {
                return value.HashValue.Remove(field);
            }
            return false;
        }
    }
} 