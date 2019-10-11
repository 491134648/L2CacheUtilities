using EasyCaching.Serialization.Protobuf;
using FH.Cache.Core.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace FH.Cache.Serialization.Protobuf
{
    /// <summary>
    /// Message pack options extension.
    /// </summary>
    public static class ProtobufPackOptionsExtension
    {
        /// <summary>
        /// Adds the services.
        /// </summary>
        /// <param name="services">Services.</param>
        public static IServiceCollection AddProtobufPackServices(this IServiceCollection services)
        {
            services.AddSingleton<ICachingSerializer, DefaultProtobufSerializer>();
            return services;
        }
    }
}
