namespace FH.Cache.Core
{
    /// <summary>
    /// caching const value.
    /// </summary>
    public class CachingConstValue
    {
        /// <summary>
        /// The config section.
        /// </summary>
        public const string ConfigSection = "caching";

        /// <summary>
        /// The redis section.
        /// </summary>
        public const string RedisSection = "caching:redis";

        /// <summary>
        /// The in-memory section.
        /// </summary>
        public const string InMemorySection = "caching:inmemory";

        /// <summary>
        /// The default name of the in-memory.
        /// </summary>
        public const string DefaultInMemoryName = "DefaultInMemory";

        /// <summary>
        /// The default name of the redis.
        /// </summary>
        public const string DefaultRedisName = "DefaultRedis";
    }
}
