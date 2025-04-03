using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RedisClone.Core.Models;

namespace RedisClone.Infrastructure
{
    public class SortedSetOperations : BaseRedisStore
    {
        public SortedSetOperations(ConcurrentDictionary<string, RedisValue> store) : base(store)
        {
        }

        public async Task<long> ZAddAsync(string key, string member, double score)
        {
            var redisValue = _store.GetOrAdd(key, _ => new RedisValue 
            { 
                Type = RedisValueType.SortedSet,
                SortedSetValue = new SortedDictionary<string, double>()
            });
            
            lock (redisValue.SortedSetValue)
            {
                var isNew = !redisValue.SortedSetValue.ContainsKey(member);
                redisValue.SortedSetValue[member] = score;
                return isNew ? 1 : 0;
            }
        }

        public async Task<long> ZAddAsync(string key, params (double score, string member)[] items)
        {
            var redisValue = _store.GetOrAdd(key, _ => new RedisValue 
            { 
                Type = RedisValueType.SortedSet,
                SortedSetValue = new SortedDictionary<string, double>()
            });
            
            lock (redisValue.SortedSetValue)
            {
                var added = 0;
                foreach (var (score, member) in items)
                {
                    if (!redisValue.SortedSetValue.ContainsKey(member))
                    {
                        added++;
                    }
                    redisValue.SortedSetValue[member] = score;
                }
                return added;
            }
        }

        public async Task<long> ZRemAsync(string key, string member)
        {
            if (TryGetValue(key, out var value) && value.Type == RedisValueType.SortedSet)
            {
                return value.SortedSetValue.Remove(member) ? 1 : 0;
            }
            return 0;
        }

        public async Task<IEnumerable<string>> ZRangeAsync(string key, long start, long stop, bool withScores = false)
        {
            if (TryGetValue(key, out var value) && value.Type == RedisValueType.SortedSet)
            {
                var sortedItems = value.SortedSetValue.OrderBy(x => x.Value).ToList();
                var startIndex = (int)Math.Max(0, start);
                var endIndex = stop < 0 ? sortedItems.Count - 1 : (int)Math.Min(sortedItems.Count - 1, stop);

                if (startIndex <= endIndex && startIndex < sortedItems.Count)
                {
                    var count = endIndex - startIndex + 1;
                    var range = sortedItems.GetRange(startIndex, count);

                    if (withScores)
                    {
                        return range.Select(x => $"{x.Key}:{x.Value}");
                    }
                    return range.Select(x => x.Key);
                }
            }
            return Enumerable.Empty<string>();
        }

        public async Task<IEnumerable<string>> ZRevRangeAsync(string key, long start, long stop, bool withScores = false)
        {
            if (TryGetValue(key, out var value) && value.Type == RedisValueType.SortedSet)
            {
                var sortedItems = value.SortedSetValue.OrderByDescending(x => x.Value).ToList();
                var startIndex = (int)Math.Max(0, start);
                var endIndex = stop < 0 ? sortedItems.Count - 1 : (int)Math.Min(sortedItems.Count - 1, stop);

                if (startIndex <= endIndex && startIndex < sortedItems.Count)
                {
                    var count = endIndex - startIndex + 1;
                    var range = sortedItems.GetRange(startIndex, count);

                    if (withScores)
                    {
                        return range.Select(x => $"{x.Key}:{x.Value}");
                    }
                    return range.Select(x => x.Key);
                }
            }
            return Enumerable.Empty<string>();
        }
    }
} 