using Microsoft.Extensions.DependencyInjection;
using System;
using FH.Cache.Redis;
using FH.Cache.Core.InMemory;
using FH.Cache.Core;
using FH.Cache.Redis.L2Cache;
using System.Diagnostics;

using Xunit;


namespace FH.CacheTest
{
    public class L2CacheTest : BaseCachingProviderTest
    {
        private readonly ICachingProvider _redis;
        private readonly IL2CacheProvider _l2CacheProvider;
        public L2CacheTest()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddMemoryServices(options =>
            {
                options.DBConfig.SizeLimit = 100;
                options.DBConfig.ExpirationScanFrequency = 10;
            });
            services.AddRedisServices(options =>
            {
                options.DBConfig.Password = "Chudian258##987";
                options.DBConfig.Endpoints.Add(new Cache.Core.Configurations.ServerEndPoint()
                {
                    Port = 6379,
                    Host = "r-wz952aaada291544pd.redis.rds.aliyuncs.com"
                });
            }).AddRedisL2Services()
            ;
            services.AddLogging();
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            ICachingProviderFactory _factory = serviceProvider.GetService<ICachingProviderFactory>();
            _provider = _factory.GetCachingProvider(CachingConstValue.DefaultInMemoryName);
            _redis = _factory.GetCachingProvider(CachingConstValue.DefaultRedisName);
            _l2CacheProvider= serviceProvider.GetService<IL2CacheProvider>();
            _defaultTs = TimeSpan.FromSeconds(600);
        }
        [Fact]
        public void CRUD()
        {
            _redis.Set("123456", "987654321", _defaultTs);
            CacheValue<string> memoryData = _provider.Get<string>("123456");
            Assert.False(memoryData.HasValue);
            memoryData=_l2CacheProvider.Get<string>("123456");
            Trace.WriteLine(memoryData.Value);
            memoryData = _provider.Get<string>("123456");
            Assert.Equal("987654321",memoryData.Value);
            Assert.True(memoryData.HasValue);
           
        }
    }
}
