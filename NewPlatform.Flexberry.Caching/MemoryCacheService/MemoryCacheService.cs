namespace NewPlatform.Flexberry.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Runtime.Caching;

    /// <summary>
    /// Implementation of <see cref="ICacheService" /> based on <see cref="MemoryCacheService" />.
    /// </summary>
    public partial class MemoryCacheService : ICacheService, IDisposable
    {
        private const string TagPrefix = "_tags/";

        private readonly MemoryCache _cache;

        /// <inheritdoc cref="ICacheService" />
        public int DefaultExpirationTime { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCacheService" /> class.
        /// </summary>
        /// <remarks>
        /// Sets <see cref="DefaultExpirationTime" /> to 0.
        /// </remarks>
        /// <param name="cacheName">The name of cache to use to look up configuration information. If not specified then default <see cref="MemoryCache" /> instance would be used.</param>
        public MemoryCacheService(string cacheName = null)
            : this(cacheName, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCacheService" /> class.
        /// </summary>
        /// <param name="cacheName">The name of cache to use to look up configuration information. If not specified then default <see cref="MemoryCache" /> instance would be used.</param>
        /// <param name="defaultExpirationTime">Default expiration time for items stored in cache (in seconds).</param>
        public MemoryCacheService(string cacheName, int defaultExpirationTime)
            : this(cacheName, defaultExpirationTime, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCacheService" /> class.
        /// </summary>
        /// <param name="cacheName">The name of cache to use to look up configuration information. If not specified then default <see cref="MemoryCache" /> instance would be used.</param>
        /// <param name="defaultExpirationTime">Default expiration time for items stored in cache (in seconds).</param>
        /// <param name="config">A collection of name/value pairs of configuration information to use for configuring the cache. Default value is <see langword="null" />.</param>
        public MemoryCacheService(string cacheName, int defaultExpirationTime, NameValueCollection config)
        {
            _cache = string.IsNullOrEmpty(cacheName) ? MemoryCache.Default : new MemoryCache(cacheName, config);
            DefaultExpirationTime = defaultExpirationTime;
        }

        /// <inheritdoc cref="ICacheService" />
        public bool SetToCache(string key, object value)
        {
            return SetToCache(key, value, DefaultExpirationTime, null);
        }

        /// <inheritdoc cref="ICacheService" />
        public bool SetToCache(string key, object value, IList<string> tags)
        {
            return SetToCache(key, value, DefaultExpirationTime, tags);
        }

        /// <inheritdoc cref="ICacheService" />
        public bool SetToCache(string key, object value, int expirationTime)
        {
            return SetToCache(key, value, expirationTime, null);
        }

        /// <inheritdoc cref="ICacheService" />
        public bool SetToCache(string key, object value, int expirationTime, IList<string> tags)
        {
            if (expirationTime < 0)
            {
                throw new ArgumentException("Expiration time can't be negative.", nameof(expirationTime));
            }

            Dictionary<string, int> tagsDictionary = null;
            if (tags != null && tags.Count > 0)
            {
                tagsDictionary = new Dictionary<string, int>();
                foreach (var tag in tags)
                {
                    var tagKeyForCache = GetKeyForTag(tag);
                    var tagVersion = 1;

                    // If tag's version with specified name is not exists in cache then add it to cache.
                    // Otherwise get current tag's version from cache.
                    if (!_cache.Contains(tagKeyForCache))
                    {
                        // Add tag's version to cache with infinite expiration time.
                        _cache.Set(tagKeyForCache, new TagItem { Version = tagVersion }, GetPolicy(0, null));
                    }
                    else
                    {
                        tagVersion = ((TagItem)_cache.Get(tagKeyForCache)).Version;
                    }

                    tagsDictionary.Add(tag, tagVersion);
                }
            }

            _cache.Set(key, new CacheItem(key, value, tagsDictionary), GetPolicy(expirationTime, tags));
            return true;
        }

        /// <inheritdoc cref="ICacheService" />
        public object GetFromCache(string key)
        {
            var cacheItem = (CacheItem)_cache.Get(key);
            if (cacheItem == null || !IsCacheItemValidated(cacheItem))
            {
                throw new KeyNotFoundException($"Key \"{key}\" is not found in cache.");
            }

            return cacheItem.Value;
        }

        /// <inheritdoc cref="ICacheService" />
        public IEnumerable<object> GetFromCacheByTag(string tag)
        {
            return GetFromCacheByTags(tag == null ? null : new List<string> { tag });
        }

        /// <inheritdoc cref="ICacheService" />
        public IEnumerable<object> GetFromCacheByTags(IList<string> tags)
        {
            if (tags == null)
            {
                throw new ArgumentNullException(nameof(tags));
            }

            var itemsWithSpecifiedTags = GetCacheItemsByTags(tags);

            var itemsWithSpecifiedTagsList = itemsWithSpecifiedTags as IList<CacheItem> ?? itemsWithSpecifiedTags.ToList();
            var itemsWithLatestTagsVersion = itemsWithSpecifiedTagsList.Where(cacheItem =>
            {
                var notInvalidated = cacheItem != null && cacheItem.Tags.All(tag => IsTagValidated(tag.Key, tag.Value));
                return notInvalidated;
            });

            var itemsWithLatestTagsVersionList = itemsWithLatestTagsVersion as IList<CacheItem> ?? itemsWithLatestTagsVersion.ToList();

            // Actually remove each item from cache when it was invalidated by some tag.
            var itemsToRemove = itemsWithSpecifiedTagsList.Except(itemsWithLatestTagsVersionList);
            foreach (var itemToRemove in itemsToRemove)
            {
                _cache.Remove(itemToRemove.Key);
            }

            return itemsWithLatestTagsVersionList.Select(cacheItem => cacheItem.Value);
        }

        /// <inheritdoc cref="ICacheService" />
        public bool TryGetFromCache(string key, out object result)
        {
            try
            {
                result = GetFromCache(key);
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }

        /// <inheritdoc cref="ICacheService" />
        public bool TryGetFromCacheByTag(string tag, out IEnumerable<object> result)
        {
            return TryGetFromCacheByTags(tag == null ? null : new List<string> { tag }, out result);
        }

        /// <inheritdoc cref="ICacheService" />
        public bool TryGetFromCacheByTags(IList<string> tags, out IEnumerable<object> result)
        {
            try
            {
                result = GetFromCacheByTags(tags);
                return true;
            }
            catch (Exception)
            {
                result = new List<object>();
                return false;
            }
        }

        /// <inheritdoc cref="ICacheService" />
        public bool UpdateInCache(string key, object value, int expirationTime, IList<string> tags)
        {
            var cacheItem = (CacheItem)_cache.Get(key);
            var newTagsList = new List<string>();
            IEnumerable<string> oldTagsList = null;
            if (cacheItem != null && IsCacheItemValidated(cacheItem))
            {
                oldTagsList = GetTagsForItem(key);
            }

            if (oldTagsList != null)
            {
                newTagsList.AddRange(oldTagsList);
            }

            if (tags != null)
            {
                newTagsList.AddRange(tags);
            }

            return SetToCache(key, value, expirationTime, newTagsList);
        }
    }
}
