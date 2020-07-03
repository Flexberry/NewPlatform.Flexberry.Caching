namespace NewPlatform.Flexberry.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Caching;

    public partial class MemoryCacheService
    {
        /// <inheritdoc cref="ICacheService"/>
        public string CacheName => _cache?.Name;

        /// <inheritdoc cref="ICacheService"/>
        /// <remarks>
        /// Information about each tag is also stored in cache.
        /// So total number of cached items will include corresponding count of items with information about tags.
        /// </remarks>
        public long GetCount()
        {
            return _cache.GetCount();
        }

        /// <inheritdoc cref="ICacheService"/>
        /// <remarks>
        /// Information about tags deleted from cache only when cahce is totally clearing.
        /// So cache will store infromation about ever added tags until chache is totally cleared.
        /// </remarks>
        public long GetTagsCount()
        {
            return _cache.Count(item => item.Value is TagItem);
        }

        /// <inheritdoc cref="ICacheService"/>
        public IEnumerable<string> GetTagsForItem(string key)
        {
            var cacheItem = (CacheItem)_cache.Get(key);
            if (cacheItem == null || !IsCacheItemValidated(cacheItem))
            {
                throw new KeyNotFoundException($"Key \"{key}\" is not found in cache.");
            }

            return cacheItem.Tags.Select(tags => tags.Key).ToList();
        }

        /// <inheritdoc cref="ICacheService"/>
        public bool DeleteFromCache(string key)
        {
            return _cache.Remove(key) != null;
        }

        /// <inheritdoc cref="ICacheService"/>
        public bool DeleteFromCacheByTag(string tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            var tagKeyForCache = GetKeyForTag(tag);
            if (_cache.Contains(tagKeyForCache))
            {
                // Increasing tag's version means invalidation of
                var currentTagVersion = ((TagItem)_cache.Get(tagKeyForCache)).Version;
                var newTagVersion = currentTagVersion + 1;
                _cache.Set(tagKeyForCache, new TagItem { Version = newTagVersion }, ObjectCache.InfiniteAbsoluteExpiration);

                // Flush cached items associated with tag's change monitors.
                SignaledChangeMonitor.Signal(_cache.Name, tag);

                return true;
            }

            return false;
        }

        /// <inheritdoc cref="ICacheService"/>
        public bool DeleteFromCacheByTags(IList<string> tags)
        {
            if (tags == null)
            {
                throw new ArgumentNullException(nameof(tags));
            }

            var anythingHasBeenDeletedFromCache = false;
            foreach (var tag in tags)
            {
                if (DeleteFromCacheByTag(tag))
                {
                    anythingHasBeenDeletedFromCache = true;
                }
            }

            return anythingHasBeenDeletedFromCache;
        }

        /// <inheritdoc cref="ICacheService"/>
        public void ClearCache()
        {
            // Flush all cached items.
            SignaledChangeMonitor.Signal(_cache.Name);
        }

        /// <inheritdoc cref="ICacheService"/>
        public bool Exists(string key)
        {
            object cacheItem;
            return TryGetFromCache(key, out cacheItem) && _cache.Contains(key);
        }

        /// <inheritdoc cref="ICacheService"/>
        public bool ExistsByTag(string tag)
        {
            var cacheItems = GetCacheItemsByTags(tag == null ? new List<string>() : new List<string> { tag });
            var cacheItemsList = cacheItems as IList<CacheItem> ?? cacheItems.ToList();
            if (!cacheItemsList.Any())
            {
                return false;
            }

            var exists = false;
            foreach (var cacheItem in cacheItemsList)
            {
                // If any item in cache was invalidated by some tag then it will be actually removed from cache.
                if (Exists(cacheItem.Key))
                {
                    exists = true;
                }
            }

            return exists;
        }
    }
}
