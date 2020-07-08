namespace NewPlatform.Flexberry.Caching
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Incapsulates information about cache item include value and versions of tags.
    /// </summary>
    internal class CacheItem
    {
        /// <summary>
        /// Gets key of cached item.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets value of cached item.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets dictionary of tags associated with cache item.
        /// </summary>
        public Dictionary<string, int> Tags { get; }

        /// <summary>
        /// Initializes a new <see cref="CacheItem" /> instance using the specified key, value and dictionary of tags (optionaly).
        /// </summary>
        /// <remarks>
        /// Information about every tag includes tag's name and tag's version.
        /// </remarks>
        /// <param name="key">Key of cached item.</param>
        /// <param name="value">Value of cached item.</param>
        /// <param name="tags">Dictionary of tags associated with cached item.</param>
        /// <exception cref="ArgumentException">Throws when tag's name is null or empty or if <paramref name="tags" /> dictionary contains two tags with same name.</exception>
        public CacheItem(string key, object value, IDictionary<string, int> tags = null)
        {
            Key = key;
            Value = value;
            Tags = new Dictionary<string, int>();
            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    if (string.IsNullOrEmpty(tag.Key))
                    {
                        throw new ArgumentException(
                            $"Tag's name can't be null or empty for cache item with key \"{key}\".", nameof(tags));
                    }

                    Tags.Add(tag.Key, tag.Value);
                }
            }
        }
    }

    /// <summary>
    /// Incapsulates information about cache item include value and versions of tags.
    /// </summary>
    /// <typeparam name="T">Type of data for storing to cache.</typeparam>
    internal class CacheItem<T> : CacheItem
    {
        /// <summary>
        /// Gets value of cached item.
        /// </summary>
        public new T Value { get; }

        /// <summary>
        /// Initializes a new <see cref="CacheItem" /> instance using the specified key, value and dictionary of tags (optionaly).
        /// </summary>
        /// <remarks>
        /// Information about every tag includes tag's name and tag's version.
        /// </remarks>
        /// <param name="key">Key of cached item.</param>
        /// <param name="value">Value of cached item.</param>
        /// <param name="tags">Dictionary of tags associated with cached item.</param>
        /// <exception cref="ArgumentException">Throws when tag's name is null or empty or if <paramref name="tags" /> dictionary contains two tags with same name.</exception>
        public CacheItem(string key, T value, IDictionary<string, int> tags = null)
            : base(key, value, tags)
        {
            Value = value;
        }
    }
}
