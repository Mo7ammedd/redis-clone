using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedisClone.Core
{
    public interface IRedisStore
    {
        Task<bool> SetAsync(string key, string value, TimeSpan? expiration = null);
        Task<string?> GetAsync(string key);
        Task<bool> DeleteAsync(string key);
        Task<bool> ExistsAsync(string key);
        Task<bool> SetExpirationAsync(string key, TimeSpan expiration);
        Task<TimeSpan?> GetExpirationAsync(string key);
        Task<long> IncrementAsync(string key);
        Task<long> DecrementAsync(string key);
        Task<long> AppendAsync(string key, string value);
        Task<string?> GetRangeAsync(string key, int start, int end);
        Task<long> SetRangeAsync(string key, int offset, string value);
        Task<long> StrLenAsync(string key);
        Task<bool> SetBitAsync(string key, int offset, bool value);
        Task<bool> GetBitAsync(string key, int offset);
        Task<long> BitCountAsync(string key, int start = 0, int end = -1);
        Task<long> BitOpAsync(string operation, string destKey, params string[] keys);
        Task<long> BitPosAsync(string key, bool bit, int start = 0, int end = -1);
        Task<long> RPushAsync(string key, params string[] values);
        Task<long> LPushAsync(string key, params string[] values);
        Task<string?> LPopAsync(string key);
        Task<string?> RPopAsync(string key);
        Task<string?> LIndexAsync(string key, int index);
        Task<long> LLenAsync(string key);
        Task<bool> LSetAsync(string key, int index, string value);
        Task<long> LRemAsync(string key, int count, string value);
        Task<bool> LTrimAsync(string key, int start, int stop);
        Task<long> SAddAsync(string key, params string[] members);
        Task<long> SRemAsync(string key, params string[] members);
        Task<bool> SIsMemberAsync(string key, string member);
        Task<string[]> SMembersAsync(string key);
        Task<long> SCardAsync(string key);
        Task<string?> SPopAsync(string key);
        Task<string[]> SRandMemberAsync(string key, int count = 1);
        Task<long> SInterStoreAsync(string destination, params string[] keys);
        Task<long> SUnionStoreAsync(string destination, params string[] keys);
        Task<long> SDiffStoreAsync(string destination, params string[] keys);
        Task<bool> HSetAsync(string key, string field, string value);
        Task<string?> HGetAsync(string key, string field);
        Task<long> HDelAsync(string key, params string[] fields);
        Task<bool> HExistsAsync(string key, string field);
        Task<Dictionary<string, string>> HGetAllAsync(string key);
        Task<long> HIncrementByAsync(string key, string field, long increment);
        Task<string[]> HKeysAsync(string key);
        Task<long> HLenAsync(string key);
        Task<string[]> HValsAsync(string key);
        Task<long> ZAddAsync(string key, params (double score, string member)[] items);
        Task<long> ZRemAsync(string key, params string[] members);
        Task<double?> ZScoreAsync(string key, string member);
        Task<long> ZIncrementByAsync(string key, string member, double increment);
        Task<long> ZCardAsync(string key);
        Task<long> ZCountAsync(string key, double min, double max);
        Task<string[]> ZRangeAsync(string key, int start, int stop, bool withScores = false);
        Task<string[]> ZRevRangeAsync(string key, int start, int stop, bool withScores = false);
        Task<string[]> ZRangeByScoreAsync(string key, double min, double max, bool withScores = false);
        Task<string[]> ZRevRangeByScoreAsync(string key, double min, double max, bool withScores = false);
        Task<long> ZRankAsync(string key, string member);
        Task<long> ZRevRankAsync(string key, string member);
        Task<long> ZRemRangeByRankAsync(string key, int start, int stop);
        Task<long> ZRemRangeByScoreAsync(string key, double min, double max);
        Task<long> ZUnionStoreAsync(string destination, params string[] keys);
        Task<long> ZInterStoreAsync(string destination, params string[] keys);
        Task<bool> SubscribeAsync(string channel, Func<string, Task> callback);
        Task<bool> UnsubscribeAsync(string channel);
        Task<bool> PublishAsync(string channel, string message);
        Task<bool> BeginTransactionAsync();
        Task<bool> CommitTransactionAsync();
        Task<bool> RollbackTransactionAsync();
    }
} 