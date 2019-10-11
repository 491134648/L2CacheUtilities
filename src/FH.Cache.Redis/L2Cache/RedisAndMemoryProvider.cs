using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FH.Cache.Core;
using FH.Cache.Core.InMemory;
using Microsoft.Extensions.Logging;

namespace FH.Cache.Redis.L2Cache
{
    /// <summary>
    ///内存、redis二级缓存的实现
    /// </summary>
    public class RedisAndMemoryProvider : CachingAbstractProvider, IL2CacheProvider
    {
        private readonly ICachingProvider _memoryProvider;
        private readonly ICachingProvider _redisProvider;
        private readonly ICachingProviderFactory _providerFactory;
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;
        public RedisAndMemoryProvider(ICachingProviderFactory providerFactory
            ,ILoggerFactory loggerFactory = null)
        {
            _providerFactory = providerFactory;
            _memoryProvider = _providerFactory.GetCachingProvider(CachingConstValue.DefaultInMemoryName);
            _redisProvider = _providerFactory.GetCachingProvider(CachingConstValue.DefaultRedisName);
            _logger = loggerFactory?.CreateLogger<RedisAndMemoryProvider>();
        }
        public override bool BaseExists(string cacheKey)
        {
            bool isExists= _memoryProvider.Exists(cacheKey);
            if (!isExists)
            {
                isExists = _redisProvider.Exists(cacheKey);
            }
            return isExists;
        }

        public override Task<bool> BaseExistsAsync(string cacheKey)
        {
            bool isExists = _memoryProvider.Exists(cacheKey);
            if (!isExists)
            {
                return _redisProvider.ExistsAsync(cacheKey);
            }
            return Task.FromResult(isExists);
        }

        public override void BaseFlush()
        {
            List<string> keys = null;
            DefaultInMemoryCachingProvider memoryCachingProvider =(DefaultInMemoryCachingProvider) _memoryProvider;
            if (memoryCachingProvider != null)
            {
                keys = memoryCachingProvider.Keys;
            }
            if (keys != null)
            {
                for(int i = 0; i < keys.Count; i+=500)
                {
                    string[] deleteKeys = keys.Skip(i).Take(500).ToArray();
                    if (deleteKeys != null && deleteKeys.Length > 0)
                    {
                        _redisProvider.RemoveAll(deleteKeys);
                    }
                }
            }
            _memoryProvider.Flush();
        }

        public override Task BaseFlushAsync()
        {
            List<string> keys = null;
            DefaultInMemoryCachingProvider memoryCachingProvider = (DefaultInMemoryCachingProvider)_memoryProvider;
            if (memoryCachingProvider != null)
            {
                keys = memoryCachingProvider.Keys;
            }
            if (keys != null)
            {
                for (int i = 0; i < keys.Count; i += 500)
                {
                    string[] deleteKeys = keys.Skip(i).Take(500).ToArray();
                    if (deleteKeys != null && deleteKeys.Length > 0)
                    {
                        _redisProvider.RemoveAllAsync(deleteKeys);
                    }
                }
            }
            return _memoryProvider.FlushAsync();
        }

        public override CacheValue<T> BaseGet<T>(string cacheKey, Func<T> dataRetriever, TimeSpan expiration)
        {
            CacheValue<T> cacheValue = _memoryProvider.Get<T>(cacheKey);
            if (cacheValue.IsNull)
            {
                cacheValue = _redisProvider.Get<T>(cacheKey);
                if (cacheValue.HasValue)
                {
                    var expration = _redisProvider.GetExpiration(cacheKey);
                    if (expration != null&& expration!=TimeSpan.Zero)
                    {
                        _memoryProvider.Set<T>(cacheKey, cacheValue.Value, expration);
                    }
                }
            }
            if (cacheValue.HasValue)
            {
                return cacheValue;
            }
            T data = dataRetriever();
            if (data != null)
            {
                _memoryProvider.Set<T>(cacheKey, data, expiration);
                _redisProvider.Set<T>(cacheKey, data, expiration);
                return new CacheValue<T>(data, true);
            }
            return new CacheValue<T>(data, false);
        }

        public override CacheValue<T> BaseGet<T>(string cacheKey)
        {
            CacheValue<T> cacheValue = _memoryProvider.Get<T>(cacheKey);
            if (cacheValue.IsNull)
            {
                cacheValue = _redisProvider.Get<T>(cacheKey);
                if (cacheValue.HasValue)
                {
                    var expration = _redisProvider.GetExpiration(cacheKey);
                    if (expration != null&&expration!=TimeSpan.Zero)
                    {
                        _memoryProvider.Set<T>(cacheKey, cacheValue.Value, expration);
                    }
                }
            }
            return cacheValue;
        }

