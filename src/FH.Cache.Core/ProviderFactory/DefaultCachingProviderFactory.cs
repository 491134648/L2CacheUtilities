namespace FH.Cache.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Default easycaching provider factory.
    /// </summary>
    public class DefaultCachingProviderFactory : ICachingProviderFactory
    {
        private readonly IEnumerable<ICachingProvider> _cachingProviders;

        private readonly IEnumerable<IRedisCachingProvider> _redisProviders;

        public DefaultCachingProviderFactory(IEnumerable<ICachingProvider> cachingProviders, IEnumerable<IRedisCachingProvider> redisProviders)
        {
            this._cachingProviders = cachingProviders;
            this._redisProviders = redisProviders;
        }

        public ICachingProvider GetCachingProvider(string name)
        {
            ArgumentCheck.NotNullOrWhiteSpace(name, nameof(name));

            var provider = _cachingProviders.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (provider == null) throw new ArgumentException("can not find a match caching provider!");

            return provider;
        }

        public IRedisCachingProvider GetRedisProvider(string name)
        {
            ArgumentCheck.NotNullOrWhiteSpace(name, nameof(name));

            var provider = _redisProviders.FirstOrDefault(x => x.RedisName.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (provider == null) throw new ArgumentException("can not find a match redis provider!");

            return provider;
        }
    }
}
