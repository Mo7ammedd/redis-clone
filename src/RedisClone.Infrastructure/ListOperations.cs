using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using RedisClone.Core.Models;

namespace RedisClone.Infrastructure
{
    public class ListOperations : BaseRedisStore
    {
        public ListOperations(ConcurrentDictionary<string, RedisValue> store) : base(store)
        {
        }

        public async Task<long> LPushAsync(string key, string value)
        {
            var redisValue = _store.GetOrAdd(key, _ => new RedisValue { Type = RedisValueType.List });
            
            lock (redisValue.ListValue)
            {
                redisValue.ListValue.Insert(0, value);
                return redisValue.ListValue.Count;
            }
        }

        public async Task<long> RPushAsync(string key, params string[] values)
        {
            if (!_store.TryGetValue(key, out var redisValue))
            {
                redisValue = new RedisValue { ListValue = new List<string>() };
                _store.TryAdd(key, redisValue);
            }

            if (redisValue.ListValue == null)
            {
                redisValue.ListValue = new List<string>();
            }

            lock (redisValue.ListValue)
            {
                redisValue.ListValue.AddRange(values);
                return redisValue.ListValue.Count;
            }
        }

        public async Task<string?> LPopAsync(string key)
        {
            if (TryGetValue(key, out var value) && value.Type == RedisValueType.List)
            {
                lock (value.ListValue)
                {
                    if (value.ListValue.Count > 0)
                    {
                        var result = value.ListValue[0];
                        value.ListValue.RemoveAt(0);
                        return result;
                    }
                }
            }
            return null;
        }

        public async Task<string?> RPopAsync(string key)
        {
            if (TryGetValue(key, out var value) && value.Type == RedisValueType.List)
            {
                lock (value.ListValue)
                {
                    if (value.ListValue.Count > 0)
                    {
                        var result = value.ListValue[value.ListValue.Count - 1];
                        value.ListValue.RemoveAt(value.ListValue.Count - 1);
                        return result;
                    }
                }
            }
            return null;
        }

        public async Task<IEnumerable<string>> LRangeAsync(string key, long start, long stop)
        {
            if (TryGetValue(key, out var value) && value.Type == RedisValueType.List)
            {
                lock (value.ListValue)
                {
                    var list = value.ListValue;
                    var startIndex = (int)Math.Max(0, start);
                    var endIndex = stop < 0 ? list.Count - 1 : (int)Math.Min(list.Count - 1, stop);

                    if (startIndex <= endIndex && startIndex < list.Count)
                    {
                        var count = endIndex - startIndex + 1;
                        return list.GetRange(startIndex, count);
                    }
                }
            }
            return new List<string>();
        }

        public async Task<string?> LIndexAsync(string key, int index)
        {
            if (!_store.TryGetValue(key, out var redisValue) || redisValue.ListValue == null)
            {
                return null;
            }

            lock (redisValue.ListValue)
            {
                if (index < 0)
                {
                    index = redisValue.ListValue.Count + index;
                }

                if (index < 0 || index >= redisValue.ListValue.Count)
                {
                    return null;
                }

                return redisValue.ListValue[index];
            }
        }
    }
} 