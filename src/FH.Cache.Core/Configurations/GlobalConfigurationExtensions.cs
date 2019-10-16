using FH.Cache.Core.Logging;
using FH.Cache.Core.LogProviders;
using System;
using System.ComponentModel;

namespace FH.Cache.Core.Configurations
{
    public static class GlobalConfigurationExtensions
    {
        public static IGlobalConfiguration<TLogProvider> UseLogProvider<TLogProvider>(
            this IGlobalConfiguration configuration,
            TLogProvider provider)
           where TLogProvider : ILogProvider
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            return configuration.Use(provider, x => LogProvider.SetCurrentLogProvider(x));
        }

        public static IGlobalConfiguration<NLogLogProvider> UseNLogLogProvider(
             this IGlobalConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return configuration.UseLogProvider(new NLogLogProvider());
        }

        public static IGlobalConfiguration<ColouredConsoleLogProvider> UseColouredConsoleLogProvider(
             this IGlobalConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return configuration.UseLogProvider(new ColouredConsoleLogProvider());
        }

        public static IGlobalConfiguration<ColouredConsoleLogProvider> UseColouredConsoleLogProvider(
             this IGlobalConfiguration configuration,
            LogLevel minLevel)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return configuration.UseLogProvider(new ColouredConsoleLogProvider(minLevel));
        }

        public static IGlobalConfiguration<Log4NetLogProvider> UseLog4NetLogProvider(
             this IGlobalConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return configuration.UseLogProvider(new Log4NetLogProvider());
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IGlobalConfiguration<T> Use<T>(
           this IGlobalConfiguration configuration, T entry,
           Action<T> entryAction)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            entryAction(entry);

            return new ConfigurationEntry<T>(entry);
        }

        private class ConfigurationEntry<T> : IGlobalConfiguration<T>
        {
            public ConfigurationEntry(T entry)
            {
                Entry = entry;
            }

            public T Entry { get; }
        }

    }
}
