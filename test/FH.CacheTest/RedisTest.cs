using System;
using FH.Cache.Core;
using FH.Cache.Redis;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FH.CacheTest
{
    public class RedisTest : BaseCachingProviderTest
    {
        public RedisTest()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddRedisServices(options =>
            {
                options.DBConfig.Password = "";
                options.DBConfig.Endpoints.Add(new Cache.Core.Configurations.ServerEndPoint()
                {
                    Port=6379,
                    Host= "127.0.0.1"
                });
            });
            services.AddLogging();
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            _provider = serviceProvider.GetService<ICachingProvider>();
            _defaultTs = TimeSpan.FromSeconds(600);
        }
        [Fact]
        public void CRUD()
        {
            _provider.Set("123456", "987654321", _defaultTs);
            string result = _provider.Get<string>("123456").Value;
            Assert.Equal("987654321", result);
            _provider.Set("123456", "987654", _defaultTs);
            result = _provider.Get<string>("123456").Value;
            Assert.Equal("987654", result);
            _provider.Remove("123456");
            bool isDelete = _provider.Get<string>("123456").HasValue;
            Assert.False(isDelete);
        }
    }
}
