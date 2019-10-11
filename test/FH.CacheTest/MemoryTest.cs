using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using FH.Cache.Core.InMemory;
using FH.Cache.Core;
using System.Diagnostics;
using System.Threading;

namespace FH.CacheTest
{
    public class MemoryTest: BaseCachingProviderTest
    {
        public MemoryTest()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddMemoryServices(options =>
            {
                options.DBConfig.SizeLimit = 100;
                options.DBConfig.ExpirationScanFrequency = 10;
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
            Assert.False( isDelete);
        }
        [Fact]
        public void ScanFrequency()
        {
            string prefixKey = "demo";
            string prefixValue = "abc";
            for(int i = 1; i < 110; i++)
            {
                _provider.Set<string>(prefixKey + i.ToString("000"), prefixValue + i.ToString(), _defaultTs);
            }
            Trace.WriteLine("Count:{0}", _provider.GetCount().ToString());
            DefaultInMemoryCachingProvider provider = (DefaultInMemoryCachingProvider)_provider;
            foreach (string item in provider.Keys)
            {
                string str = string.Format("Key:{0};Value:{1}", item, _provider.Get<string>(prefixKey));
                Trace.WriteLine(str);
            }
            Thread.Sleep(30000);
            for (int i = 111; i < 130; i++)
            {
                _provider.Set<string>(prefixKey + i.ToString("000"), prefixValue + i.ToString(), _defaultTs);
            }
            Trace.WriteLine("Count:{0}", _provider.GetCount().ToString());
            Thread.Sleep(30000);
            foreach (string item in provider.Keys)
            {
                string str = string.Format("Key:{0};Value:{1}", item, _provider.Get<string>(prefixKey));
                Trace.WriteLine(str);
            }

            Assert.Equal(100, provider.Keys.Count);
        }
    }
}
