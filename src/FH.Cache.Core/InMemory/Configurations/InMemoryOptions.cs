using FH.Cache.Core.Configurations;

namespace FH.Cache.Core.InMemory
{
    public class InMemoryOptions : BaseProviderOptions
    {
        public InMemoryOptions()
        {
            this.CachingProviderType = CachingProviderType.InMemory;
        }
        public InMemoryCachingOptions DBConfig { get; set; } = new InMemoryCachingOptions();
    }
}
