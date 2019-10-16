using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace FH.Cache.Core.Configurations
{
    public enum CompatibilityLevel
    {
        Version_110 = 110,
        Version_170 = 170,
    }

    public class GlobalConfiguration : IGlobalConfiguration
    {
        private static int _compatibilityLevel = (int)CompatibilityLevel.Version_110;

        public static IGlobalConfiguration Configuration { get; } = new GlobalConfiguration();

        internal static CompatibilityLevel CompatibilityLevel
        {
            get => (CompatibilityLevel)Volatile.Read(ref _compatibilityLevel);
            set => Volatile.Write(ref _compatibilityLevel, (int)value);
        }

        internal static bool HasCompatibilityLevel(CompatibilityLevel level)
        {
            return CompatibilityLevel >= level;
        }

        internal GlobalConfiguration()
        {
        }
    }

    public static class CompatibilityLevelExtensions
    {
        public static IGlobalConfiguration SetDataCompatibilityLevel(
            this IGlobalConfiguration configuration,
            CompatibilityLevel compatibilityLevel)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

#if !NETSTANDARD1_3
            if (!Enum.IsDefined(typeof(CompatibilityLevel), compatibilityLevel))
                throw new InvalidEnumArgumentException(nameof(compatibilityLevel), (int)compatibilityLevel,
                    typeof(CompatibilityLevel));
#endif

            GlobalConfiguration.CompatibilityLevel = compatibilityLevel;

            return configuration;
        }
    }
}
