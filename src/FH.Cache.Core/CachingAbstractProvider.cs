﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FH.Cache.Core.Diagnostics;

namespace FH.Cache.Core
{
    /// <summary>
    /// Caching abstract provider
    /// </summary>
    public abstract class CachingAbstractProvider : ICachingProvider
    {
        protected static readonly DiagnosticListener s_diagnosticListener =
                    new DiagnosticListener(CachingDiagnosticListenerExtensions.DiagnosticListenerName);
        /// <summary>
        /// 服务名称
        /// </summary>
        protected string ProviderName { get; set; }
        /// <summary>
        /// 是否分布式缓存
        /// </summary>
        protected bool IsDistributedProvider { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        protected int ProviderOrder { get; set; }
        /// <summary>
        /// 预防缓存在同一时间全部失效，可以为每个key的过期时间添加一个随机的秒数，默认值是120秒
        /// </summary>
        protected int ProviderMaxRdSecond { get; set; }
        /// <summary>
        /// 提供对象
        /// </summary>
        protected CachingProviderType ProviderType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        protected CacheStats ProviderStats { get; set; }

        public string Name => this.ProviderName;
        public bool IsDistributedCache => this.IsDistributedProvider;
        public int Order => this.ProviderOrder;
        public int MaxRdSecond => this.ProviderMaxRdSecond;
        public CachingProviderType CachingProviderType => this.ProviderType;
        public CacheStats CacheStats => this.ProviderStats;
        /// <summary>
        /// 是否存在相关缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public abstract bool BaseExists(string cacheKey);
        /// <summary>
        /// 异步获取是否存在缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public abstract Task<bool> BaseExistsAsync(string cacheKey);
        /// <summary>
        /// 
        /// </summary>
        public abstract void BaseFlush();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract Task BaseFlushAsync();
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="dataRetriever"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public abstract CacheValue<T> BaseGet<T>(string cacheKey, Func<T> dataRetriever, TimeSpan expiration);
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public abstract CacheValue<T> BaseGet<T>(string cacheKey);
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKeys"></param>
        /// <returns></returns>
        public abstract IDictionary<string, CacheValue<T>> BaseGetAll<T>(IEnumerable<string> cacheKeys);
        public abstract Task<IDictionary<string, CacheValue<T>>> BaseGetAllAsync<T>(IEnumerable<string> cacheKeys);
        public abstract Task<CacheValue<T>> BaseGetAsync<T>(string cacheKey, Func<Task<T>> dataRetriever, TimeSpan expiration);
        public abstract Task<object> BaseGetAsync(string cacheKey, Type type);
        public abstract Task<CacheValue<T>> BaseGetAsync<T>(string cacheKey);
        public abstract IDictionary<string, CacheValue<T>> BaseGetByPrefix<T>(string prefix);
        public abstract Task<IDictionary<string, CacheValue<T>>> BaseGetByPrefixAsync<T>(string prefix);
        /// <summary>
        /// 获取缓存总数
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public abstract int BaseGetCount(string prefix = "");
        public abstract void BaseRefresh<T>(string cacheKey, T cacheValue, TimeSpan expiration);
        public abstract Task BaseRefreshAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration);
        public abstract void BaseRemove(string cacheKey);
        public abstract void BaseRemoveAll(IEnumerable<string> cacheKeys);
        public abstract Task BaseRemoveAllAsync(IEnumerable<string> cacheKeys);
        public abstract Task BaseRemoveAsync(string cacheKey);
        public abstract void BaseRemoveByPrefix(string prefix);
        public abstract Task BaseRemoveByPrefixAsync(string prefix);
        public abstract void BaseSet<T>(string cacheKey, T cacheValue, TimeSpan expiration);
        public abstract void BaseSetAll<T>(IDictionary<string, T> values, TimeSpan expiration);
        public abstract Task BaseSetAllAsync<T>(IDictionary<string, T> values, TimeSpan expiration);
        public abstract Task BaseSetAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration);
        public abstract bool BaseTrySet<T>(string cacheKey, T cacheValue, TimeSpan expiration);
        public abstract Task<bool> BaseTrySetAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration);

        public abstract TimeSpan BaseGetExpiration(string cacheKey);
        public abstract Task<TimeSpan> BaseGetExpirationAsync(string cacheKey);

        public bool Exists(string cacheKey)
        {
            var operationId = s_diagnosticListener.WriteExistsCacheBefore(new BeforeExistsRequestEventData(CachingProviderType.ToString(), Name, nameof(Exists), cacheKey));
            Exception e = null;
            try
            {
                return BaseExists(cacheKey);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteExistsCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteExistsCacheAfter(operationId);
                }
            }
        }
        public async Task<bool> ExistsAsync(string cacheKey)
        {
            var operationId = s_diagnosticListener.WriteExistsCacheBefore(new BeforeExistsRequestEventData(CachingProviderType.ToString(), Name, nameof(ExistsAsync), cacheKey));
            Exception e = null;
            try
            {
                var flag = await BaseExistsAsync(cacheKey);
                return flag;
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteExistsCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteExistsCacheAfter(operationId);
                }
            }
        }

        public void Flush()
        {
            var operationId = s_diagnosticListener.WriteFlushCacheBefore(new EventData(CachingProviderType.ToString(), Name, nameof(Flush)));
            Exception e = null;
            try
            {
                BaseFlush();
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteFlushCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteFlushCacheAfter(operationId);
                }
            }
        }

        public async Task FlushAsync()
        {
            var operationId = s_diagnosticListener.WriteFlushCacheBefore(new EventData(CachingProviderType.ToString(), Name, nameof(FlushAsync)));
            Exception e = null;
            try
            {
                await BaseFlushAsync();
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteFlushCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteFlushCacheAfter(operationId);
                }
            }
        }

        public CacheValue<T> Get<T>(string cacheKey, Func<T> dataRetriever, TimeSpan expiration)
        {
            var operationId = s_diagnosticListener.WriteGetCacheBefore(new BeforeGetRequestEventData(CachingProviderType.ToString(), Name, nameof(Get), new[] { cacheKey }, expiration));
            Exception e = null;
            try
            {
                return BaseGet(cacheKey, dataRetriever, expiration);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteGetCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteGetCacheAfter(operationId);
                }
            }
        }

        public CacheValue<T> Get<T>(string cacheKey)
        {
            var operationId = s_diagnosticListener.WriteGetCacheBefore(new BeforeGetRequestEventData(CachingProviderType.ToString(), Name, nameof(Get), new[] { cacheKey }));
            Exception e = null;
            try
            {
                return BaseGet<T>(cacheKey);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteGetCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteGetCacheAfter(operationId);
                }
            }
        }

        public IDictionary<string, CacheValue<T>> GetAll<T>(IEnumerable<string> cacheKeys)
        {
            var operationId = s_diagnosticListener.WriteGetCacheBefore(new BeforeGetRequestEventData(CachingProviderType.ToString(), Name, nameof(GetAll), cacheKeys.ToArray()));
            Exception e = null;
            try
            {
                return BaseGetAll<T>(cacheKeys);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteGetCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteGetCacheAfter(operationId);
                }
            }
        }

        public async Task<IDictionary<string, CacheValue<T>>> GetAllAsync<T>(IEnumerable<string> cacheKeys)
        {
            var operationId = s_diagnosticListener.WriteGetCacheBefore(new BeforeGetRequestEventData(CachingProviderType.ToString(), Name, nameof(GetAllAsync), cacheKeys.ToArray()));
            Exception e = null;
            try
            {
                return await BaseGetAllAsync<T>(cacheKeys);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteGetCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteGetCacheAfter(operationId);
                }
            }
        }

        public async Task<CacheValue<T>> GetAsync<T>(string cacheKey, Func<Task<T>> dataRetriever, TimeSpan expiration)
        {
            var operationId = s_diagnosticListener.WriteGetCacheBefore(new BeforeGetRequestEventData(CachingProviderType.ToString(), Name, nameof(GetAsync), new[] { cacheKey }, expiration));
            Exception e = null;
            try
            {
                return await BaseGetAsync<T>(cacheKey, dataRetriever, expiration);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteGetCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteGetCacheAfter(operationId);
                }
            }
        }

        public async Task<object> GetAsync(string cacheKey, Type type)
        {
            var operationId = s_diagnosticListener.WriteGetCacheBefore(new BeforeGetRequestEventData(CachingProviderType.ToString(), Name, "GetAsync_Type", new[] { cacheKey }));
            Exception e = null;
            try
            {
                return await BaseGetAsync(cacheKey, type);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteGetCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteGetCacheAfter(operationId);
                }
            }
        }

        public async Task<CacheValue<T>> GetAsync<T>(string cacheKey)
        {
            var operationId = s_diagnosticListener.WriteGetCacheBefore(new BeforeGetRequestEventData(CachingProviderType.ToString(), Name, nameof(GetAsync), new[] { cacheKey }));
            Exception e = null;
            try
            {
                return await BaseGetAsync<T>(cacheKey);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteGetCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteGetCacheAfter(operationId);
                }
            }
        }

        public IDictionary<string, CacheValue<T>> GetByPrefix<T>(string prefix)
        {
            var operationId = s_diagnosticListener.WriteGetCacheBefore(new BeforeGetRequestEventData(CachingProviderType.ToString(), Name, nameof(GetByPrefix), new[] { prefix }));
            Exception e = null;
            try
            {
                return BaseGetByPrefix<T>(prefix);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteGetCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteGetCacheAfter(operationId);
                }
            }
        }

        public async Task<IDictionary<string, CacheValue<T>>> GetByPrefixAsync<T>(string prefix)
        {
            var operationId = s_diagnosticListener.WriteGetCacheBefore(new BeforeGetRequestEventData(CachingProviderType.ToString(), Name, nameof(GetByPrefixAsync), new[] { prefix }));
            Exception e = null;
            try
            {
                return await BaseGetByPrefixAsync<T>(prefix);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteGetCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteGetCacheAfter(operationId);
                }
            }
        }

        public int GetCount(string prefix = "")
        {
            return BaseGetCount(prefix);
        }

        public void Refresh<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            var operationId = s_diagnosticListener.WriteSetCacheBefore(new BeforeSetRequestEventData(CachingProviderType.ToString(), Name, nameof(Refresh), new Dictionary<string, object> { { cacheKey, cacheValue } }, expiration));
            Exception e = null;
            try
            {
                BaseRefresh(cacheKey, cacheValue, expiration);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteSetCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteSetCacheAfter(operationId);
                }
            }
        }

        public async Task RefreshAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            var operationId = s_diagnosticListener.WriteSetCacheBefore(new BeforeSetRequestEventData(CachingProviderType.ToString(), Name, nameof(RefreshAsync), new Dictionary<string, object> { { cacheKey, cacheValue } }, expiration));
            Exception e = null;
            try
            {
                await BaseRefreshAsync(cacheKey, cacheValue, expiration);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteSetCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteSetCacheAfter(operationId);
                }
            }
        }

        public void Remove(string cacheKey)
        {
            var operationId = s_diagnosticListener.WriteRemoveCacheBefore(new BeforeRemoveRequestEventData(CachingProviderType.ToString(), Name, nameof(Remove), new[] { cacheKey }));
            Exception e = null;
            try
            {
                BaseRemove(cacheKey);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteRemoveCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteRemoveCacheAfter(operationId);
                }
            }
        }

        public void RemoveAll(IEnumerable<string> cacheKeys)
        {
            var operationId = s_diagnosticListener.WriteRemoveCacheBefore(new BeforeRemoveRequestEventData(CachingProviderType.ToString(), Name, nameof(RemoveAll), cacheKeys.ToArray()));
            Exception e = null;
            try
            {
                BaseRemoveAll(cacheKeys);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteRemoveCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteRemoveCacheAfter(operationId);
                }
            }
        }

        public async Task RemoveAllAsync(IEnumerable<string> cacheKeys)
        {
            var operationId = s_diagnosticListener.WriteRemoveCacheBefore(new BeforeRemoveRequestEventData(CachingProviderType.ToString(), Name, nameof(RemoveAllAsync), cacheKeys.ToArray()));
            Exception e = null;
            try
            {
                await BaseRemoveAllAsync(cacheKeys);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteRemoveCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteRemoveCacheAfter(operationId);
                }
            }
        }

        public async Task RemoveAsync(string cacheKey)
        {
            var operationId = s_diagnosticListener.WriteRemoveCacheBefore(new BeforeRemoveRequestEventData(CachingProviderType.ToString(), Name, nameof(RemoveAsync), new[] { cacheKey }));
            Exception e = null;
            try
            {
                await BaseRemoveAsync(cacheKey);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteRemoveCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteRemoveCacheAfter(operationId);
                }
            }
        }

        public void RemoveByPrefix(string prefix)
        {
            var operationId = s_diagnosticListener.WriteRemoveCacheBefore(new BeforeRemoveRequestEventData(CachingProviderType.ToString(), Name, nameof(RemoveByPrefix), new[] { prefix }));
            Exception e = null;
            try
            {
                BaseRemoveByPrefix(prefix);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteRemoveCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteRemoveCacheAfter(operationId);
                }
            }
        }

        public async Task RemoveByPrefixAsync(string prefix)
        {
            var operationId = s_diagnosticListener.WriteRemoveCacheBefore(new BeforeRemoveRequestEventData(CachingProviderType.ToString(), Name, nameof(RemoveByPrefixAsync), new[] { prefix }));
            Exception e = null;
            try
            {
                await BaseRemoveByPrefixAsync(prefix);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteRemoveCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteRemoveCacheAfter(operationId);
                }
            }
        }

        public void Set<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            var operationId = s_diagnosticListener.WriteSetCacheBefore(new BeforeSetRequestEventData(CachingProviderType.ToString(), Name, nameof(Set), new Dictionary<string, object> { { cacheKey, cacheValue } }, expiration));
            Exception e = null;
            try
            {
                BaseSet(cacheKey, cacheValue, expiration);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteSetCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteSetCacheAfter(operationId);
                }
            }
        }

        public void SetAll<T>(IDictionary<string, T> value, TimeSpan expiration)
        {
            var operationId = s_diagnosticListener.WriteSetCacheBefore(new BeforeSetRequestEventData(CachingProviderType.ToString(), Name, nameof(SetAll), value.ToDictionary(k => k.Key, v => (object)v.Value), expiration));
            Exception e = null;
            try
            {
                BaseSetAll(value, expiration);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteSetCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteSetCacheAfter(operationId);
                }
            }
        }

        public async Task SetAllAsync<T>(IDictionary<string, T> value, TimeSpan expiration)
        {
            var operationId = s_diagnosticListener.WriteSetCacheBefore(new BeforeSetRequestEventData(CachingProviderType.ToString(), Name, nameof(SetAllAsync), value.ToDictionary(k => k.Key, v => (object)v.Value), expiration));
            Exception e = null;
            try
            {
                await BaseSetAllAsync(value, expiration);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteSetCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteSetCacheAfter(operationId);
                }
            }
        }

        public async Task SetAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            var operationId = s_diagnosticListener.WriteSetCacheBefore(new BeforeSetRequestEventData(CachingProviderType.ToString(), Name, nameof(SetAsync), new Dictionary<string, object> { { cacheKey, cacheValue } }, expiration));
            Exception e = null;
            try
            {
                await BaseSetAsync(cacheKey, cacheValue, expiration);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteSetCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteSetCacheAfter(operationId);
                }
            }
        }

        public bool TrySet<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            var operationId = s_diagnosticListener.WriteSetCacheBefore(new BeforeSetRequestEventData(CachingProviderType.ToString(), Name, nameof(TrySet), new Dictionary<string, object> { { cacheKey, cacheValue } }, expiration));
            Exception e = null;
            try
            {
                return BaseTrySet(cacheKey, cacheValue, expiration);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteSetCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteSetCacheAfter(operationId);
                }
            }
        }

        public async Task<bool> TrySetAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            var operationId = s_diagnosticListener.WriteSetCacheBefore(new BeforeSetRequestEventData(CachingProviderType.ToString(), Name, nameof(TrySetAsync), new Dictionary<string, object> { { cacheKey, cacheValue } }, expiration));
            Exception e = null;
            try
            {
                return await BaseTrySetAsync(cacheKey, cacheValue, expiration);
            }
            catch (Exception ex)
            {
                e = ex;
                throw;
            }
            finally
            {
                if (e != null)
                {
                    s_diagnosticListener.WriteSetCacheError(operationId, e);
                }
                else
                {
                    s_diagnosticListener.WriteSetCacheAfter(operationId);
                }
            }
        }

        public TimeSpan GetExpiration(string cacheKey)
        {
            return BaseGetExpiration(cacheKey);
        }

        public async Task<TimeSpan> GetExpirationAsync(string cacheKey)
        {
            return await BaseGetExpirationAsync(cacheKey);
        }        
    }
}
