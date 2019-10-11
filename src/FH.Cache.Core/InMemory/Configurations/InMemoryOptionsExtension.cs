using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace FH.Cache.Core.InMemory
{
   
    /// <summary>
    /// InMemory options extension.
    /// </summary>
    public static class InMemoryOptionsExtension 
    {
        /// <summary>
        /// Uses the in memory.
        /// </summary>
        /// <returns>The in memory.</returns>
        /// <param name="options">Options.</param>
        /// <param name="configuration">Configuration.</param>
        /// <param name="name">Name.</param>
        /// <param name="sectionName">SectionName.</param>
        public static IServiceCollection AddMemoryServices(this IServiceCollection services, IConfiguration configuration, string name = CachingConstValue.DefaultInMemoryName, string sectionName = CachingConstValue.InMemorySection)
        {
            var dbConfig = configuration.GetSection(sectionName);
            var memoryOptions = new InMemoryOptions();
            dbConfig.Bind(memoryOptions);

            //void configure(InMemoryOptions x)
            //{
            //    x.CachingProviderType = memoryOptions.CachingProviderType;
            //    x.EnableLogging = memoryOptions.EnableLogging;
            //    x.MaxRdSecond = memoryOptions.MaxRdSecond;
            //    x.Order = memoryOptions.Order;
            //    x.DBConfig = memoryOptions.DBConfig;
            //}
            return AddMemoryServices(services,x=> {
                x.CachingProviderType = memoryOptions.CachingProviderType;
                x.EnableLogging = memoryOptions.EnableLogging;
                x.MaxRdSecond = memoryOptions.MaxRdSecond;
                x.Order = memoryOptions.Order;
                x.DBConfig = memoryOptions.DBConfig;
            }, name);
        }
        /// <summary>
        /// Adds the services.
        /// </summary>
        /// <param name="services">Services.</param>
        public static IServiceCollection AddMemoryServices(this IServiceCollection services, Action<InMemoryOptions> configure, string name = CachingConstValue.DefaultInMemoryName)
        {
            services.AddOptions();
            services.Configure(name, configure);
            services.AddMemoryCache();
            services.AddSingleton<IInMemoryCaching, InMemoryCaching>(x =>
            {
                var optionsMon = x.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<InMemoryOptions>>();
                var options = optionsMon.Get(name);
                IMemoryCache memoryCache=x.GetRequiredService<IMemoryCache>();
                return new InMemoryCaching(name, options.DBConfig,memoryCache);
            });
            services.TryAddSingleton<ICachingProviderFactory, DefaultCachingProviderFactory>();
            services.AddSingleton<ICachingProvider, DefaultInMemoryCachingProvider>(x =>
            {
                var mCache = x.GetServices<IInMemoryCaching>();
                var optionsMon = x.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<InMemoryOptions>>();
                var options = optionsMon.Get(name);
                //ILoggerFactory can be null
                var factory = x.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();
                return new DefaultInMemoryCachingProvider(name, mCache, options, factory);
            });
            return services;

        }
    }
}