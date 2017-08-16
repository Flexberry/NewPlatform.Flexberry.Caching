namespace NewPlatform.Flexberry.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Incapsulates information about tag to store in cache.
    /// </summary>
    internal class TagItem
    {
        /// <summary>
        /// Version of tag.
        /// </summary>
        public int Version { get; set; }
    }
}
