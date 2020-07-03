namespace NewPlatform.Flexberry.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Caching;

    public partial class MemoryCacheService
    {
        /// <summary>
        /// Destroying internal MemoryCache.
        /// </summary>
        public void Dispose()
        {
            // If internal cache is default MemoryCache instance then it should not be destroyed!
            if (_cache.Name != MemoryCache.Default.Name)
            {
                _cache.Dispose();
            }
        }

        /// <summary>
        /// Returns internal key for storing versions of tags in cache.
        /// </summary>
        /// <param name="tagName">Tag's name.</param>
        /// <returns>Internal key for storing versions of tags in cache.</returns>
        private string GetKeyForTag(string tagName)
        {
            return TagPrefix + tagName;
        }

        /// <summary>
        /// Checks if tag's version is valid.
        /// </summary>
        /// <param name="tagName">Tag's name.</param>
        /// <param name="tagVersion">Tag's current version for stored item.</param>
        /// <returns><c>True</c> if specified tag version equals tag's version stored in cache and <c>false</c> otherwise.</returns>
        private bool IsTagValidated(string tagName, int tagVersion)
        {
            var tagKeyForCache = GetKeyForTag(tagName);
            var tagVersionFromCache = ((TagItem)_cache.Get(tagKeyForCache)).Version;

            // If version of some tag differs from tag's version from cache
            // then it means that corresponding item was invalidated by current tag.
            return tagVersion == tagVersionFromCache;
        }

        /// <summary>
        /// Returns collection of <c>CacheItem</c> instances for items marked with specified tags.
        /// </summary>
        /// <param name="tags">List of tags for searching items in cache.</param>
        /// <returns>Collection of <c>CacheItem</c> instances for items marked with specified tags.</returns>
        private IEnumerable<CacheItem> GetCacheItemsByTags(IList<string> tags)
        {
            if (tags == null)
            {
                throw new ArgumentNullException(nameof(tags));
            }

            return _cache.Where(item =>
            {
                var cacheItem = item.Value as CacheItem;
                return cacheItem != null && cacheItem.Tags.Keys.Any(tags.Contains);
            }).Select(item => item.Value).Cast<CacheItem>();
        }

        /// <summary>
        /// Returns <c>CacheItemPolicy</c> instance with specified params for new cache item.
        /// </summary>
        /// <param name="expirationTime">Expiration time for stored item (in seconds). Zero value means data stored without expiration.</param>
        /// <param name="tags">Tags for cached item.</param>
        /// <returns>A set of eviction and expiration details.</returns>
        private CacheItemPolicy GetPolicy(int expirationTime, IList<string> tags)
        {
            var dateTimeOffset = expirationTime == 0
                ? ObjectCache.InfiniteAbsoluteExpiration
                : DateTimeOffset.Now.AddSeconds(expirationTime);

            var cip = new CacheItemPolicy
            {
                AbsoluteExpiration = dateTimeOffset,
                SlidingExpiration = ObjectCache.NoSlidingExpiration,
            };

            if (tags == null || tags.Count == 0)
            {
                cip.ChangeMonitors.Add(new SignaledChangeMonitor(_cache.Name));
            }
            else
            {
                foreach (var tag in tags)
                {
                    cip.ChangeMonitors.Add(new SignaledChangeMonitor(_cache.Name, tag));
                }
            }

            return cip;
        }

        /// <summary>
        /// Checks cache item's tags to ensure it is validated.
        /// </summary>
        /// <param name="cacheItem">Instance of <see cref="CacheItem"/> class to check its validity.</param>
        /// <returns><c>True</c> if cache item is validated, i.e. all its tags are valid.</returns>
        private bool IsCacheItemValidated(CacheItem cacheItem)
        {
            var tags = cacheItem.Tags;
            var key = cacheItem.Key;
            if (tags.Any(tag => !IsTagValidated(tag.Key, tag.Value)))
            {
                _cache.Remove(key);
                return false;
            }

            return true;
        }
    }
}
