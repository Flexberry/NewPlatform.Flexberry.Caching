namespace NewPlatform.Flexberry.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Text;
    using System.Threading.Tasks;

    public partial class MemoryCacheService
    {
        /// <inheritdoc cref="ICacheService"/>
        public bool SetToCache<T>(string key, T value)
        {
            return SetToCache(key, value, DefaultExpirationTime, null);
        }

        /// <inheritdoc cref="ICacheService"/>
        public bool SetToCache<T>(string key, T value, IList<string> tags)
        {
            return SetToCache(key, value, DefaultExpirationTime, tags);
        }

        /// <inheritdoc cref="ICacheService"/>
        public bool SetToCache<T>(string key, T value, int expirationTime)
        {
            return SetToCache(key, value, expirationTime, null);
        }

        /// <inheritdoc cref="ICacheService"/>
        public bool SetToCache<T>(string key, T value, int expirationTime, IList<string> tags)
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

            _cache.Set(key, new CacheItem<T>(key, value, tagsDictionary), GetPolicy(expirationTime, tags));
            return true;
        }

        /// <inheritdoc cref="ICacheService"/>
        public T GetFromCache<T>(string key)
        {
            var cacheItem = (CacheItem<T>)_cache.Get(key);
            if (cacheItem == null || !IsCacheItemValidated(cacheItem))
            {
                throw new KeyNotFoundException($"Key \"{key}\" is not found in cache.");
            }

            return cacheItem.Value;
        }

        /// <inheritdoc cref="ICacheService"/>
        public IEnumerable<T> GetFromCacheByTag<T>(string tag)
        {
            return GetFromCacheByTags<T>(tag == null ? null : new List<string> { tag });
        }

        /// <inheritdoc cref="ICacheService"/>
        public IEnumerable<T> GetFromCacheByTags<T>(IList<string> tags)
        {
            if (tags == null)
            {
                throw new ArgumentNullException(nameof(tags));
            }

            var itemsWithSpecifiedTags = GetCacheItemsByTags(tags).Cast<CacheItem<T>>();

            var itemsWithSpecifiedTagsList = itemsWithSpecifiedTags as IList<CacheItem<T>> ?? itemsWithSpecifiedTags.ToList();
            var itemsWithLatestTagsVersion = itemsWithSpecifiedTagsList.Where(cacheItem =>
            {
                var notInvalidated = cacheItem != null && cacheItem.Tags.All(tag => IsTagValidated(tag.Key, tag.Value));
                return notInvalidated;
            });

            var itemsWithLatestTagsVersionList = itemsWithLatestTagsVersion as IList<CacheItem<T>> ?? itemsWithLatestTagsVersion.ToList();

            // Actually remove each item from cache when it was invalidated by some tag.
            var itemsToRemove = itemsWithSpecifiedTagsList.Except(itemsWithLatestTagsVersionList);
            foreach (var itemToRemove in itemsToRemove)
            {
                _cache.Remove(itemToRemove.Key);
            }

            return itemsWithLatestTagsVersionList.Select(cacheItem => cacheItem.Value);
        }

        /// <inheritdoc cref="ICacheService"/>
        public bool TryGetFromCache<T>(string key, out T result)
        {
            try
            {
                result = GetFromCache<T>(key);
                return true;
            }
            catch (Exception)
            {
                result = default(T);
                return false;
            }
        }

        /// <inheritdoc cref="ICacheService"/>
        public bool TryGetFromCacheByTag<T>(string tag, out IEnumerable<T> result)
        {
            return TryGetFromCacheByTags(tag == null ? null : new List<string> { tag }, out result);
        }

        /// <inheritdoc cref="ICacheService"/>
        public bool TryGetFromCacheByTags<T>(IList<string> tags, out IEnumerable<T> result)
        {
            try
            {
                result = GetFromCacheByTags<T>(tags);
                return true;
            }
            catch (Exception)
            {
                result = new List<T>();
                return false;
            }
        }

        /// <inheritdoc cref="ICacheService"/>
        public bool UpdateInCache<T>(string key, T value, int expirationTime, IList<string> tags)
        {
            var cacheItem = (CacheItem<T>)_cache.Get(key);
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
