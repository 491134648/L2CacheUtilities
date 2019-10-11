using FH.Cache.Core;
using System;


namespace FH.CacheTest
{
    public abstract class BaseCachingProviderTest
    {
        protected ICachingProvider _provider;
        protected TimeSpan _defaultTs;
        protected string _nameSpace = string.Empty;
    }
}
