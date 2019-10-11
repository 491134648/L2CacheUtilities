namespace FH.Cache.Core
{    
    /// <summary>
    /// Caching provider type.
    /// </summary>
    public enum CachingProviderType
    {
        /// <summary>
        /// 内存缓存
        /// </summary>
        InMemory,
        /// <summary>
        /// redis缓存
        /// </summary>
        Redis,
        /// <summary>
        /// sqllite
        /// </summary>
        SQLite
    }
}
