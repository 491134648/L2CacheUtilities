using FH.Cache.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FH.Cache.Redis.L2Cache
{
    public static class L2Extensions
    {
        /// <summary>
        /// Adds the services.
        /// </summary>
        /// <param name="services">Services.</param>
        public static IServiceCollection AddRedisL2Services(this IServiceCollection services)
        {
           
            
            services.AddSingleton<IL2CacheProvider, RedisAndMemoryProvider>();
            return services;
        }
    }
}