        public override IDictionary<string, CacheValue<T>> BaseGetAll<T>(IEnumerable<string> cacheKeys)
        {
            IDictionary<string, CacheValue<T>> cacheValues = _memoryProvider.GetAll<T>(cacheKeys);
            if (cacheValues!=null&&cacheValues.Count>0)
            {
                string[] alreadKeys = cacheValues.Keys.ToArray();
                cacheKeys = cacheKeys.Where(r => !alreadKeys.Contains(r)).ToList();
            }
            if (cacheKeys != null && cacheKeys.Count() > 0)
            {
                IDictionary<string, CacheValue<T>>  datas= _redisProvider.GetAll<T>(cacheKeys);
                if (datas != null && datas.Count > 0)
                {
                    foreach(var item in datas)
                    {
                        if (item.Value.HasValue)
                        {
                            cacheValues.Add(item.Key, item.Value);
                        }
                        
                    }
                }
            }
            return cacheValues;
        }

        public override Task<IDictionary<string, CacheValue<T>>> BaseGetAllAsync<T>(IEnumerable<string> cacheKeys)
        {
            IDictionary<string, CacheValue<T>> cacheValues = _memoryProvider.GetAll<T>(cacheKeys);
            if (cacheValues != null && cacheValues.Count > 0)
            {
                string[] alreadKeys = cacheValues.Keys.ToArray();
                cacheKeys = cacheKeys.Where(r => !alreadKeys.Contains(r)).ToList();
            }
            if (cacheKeys != null && cacheKeys.Count() > 0)
            {
                IDictionary<string, CacheValue<T>> datas = _redisProvider.GetAll<T>(cacheKeys);
                if (datas != null && datas.Count > 0)
                {
                    foreach (var item in datas)
                    {
                        if (item.Value.HasValue)
                        {
                            cacheValues.Add(item.Key, item.Value);
                        }

                    }
                }
            }
            return Task.FromResult(cacheValues);
        }
        public override async Task<CacheValue<T>> BaseGetAsync<T>(string cacheKey, Func<Task<T>> dataRetriever, TimeSpan expiration)
        {
            CacheValue<T> cacheValue = _memoryProvider.Get<T>(cacheKey);
            if (cacheValue.IsNull)
            {
                cacheValue = _redisProvider.Get<T>(cacheKey);
                if (cacheValue.HasValue)
                {
                    var expri = _redisProvider.GetExpiration(cacheKey);
                    if (expri != TimeSpan.Zero)
                    {
                        _memoryProvider.Set(cacheKey, cacheValue.Value, expri);
                    }
                }
            }
            if (cacheValue.IsNull)
            {
                T data =await dataRetriever();
                if (data != null)
                {
                    _memoryProvider.Set<T>(cacheKey, data, expiration);
                    _redisProvider.Set<T>(cacheKey, data, expiration);
                    cacheValue = new CacheValue<T>(data, true);
                }
            }
            return cacheValue;
        }

        public override async Task<object> BaseGetAsync(string cacheKey, Type type)
        {

            var data= await _memoryProvider.GetAsync(cacheKey,type);
            if (data==null)
            {
                data = await _redisProvider.GetAsync(cacheKey, type);
                if (data != null)
                {
                    var expir = _redisProvider.GetExpiration(cacheKey);
                    if (expir != TimeSpan.Zero)
                    {
                        _memoryProvider.Set(cacheKey, data, expir);
                    }
                }
            }
            return data;
        }

        public override Task<CacheValue<T>> BaseGetAsync<T>(string cacheKey)
        {
             var data= _memoryProvider.Get<T>(cacheKey);
            if (data.IsNull)
            {
                data = _redisProvider.Get<T>(cacheKey);
                if (data.HasValue)
                {
                    var expire = _redisProvider.GetExpiration(cacheKey);
                    if (expire != TimeSpan.Zero)
                    {
                        _memoryProvider.Set(cacheKey, data.Value, expire);
                    }
                }
            }
            return Task.FromResult(data);
        }

        public override IDictionary<string, CacheValue<T>> BaseGetByPrefix<T>(string prefix)
        {
            IDictionary<string, CacheValue<T>>  cacheValues=_memoryProvider.GetByPrefix<T>(prefix);
            IDictionary<string, CacheValue<T>> redisValues = _redisProvider.GetByPrefix<T>(prefix);
            if (cacheValues.Count>0&&cacheValues.Count != redisValues.Values.Count)
            {
                List<string> keys = cacheValues.Keys.ToList();
                foreach(var item in redisValues)
                {
                    if (!keys.Contains(item.Key))
                    {
                        cacheValues.Add(item);
                    }
                }
            }
            return cacheValues;
        }

