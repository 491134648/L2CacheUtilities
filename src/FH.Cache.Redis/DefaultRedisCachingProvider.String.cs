﻿using System.Threading.Tasks;
using FH.Cache.Core;

namespace FH.Cache.Redis
{
    /// <summary>
    /// Default redis caching provider.
    /// </summary>
    public partial class DefaultRedisCachingProvider : ICachingProvider
    {
        public long IncrBy(string cacheKey, long value = 1)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var res = _cache.StringIncrement(cacheKey, value);
            return res;
        }

        public async Task<long> IncrByAsync(string cacheKey, long value = 1)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var res = await _cache.StringIncrementAsync(cacheKey, value);
            return res;
        }

        public double IncrByFloat(string cacheKey, double value = 1)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var res = _cache.StringIncrement(cacheKey, value);
            return res;
        }

        public async Task<double> IncrByFloatAsync(string cacheKey, double value = 1)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var res = await _cache.StringIncrementAsync(cacheKey, value);
            return res;
        }

        public bool StringSet(string cacheKey, string cacheValue, System.TimeSpan? expiration)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            bool flag = _cache.StringSet(cacheKey, cacheValue, expiration);
            return flag;
        }

        public async Task<bool> StringSetAsync(string cacheKey, string cacheValue, System.TimeSpan? expiration)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            bool flag = await _cache.StringSetAsync(cacheKey, cacheValue, expiration);
            return flag;
        }

        public string StringGet(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var val = _cache.StringGet(cacheKey);
            return val;
        }

        public async Task<string> StringGetAsync(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var val = await _cache.StringGetAsync(cacheKey);
            return val;
        }

        public long StringLen(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var len = _cache.StringLength(cacheKey);
            return len;
        }
        public async Task<long> StringLenAsync(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var len = await _cache.StringLengthAsync(cacheKey);
            return len;
        }
        public long StringSetRange(string cacheKey, long offest, string value)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var res = _cache.StringSetRange(cacheKey, offest, value);
            return (long)res;
        }

        public async Task<long> StringSetRangeAsync(string cacheKey, long offest, string value)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var res = await _cache.StringSetRangeAsync(cacheKey, offest, value);
            return (long)res;
        }

        public string StringGetRange(string cacheKey, long start, long end)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var res = _cache.StringGetRange(cacheKey, start, end);
            return res;
        }

        public async Task<string> StringGetRangeAsync(string cacheKey, long start, long end)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var res = await _cache.StringGetRangeAsync(cacheKey, start, end);
            return res;
        }
    }
}
