using Microsoft.Extensions.Options;

namespace FH.Cache.Core.InMemory
{
    public class InMemoryCachingOptions : IOptions<InMemoryCachingOptions>
    {
        /// <summary>
        /// Gets or sets the expiration scan frequency, the unit is second.
        /// </summary>
        /// <value>The expiration scan frequency.</value>
        public int ExpirationScanFrequency { get; set; } = 60;

        /// <summary>
        /// Gets or sets the size limit.
        /// </summary>
        /// <value>The size limit.</value>
        public int SizeLimit { get; set; } = 2048;


        InMemoryCachingOptions IOptions<InMemoryCachingOptions>.Value
        {
            get { return this; }
        }
    }
}
