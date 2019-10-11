using FH.Cache.Core;
using FH.Cache.Core.Configurations;
using FH.Cache.Core.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace FH.Cache.Redis
{

    /// <summary>
    /// Redis options extension.
    /// </summary>
    public static class RedisOptionsExtension 
    {



        public static IServiceCollection AddRedisServices(this IServiceCollection services, IConfiguration configuration, string name = CachingConstValue.DefaultRedisName, string sectionName = CachingConstValue.RedisSection)
        {
            var dbConfig = configuration.GetSection(sectionName);
            var redisOptions = new RedisOptions();
            dbConfig.Bind(redisOptions);

            //void configure(RedisOptions x)
            //{
            //    x.CachingProviderType = redisOptions.CachingProviderType;
            //    x.EnableLogging = redisOptions.EnableLogging;
            //    x.MaxRdSecond = redisOptions.MaxRdSecond;
            //    x.Order = redisOptions.Order;
            //    x.DBConfig = redisOptions.DBConfig;
            //}
            return services.AddRedisServices(x=> {
                x.CachingProviderType = redisOptions.CachingProviderType;
                x.EnableLogging = redisOptions.EnableLogging;
                x.MaxRdSecond = redisOptions.MaxRdSecond;
                x.Order = redisOptions.Order;
                x.DBConfig = redisOptions.DBConfig;
            },name);
        }
        /// <summary>
        /// Adds the services.
        /// </summary>
        /// <param name="services">Services.</param>
        public static IServiceCollection AddRedisServices(this IServiceCollection services, Action<RedisOptions> configure, string name = CachingConstValue.DefaultRedisName)
        {
            services.AddOptions();

            services.TryAddSingleton<ICachingSerializer, DefaultBinaryFormatterSerializer>();

            services.Configure(name, configure);

            services.TryAddSingleton<ICachingProviderFactory, DefaultCachingProviderFactory>();
            services.AddSingleton<IRedisDatabaseProvider, RedisDatabaseProvider>(x =>
            {
                var optionsMon = x.GetRequiredService<IOptionsMonitor<RedisOptions>>();
                var options = optionsMon.Get(name);
                return new RedisDatabaseProvider(name, options);
            });

            Func<IServiceProvider, DefaultRedisCachingProvider> createFactory = x =>
            {
                var dbProviders = x.GetServices<IRedisDatabaseProvider>();
                var serializer = x.GetRequiredService<ICachingSerializer>();
                var optionsMon = x.GetRequiredService<IOptionsMonitor<RedisOptions>>();
                var options = optionsMon.Get(name);
                var factory = x.GetService<ILoggerFactory>();
                return new DefaultRedisCachingProvider(name, dbProviders, serializer, options, factory);
            };

            services.AddSingleton<ICachingProvider, DefaultRedisCachingProvider>(createFactory);
            services.AddSingleton<IRedisCachingProvider, DefaultRedisCachingProvider>(createFactory);
            return services;
        }
    }
}
