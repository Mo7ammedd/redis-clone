using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using RedisClone.Core.Models;

namespace RedisClone.Infrastructure
{
    public class SetOperations : BaseRedisStore
    {
        public SetOperations(ConcurrentDictionary<string, RedisValue> store) : base(store)
        {
        }

        public async Task<long> SAddAsync(string key, params string[] members)
        {
            if (!_store.TryGetValue(key, out var redisValue))
            {
                redisValue = new RedisValue { Type = RedisValueType.Set, SetValue = new HashSet<string>() };
                _store.TryAdd(key, redisValue);
            }
            else if (redisValue.Type != RedisValueType.Set)
            {
                redisValue.Type = RedisValueType.Set;
                redisValue.SetValue = new HashSet<string>();
            }

            long addedCount = 0;
            lock (redisValue.SetValue)
            {
                foreach (var member in members)
                {
                    if (redisValue.SetValue.Add(member))
                    {
                        addedCount++;
                    }
                }
            }

            return addedCount;
        }

        public async Task<long> SRemAsync(string key, string value)
        {
            if (TryGetValue(key, out var redisValue) && redisValue.Type == RedisValueType.Set)
            {
                lock (redisValue.SetValue)
                {
                    return redisValue.SetValue.Remove(value) ? 1 : 0;
                }
            }
            return 0;
        }

        public async Task<IEnumerable<string>> SMembersAsync(string key)
        {
            if (TryGetValue(key, out var value) && value.Type == RedisValueType.Set && value.SetValue != null)
            {
                return value.SetValue;
            }
            return new HashSet<string>();
        }

        public async Task<bool> SIsMemberAsync(string key, string value)
        {
            if (TryGetValue(key, out var redisValue) && redisValue.Type == RedisValueType.Set && redisValue.SetValue != null)
            {
                return redisValue.SetValue.Contains(value);
            }
            return false;
        }
    }
} 