using FH.Cache.Core;
using FH.Cache.Core.Configurations;

namespace FH.Cache.Redis
{
    
    public class RedisOptions: BaseProviderOptions
    {
        public RedisOptions()
        {
            this.CachingProviderType = CachingProviderType.Redis;
        }
        public RedisDBOptions DBConfig { get; set; } = new RedisDBOptions();
    }
}
