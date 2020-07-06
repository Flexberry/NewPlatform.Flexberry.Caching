namespace NewPlatform.Flexberry.Caching
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Base interface for cache services.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Gets name of cache instance.
        /// Use when multiple cache instances are provided in application.
        /// </summary>
        string CacheName { get; }

        /// <summary>
        /// Default expiration time for cached items (in seconds).
        /// Zero value means data stored without expiration.
        /// </summary>
        int DefaultExpirationTime { get; set; }

        /// <summary>
        /// Returns the total number of cached itmes.
        /// </summary>
        /// <returns>Total number of cached itmes.</returns>
        long GetCount();

        /// <summary>
        /// Returns the total number of tags for cached itmes.
        /// </summary>
        /// <returns>Total number of tags for cached items.</returns>
        long GetTagsCount();

        /// <summary>
        /// Saves data to cache.
        /// </summary>
        /// <remarks>
        /// Data will be stored with <see cref="DefaultExpirationTime"/>.
        /// If item with specified key already exists in cache, it will be replaced with item with new data.
        /// </remarks>
        /// <param name="key">Key of cached item.</param>
        /// <param name="value">Value for caching.</param>
        /// <exception cref="ArgumentNullException">Thrown if key is null.</exception>
        /// <returns>Returns <c>true</c> if data has been saved.</returns>
        bool SetToCache(string key, object value);

        /// <summary>
        /// Saves data to cache.
        /// </summary>
        /// <remarks>
        /// Data will be stored with <see cref="DefaultExpirationTime"/>.
        /// If item with specified key already exists in cache, it will be replaced with item with new data.
        /// </remarks>
        /// <param name="key">Key of cached item.</param>
        /// <param name="value">Value for caching.</param>
        /// <param name="tags">Tags for cached item.</param>
        /// <exception cref="ArgumentNullException">Thrown if key is null.</exception>
        /// <returns>Returns <c>true</c> if data has been saved.</returns>
        bool SetToCache(string key, object value, IList<string> tags);

        /// <summary>
        /// Saves data to cache.
        /// </summary>
        /// <remarks>
        /// If item with specified key already exists in cache, it will be replaced with item with new data.
        /// </remarks>
        /// <param name="key">Key of cached item.</param>
        /// <param name="value">Value for caching.</param>
        /// <param name="expirationTime">Expiration time for stored item (in seconds). Zero value means data stored without expiration.</param>
        /// <exception cref="ArgumentNullException">Thrown if key is null.</exception>
        /// <exception cref="ArgumentException">Thrown if expiration time is negative.</exception>
        /// <returns>Returns <c>true</c> if data has been saved.</returns>
        bool SetToCache(string key, object value, int expirationTime);

        /// <summary>
        /// Saves data to cache.
        /// </summary>
        /// <remarks>
        /// If item with specified key already exists in cache, it will be replaced with item with new data.
        /// </remarks>
        /// <param name="key">Key of cached item.</param>
        /// <param name="value">Value for caching.</param>
        /// <param name="expirationTime">Expiration time for stored item (in seconds). Zero value means data stored without expiration.</param>
        /// <param name="tags">Tags for cached item.</param>
        /// <exception cref="ArgumentNullException">Thrown if key is null.</exception>
        /// <exception cref="ArgumentException">Thrown if expiration time is negative.</exception>
        /// <returns>Returns <c>true</c> if data has been saved.</returns>
        bool SetToCache(string key, object value, int expirationTime, IList<string> tags);

        /// <summary>
        /// Saves data to cache.
        /// </summary>
        /// <remarks>
        /// Data will be stored with <see cref="DefaultExpirationTime"/>.
        /// If item with specified key already exists in cache, it will be replaced with item with new data.
        /// </remarks>
        /// <typeparam name="T">Type of data for storing to cache.</typeparam>
        /// <param name="key">Key of cached item.</param>
        /// <param name="value">Value for caching.</param>
        /// <exception cref="ArgumentNullException">Thrown if key is null.</exception>
        /// <returns>Returns <c>true</c> if data has been saved.</returns>
        bool SetToCache<T>(string key, T value);

        /// <summary>
        /// Saves data to cache.
        /// </summary>
        /// <remarks>
        /// Data will be stored with <see cref="DefaultExpirationTime"/>.
        /// If item with specified key already exists in cache, it will be replaced with item with new data.
        /// </remarks>
        /// <typeparam name="T">Type of data for storing to cache.</typeparam>
        /// <param name="key">Key of cached item.</param>
        /// <param name="value">Value for caching.</param>
        /// <param name="tags">Tags for cached item.</param>
        /// <exception cref="ArgumentNullException">Thrown if key is null.</exception>
        /// <returns>Returns <c>true</c> if data has been saved.</returns>
        bool SetToCache<T>(string key, T value, IList<string> tags);

        /// <summary>
        /// Saves data to cache.
        /// </summary>
        /// <remarks>
        /// If item with specified key already exists in cache, it will be replaced with item with new data.
        /// </remarks>
        /// <typeparam name="T">Type of data for storing to cache.</typeparam>
        /// <param name="key">Key of cached item.</param>
        /// <param name="value">Value for caching.</param>
        /// <param name="expirationTime">Expiration time for stored item (in seconds). Zero value means data stored without expiration.</param>
        /// <exception cref="ArgumentNullException">Thrown if key is null.</exception>
        /// <exception cref="ArgumentException">Thrown if expiration time is negative.</exception>
        /// <returns>Returns <c>true</c> if data has been saved.</returns>
        bool SetToCache<T>(string key, T value, int expirationTime);

        /// <summary>
        /// Saves data to cache.
        /// </summary>
        /// <remarks>
        /// If item with specified key already exists in cache, it will be replaced with item with new data.
        /// </remarks>
        /// <typeparam name="T">Type of data for storing to cache.</typeparam>
        /// <param name="key">Key of cached item.</param>
        /// <param name="value">Value for caching.</param>
        /// <param name="expirationTime">Expiration time for stored item (in seconds). Zero value means data stored without expiration.</param>
        /// <param name="tags">Tags for cached item.</param>
        /// <exception cref="ArgumentNullException">Thrown if key is null.</exception>
        /// <exception cref="ArgumentException">Thrown if expiration time is negative.</exception>
        /// <returns>Returns <c>true</c> if data has been saved.</returns>
        bool SetToCache<T>(string key, T value, int expirationTime, IList<string> tags);

        /// <summary>
        /// Retrieves data from cache.
        /// </summary>
        /// <param name="key">Key of cached item.</param>
        /// <exception cref="ArgumentNullException">Thrown when key is null.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when item is not found in cache.</exception>
        /// <returns>The retrieved cached item.</returns>
        object GetFromCache(string key);

        /// <summary>
        /// Retrieves data from cache by specified tag.
        /// </summary>
        /// <param name="tag">Tag for searching cached items.</param>
        /// <exception cref="ArgumentNullException">Thrown when tag is null.</exception>
        /// <returns>The retrieved cached items.</returns>
        IEnumerable<object> GetFromCacheByTag(string tag);

        /// <summary>
        /// Retrieves data from cache by specified tags.
        /// </summary>
        /// <param name="tags">Tags for searching cached items.</param>
        /// <exception cref="ArgumentNullException">Thrown when tags are null.</exception>
        /// <returns>The retrieved cached items.</returns>
        IEnumerable<object> GetFromCacheByTags(IList<string> tags);

        /// <summary>
        /// Retrieves data from cache.
        /// </summary>
        /// <typeparam name="T">Type of data for getting from cache.</typeparam>
        /// <param name="key">Key of cached item.</param>
        /// <exception cref="ArgumentNullException">Thrown when key is null.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when item is not found in cache.</exception>
        /// <returns>The retrieved cached item.</returns>
        T GetFromCache<T>(string key);

        /// <summary>
        /// Retrieves data from cache by specified tag.
        /// </summary>
        /// <typeparam name="T">Type of data for getting from cache.</typeparam>
        /// <param name="tag">Tag for searching cached items.</param>
        /// <exception cref="ArgumentNullException">Thrown when tag is null.</exception>
        /// <returns>The retrieved cached items.</returns>
        IEnumerable<T> GetFromCacheByTag<T>(string tag);

        /// <summary>
        /// Retrieves data from cache by specified tags.
        /// </summary>
        /// <typeparam name="T">Type of data for getting from cache.</typeparam>
        /// <param name="tags">Tags for searching cached items.</param>
        /// <exception cref="ArgumentNullException">Thrown when tags are null.</exception>
        /// <returns>The retrieved cached items.</returns>
        IEnumerable<T> GetFromCacheByTags<T>(IList<string> tags);

        /// <summary>
        /// Tries to retrieve the data from cache.
        /// </summary>
        /// <param name="key">Key of cached item.</param>
        /// <param name="result">The retrieved cached item or <c>null</c> if item is not found in cache or key is null.</param>
        /// <returns><c>True</c> if operation has been completed successfully and <c>false</c> otherwise.</returns>
        bool TryGetFromCache(string key, out object result);

        /// <summary>
        /// Tries to retrieve the data from cache by specified tag.
        /// </summary>
        /// <remarks>
        /// If there is no items in cache marked by specified tag method will also return <c>true</c>.
        /// </remarks>
        /// <param name="tag">Tag for searching cached items.</param>
        /// <param name="result">The retrieved cached item or empty collection if items are not found in cache.</param>
        /// <exception cref="ArgumentNullException">Thrown when tag is null.</exception>
        /// <returns><c>True</c> if operation has been completed successfully and <c>false</c> otherwise.</returns>
        bool TryGetFromCacheByTag(string tag, out IEnumerable<object> result);

        /// <summary>
        /// Tries to retrieve the data from cache by specified tags.
        /// </summary>
        /// <remarks>
        /// If there is no items in cache marked by specified tags method will also return <c>true</c>.
        /// </remarks>
        /// <param name="tags">Tags for searching cached items.</param>
        /// <param name="result">The retrieved cached items or empty collection if items are not found in cache.</param>
        /// <exception cref="ArgumentNullException">Thrown when tags are null.</exception>
        /// <returns><c>True</c> if operation has been completed successfully and <c>false</c> otherwise.</returns>
        bool TryGetFromCacheByTags(IList<string> tags, out IEnumerable<object> result);

        /// <summary>
        /// Tries to retrieve the data from cache.
        /// </summary>
        /// <typeparam name="T">Type of data for getting from cache.</typeparam>
        /// <param name="key">Key of cached item.</param>
        /// <param name="result">The retrieved cached item or <c>default(T)</c> if if item is not found in cache or key is null.</param>
        /// <returns><c>True</c> if operation has been completed successfully and <c>false</c> otherwise.</returns>
        bool TryGetFromCache<T>(string key, out T result);

        /// <summary>
        /// Tries to retrieve the data from cache by specified tag.
        /// </summary>
        /// <remarks>
        /// If there is no items in cache marked by specified tag method will also return <c>true</c>.
        /// </remarks>
        /// <typeparam name="T">Type of data for getting from cache.</typeparam>
        /// <param name="tag">Tag for searching cached items.</param>
        /// <param name="result">The retrieved cached items or empty collection if items are not found in cache.</param>
        /// <exception cref="ArgumentNullException">Thrown when tag is null.</exception>
        /// <returns><c>True</c> if operation has been completed successfully and <c>false</c> otherwise.</returns>
        bool TryGetFromCacheByTag<T>(string tag, out IEnumerable<T> result);

        /// <summary>
        /// Tries to retrieve the data from cache by specified tags.
        /// </summary>
        /// <remarks>
        /// If there is no items in cache marked by specified tags method will also return <c>true</c>.
        /// </remarks>
        /// <typeparam name="T">Type of data for getting from cache.</typeparam>
        /// <param name="tags">Tags for searching cached items.</param>
        /// <param name="result">The retrieved cached items or empty collection if items are not found in cache.</param>
        /// <exception cref="ArgumentNullException">Thrown when tags are null.</exception>
        /// <returns><c>True</c> if operation has been completed successfully and <c>false</c> otherwise.</returns>
        bool TryGetFromCacheByTags<T>(IList<string> tags, out IEnumerable<T> result);

        /// <summary>
        /// Returns list of tags associated with specified cache item.
        /// </summary>
        /// <param name="key">Key of cached item.</param>
        /// <exception cref="ArgumentNullException">Thrown when key is null.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when item is not found in cache.</exception>
        /// <returns>List of tags associated with specified cache item.</returns>
        IEnumerable<string> GetTagsForItem(string key);

        /// <summary>
        /// Updates existing item in cache.
        /// </summary>
        /// <param name="key">Key of existing cache item.</param>
        /// <param name="value">New value for update in cache item.</param>
        /// <param name="expirationTime">New expiration time for existing cache item (in seconds). Zero value means data stored without expiration.</param>
        /// <param name="tags">Additional list of tags for cached item.</param>
        /// <exception cref="ArgumentNullException">Thrown if key is null.</exception>
        /// <exception cref="ArgumentException">Thrown if expiration time is negative.</exception>
        /// <returns>Returns <c>true</c> if data has been updated.</returns>
        bool UpdateInCache(string key, object value, int expirationTime, IList<string> tags);

        /// <summary>
        /// Updates existing item in cache.
        /// </summary>
        /// <typeparam name="T">Type of data for updating in cache.</typeparam>
        /// <param name="key">Key of existing cache item.</param>
        /// <param name="value">New value for update in cache item.</param>
        /// <param name="expirationTime">New expiration time for existing cache item (in seconds). Zero value means data stored without expiration.</param>
        /// <param name="tags">Additional list of tags for cached item.</param>
        /// <exception cref="ArgumentNullException">Thrown if key is null.</exception>
        /// <exception cref="ArgumentException">Thrown if expiration time is negative.</exception>
        /// <returns>Returns <c>true</c> if data has been saved.</returns>
        bool UpdateInCache<T>(string key, T value, int expirationTime, IList<string> tags);

        /// <summary>
        /// Deletes data from cache.
        /// </summary>
        /// <param name="key">Key of cached item.</param>
        /// <exception cref="ArgumentNullException">Thrown when key is null.</exception>
        /// <returns>Returns <c>true</c> if data has been deleted and <c>false</c> if key is not found in cache.</returns>
        bool DeleteFromCache(string key);

        /// <summary>
        /// Deletes data from cache by specified tag.
        /// </summary>
        /// <param name="tag">Tag of cached items.</param>
        /// <exception cref="ArgumentNullException">Thrown when tag is null.</exception>
        /// <returns>Returns <c>true</c> if data has been deleted and <c>false</c> if no matching data is found in cache.</returns>
        bool DeleteFromCacheByTag(string tag);

        /// <summary>
        /// Deletes data from cache by specified tags.
        /// </summary>
        /// <param name="tags">Tags of cached items.</param>
        /// <exception cref="ArgumentNullException">Thrown when tags are null.</exception>
        /// <returns>Returns <c>true</c> if data has been deleted and <c>false</c> if no matching data is found in cache.</returns>
        bool DeleteFromCacheByTags(IList<string> tags);

        /// <summary>
        /// Clears all stored objects from memory.
        /// </summary>
        void ClearCache();

        /// <summary>
        /// Removes a specified percentage of cache entries from the cache object.
        /// </summary>
        /// <param name="percent">The percentage of total cache entries to remove.</param>
        /// <returns>The number of entries removed from the cache.</returns>
        long Trim(int percent);

        /// <summary>
        /// Check for existing item in cache.
        /// </summary>
        /// <param name="key">Key of cached item.</param>
        /// <returns><c>True</c> if item exists in cache. <c>False</c> if item is not exists in cache or key is null.</returns>
        bool Exists(string key);

        /// <summary>
        /// Check if at least one item with specified tag exists in cache.
        /// </summary>
        /// <param name="tag">Tag of cached items.</param>
        /// <returns><c>True</c> if at least one item with specified tag exists.</returns>
        bool ExistsByTag(string tag);
    }
}
