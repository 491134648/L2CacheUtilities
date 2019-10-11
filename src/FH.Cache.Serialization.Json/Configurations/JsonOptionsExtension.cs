using FH.Cache.Core.Configurations;
using FH.Cache.Core.Serialization;
using FH.Cache.Serialization.Json;
using FH.Cache.Serialization.Json.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FH.Cache.Serialization.MessagePack
{
   

    /// <summary>
    /// Message pack options extension.
    /// </summary>
    public static class JsonOptionsExtension
    {
        /// <summary>
        /// Adds the services.
        /// </summary>
        /// <param name="services">Services.</param>
        public static IServiceCollection AddJsonPackServices(this IServiceCollection services, string name = "json", Action<CachingJsonSerializerOptions> configure=null)
        {
            if (configure == null)
            {
                configure= x => { }; ;
            }
            services.AddOptions();
            services.Configure(name, configure);
            services.AddSingleton<ICachingSerializer, DefaultJsonSerializer>(x =>
            {
                var optionsMon = x.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<CachingJsonSerializerOptions>>();
                var options = optionsMon.Get(name);
                return new DefaultJsonSerializer(name, options);
            });
            return services;
        }
    }
}
