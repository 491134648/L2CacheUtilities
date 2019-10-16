using FH.Cache.Core.Configurations;
using FH.Cache.Core.Dashboard;
using FH.Cache.Core.LogProviders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FH.Cache.Core
{
    public static class ServiceCollectionExtensions
    {
        public static void ThrowIfNotConfigured(IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetService<IGlobalConfiguration>();
            if (configuration == null)
            {
                throw new InvalidOperationException(
                    "Unable to find the required services. Please add all the required services by calling 'IServiceCollection.AddHangfire' inside the call to 'ConfigureServices(...)' in the application startup code.");
            }
        }
        public static void AddDashboard(this IServiceCollection services,
            Action<IServiceProvider, IGlobalConfiguration> configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            services.TryAddSingleton(_ => DashboardRoutes.Routes);
            services.AddSingleton<IGlobalConfiguration>(serviceProvider =>
            {
                var configurationInstance = GlobalConfiguration.Configuration;

                // init defaults for log provider and job activator
                // they may be overwritten by the configuration callback later

                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                if (loggerFactory != null)
                {
                    configurationInstance.UseLogProvider(new AspNetCoreLogProvider(loggerFactory));
                }

                //var scopeFactory = serviceProvider.GetService<IServiceScopeFactory>();
                //if (scopeFactory != null)
                //{
                //    configurationInstance.UseActivator(new AspNetCoreJobActivator(scopeFactory));
                //}

                // do configuration inside callback

                configuration(serviceProvider, configurationInstance);

                return configurationInstance;
            });
        }
    }
}
