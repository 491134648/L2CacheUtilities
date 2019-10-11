using FH.Cache.Core.Configurations;
using FH.Cache.Core.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FH.Cache.Serialization.MessagePack
{
   

    /// <summary>
    /// Message pack options extension.
    /// </summary>
    public static class MessagePackOptionsExtension
    {
        /// <summary>
        /// Adds the services.
        /// </summary>
        /// <param name="services">Services.</param>
        public static IServiceCollection AddMessagePackServices(this IServiceCollection services)
        {
            services.AddSingleton<ICachingSerializer, DefaultMessagePackSerializer>();
            return services;
        }
    }
}
