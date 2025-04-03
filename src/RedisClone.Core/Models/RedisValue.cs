using System;
using System.Collections.Generic;

namespace RedisClone.Core.Models
{
    public class RedisValue
    {
        public RedisValueType Type { get; set; }
        public string? StringValue { get; set; }
        public List<string> ListValue { get; set; }
        public HashSet<string> SetValue { get; set; }
        public Dictionary<string, string> HashValue { get; set; }
        public SortedDictionary<string, double> SortedSetValue { get; set; }
        public DateTime? ExpirationTime { get; set; }

        public RedisValue()
        {
            ListValue = new List<string>();
            SetValue = new HashSet<string>();
            HashValue = new Dictionary<string, string>();
            SortedSetValue = new SortedDictionary<string, double>();
        }

        public bool IsExpired()
        {
            return ExpirationTime.HasValue && DateTime.UtcNow > ExpirationTime.Value;
        }
    }

    public enum RedisValueType
    {
        String,
        List,
        Set,
        Hash,
        SortedSet
    }
} 