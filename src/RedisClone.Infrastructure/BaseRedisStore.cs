using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using RedisClone.Core.Models;

namespace RedisClone.Infrastructure
{
    public abstract class BaseRedisStore
    {
        protected readonly ConcurrentDictionary<string, RedisValue> _store;
        protected readonly object _lock = new object();

        protected BaseRedisStore(ConcurrentDictionary<string, RedisValue> store)
        {
            _store = store;
        }

        protected bool TryGetValue(string key, out RedisValue? value)
        {
            if (_store.TryGetValue(key, out value))
            {
                if (value.IsExpired())
                {
                    _store.TryRemove(key, out _);
                    value = null;
                    return false;
                }
                return true;
            }
            return false;
        }
    }
} 