        public override Task<IDictionary<string, CacheValue<T>>> BaseGetByPrefixAsync<T>(string prefix)
        {
            IDictionary<string, CacheValue<T>> cacheValues = _memoryProvider.GetByPrefix<T>(prefix);
            IDictionary<string, CacheValue<T>> redisValues = _redisProvider.GetByPrefix<T>(prefix);
            if (cacheValues.Count > 0 && cacheValues.Count != redisValues.Values.Count)
            {
                List<string> keys = cacheValues.Keys.ToList();
                foreach (var item in redisValues)
                {
                    if (!keys.Contains(item.Key))
                    {
                        cacheValues.Add(item);
                    }
                }
            }
            return Task.FromResult(cacheValues);
        }

        public override int BaseGetCount(string prefix = "")
        {
            return _memoryProvider.GetCount(prefix);
        }

        public override TimeSpan BaseGetExpiration(string cacheKey)
        {
            TimeSpan timeSpan= _memoryProvider.GetExpiration(cacheKey);
            if (timeSpan == TimeSpan.Zero)
            {
                timeSpan = _redisProvider.GetExpiration(cacheKey);
            }
            return timeSpan;
        }

        public override Task<TimeSpan> BaseGetExpirationAsync(string cacheKey)
        {
            TimeSpan timeSpan = _memoryProvider.GetExpiration(cacheKey);
            if (timeSpan == TimeSpan.Zero)
            {
                timeSpan = _redisProvider.GetExpiration(cacheKey);
            }
            return Task.FromResult(timeSpan);
        }

        public override void BaseRefresh<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            _memoryProvider.Refresh(cacheKey, cacheKey, expiration);
            _redisProvider.Refresh(cacheKey, cacheKey, expiration);
        }

        public override Task BaseRefreshAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            _memoryProvider.RefreshAsync(cacheKey, cacheKey, expiration);
            return _redisProvider.RefreshAsync(cacheKey, cacheKey, expiration);
        }

        public override void BaseRemove(string cacheKey)
        {
            _memoryProvider.Remove(cacheKey);
            _redisProvider.Remove(cacheKey);
        }

        public override void BaseRemoveAll(IEnumerable<string> cacheKeys)
        {
            _memoryProvider.RemoveAll(cacheKeys);
            _redisProvider.RemoveAll(cacheKeys);
        }

        public override Task BaseRemoveAllAsync(IEnumerable<string> cacheKeys)
        {
            _memoryProvider.RemoveAllAsync(cacheKeys);
            return _redisProvider.RemoveAllAsync(cacheKeys);
        }

        public override Task BaseRemoveAsync(string cacheKey)
        {
            _memoryProvider.RemoveAsync(cacheKey);
            return _redisProvider.RemoveAsync(cacheKey);
        }

        public override void BaseRemoveByPrefix(string prefix)
        {
            _memoryProvider.RemoveByPrefix(prefix);
            _redisProvider.RemoveByPrefix(prefix);
        }

        public override Task BaseRemoveByPrefixAsync(string prefix)
        {
            _memoryProvider.RemoveByPrefixAsync(prefix);
            return _memoryProvider.RemoveByPrefixAsync(prefix);
        }

        public override void BaseSet<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            _memoryProvider.Set(cacheKey, cacheKey, expiration);
            _redisProvider.Set(cacheKey, cacheKey, expiration);
        }

        public override void BaseSetAll<T>(IDictionary<string, T> values, TimeSpan expiration)
        {
            _memoryProvider.SetAll(values, expiration);
            _redisProvider.SetAll(values, expiration);
        }

        public override Task BaseSetAllAsync<T>(IDictionary<string, T> values, TimeSpan expiration)
        {
            _memoryProvider.SetAllAsync(values, expiration);
            return _redisProvider.SetAllAsync(values, expiration);
        }

        public override Task BaseSetAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            _memoryProvider.Set<T>(cacheKey, cacheValue, expiration);
            return _redisProvider.SetAsync<T>(cacheKey, cacheValue, expiration);
        }

        public override bool BaseTrySet<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            _memoryProvider.TrySet<T>(cacheKey, cacheValue, expiration);
            return _redisProvider.TrySet<T>(cacheKey, cacheValue, expiration);
        }

        public override Task<bool> BaseTrySetAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            _memoryProvider.TrySetAsync<T>(cacheKey, cacheValue, expiration);
            return _redisProvider.TrySetAsync<T>(cacheKey, cacheValue, expiration);
        }
    }
}
