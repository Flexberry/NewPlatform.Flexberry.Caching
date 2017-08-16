namespace NewPlatform.Flexberry.Caching.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using Xunit;

    /// <summary>
    /// Base class for testing implementations of <see cref="ICacheService"/>.
    /// </summary>
    public class BaseCacheServiceTest : IDisposable
    {
        private readonly ICacheService _cacheService;

        /// <summary>
        /// Flag: Indicates whether "Dispose" has already been called.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Stores initial number of items in cache added by calling <see cref="Initialize"/> method.
        /// Key of dictionary is cache name for cases when multiple caches are used in tests.
        /// </summary>
        protected readonly Dictionary<string, long> InitialNumberOfItemsInCache;

        /// <summary>
        /// Stores initial number of tags in cache added by calling <see cref="Initialize"/> method.
        /// Key of dictionary is cache name for cases when multiple caches are used in tests.
        /// </summary>
        protected readonly Dictionary<string, long> InitialNumberOfTagsInCache;

        public BaseCacheServiceTest(ICacheService cacheService)
        {
            _disposed = false;
            _cacheService = cacheService;
            InitialNumberOfItemsInCache = new Dictionary<string, long>();
            InitialNumberOfTagsInCache = new Dictionary<string, long>();
            Initialize(_cacheService);
        }

        protected ICacheService CacheService
        {
            get
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(null);
                }

                return _cacheService;
            }
        }

        /// <summary>
        /// Clearing cache on destroy.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Clearing cache on destroy.
        /// </summary>
        /// <param name="disposing">Flag: indicates whether method is calling from <see cref="Dispose"/> or not.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _cacheService.ClearCache();
            }

            _disposed = true;
        }

        /// <summary>
        /// Initializes data for tests.
        /// </summary>
        protected void Initialize(ICacheService cacheService)
        {
            // Adding instances of SomeClass to cache.
            var items = new List<SomeClass>
            {
                new SomeClass { Key = "3d10fe87-2f92-4ff2-9e13-fffe2d59dad1", IntValue = 100, StringValue = "String value 1", Tags = new List<string> { "tag1", "tag2" } },
                new SomeClass { Key = "fc3866d1-bf8f-4582-84c5-f8ddf9ae632c", IntValue = 200, StringValue = "String value 2", Tags = new List<string> { "tag1" } },
                new SomeClass { Key = "b4f5ac0e-9766-4198-8914-90947816dcc2", IntValue = 300, StringValue = "String value 3", Tags = new List<string> { "tag1", "tag2", "tag3" } },
                new SomeClass { Key = "e9a8aa9c-a979-4616-a732-58eefdbd7d9e", IntValue = 400, StringValue = "String value 4", Tags = new List<string> { "tag2" } },
                new SomeClass { Key = "d5ba011c-a1bd-4543-ac91-206bb1c6ec3d", IntValue = 500, StringValue = "String value 5", Tags = null },
                new SomeClass { Key = "526f8d7f-e535-48b0-b5c5-fc0c9e155089", IntValue = 600, StringValue = "String value 6", Tags = new List<string> { "tag4" } },
                new SomeClass { Key = "48ced5b3-92b1-40d4-bf5d-d946181c07d1", IntValue = 700, StringValue = "String value 7", Tags = new List<string> { "tag2", "tag3" } },
                new SomeClass { Key = "33ac831c-c2a8-4d49-9e17-5b0030459d10", IntValue = 800, StringValue = "String value 8", Tags = new List<string> { "tag1", "tag3" } },
                new SomeClass { Key = "9f2933b6-5ed4-4069-8ee6-aa041bb15716", IntValue = 900, StringValue = "String value 9", Tags = null },
                new SomeClass { Key = "618f9624-417e-47a1-a53e-d370d8ed8611", IntValue = 1000, StringValue = "String value 10", Tags = new List<string> { "tag1", "tag2" } }
            };

            foreach (var item in items)
            {
                if (!cacheService.SetToCache(item.Key, item, item.Tags))
                {
                    throw new Exception("Error while initializing cache for tests.");
                }
            }

            // Adding objects of anonymous type to cache.
            var objectItems = new List<object>
            {
                new { Key = "e07d5d4f-e531-4e7a-8840-21c2e9482404", Value = 111, Tags = new List<string> { "tag1", "tag5" } },
                new { Key = "33a008b4-3846-4424-ae1b-f7384192046f", Value = 222, Tags = new List<string> { "tag5" } },
                new { Key = "66d79b6a-5225-4a5c-9d2f-f774d484ec05", Value = false, Tags = new List<string>() }
            };

            foreach (var item in objectItems)
            {
                var typeOfItem = item.GetType();
                var key = (string)typeOfItem.GetProperty("Key", typeof(string)).GetValue(item);
                var tags = (List<string>)typeOfItem.GetProperty("Tags", typeof(List<string>)).GetValue(item);
                if (!cacheService.SetToCache(key, item, tags))
                {
                    throw new Exception("Error while initializing cache for tests.");
                }
            }

            InitialNumberOfItemsInCache.Add(cacheService.CacheName, items.Count + objectItems.Count);
            InitialNumberOfTagsInCache.Add(cacheService.CacheName, cacheService.GetTagsCount());
        }
    }
}
