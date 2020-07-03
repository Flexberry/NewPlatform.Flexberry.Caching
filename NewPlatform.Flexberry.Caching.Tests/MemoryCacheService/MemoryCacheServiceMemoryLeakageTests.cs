namespace NewPlatform.Flexberry.Caching.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Xunit;

    public class MemoryCacheServiceMemoryLeakageTests
    {
        private const int ExpiratoinTime = 3; // In seconds.

        private const string CacheName = "expiration";

        /// <summary>
        /// Test memory leakage on expired values.
        /// </summary>
        [Fact(Skip = "Manual testing")]
        public void MemoryLeakageOnExpirationTest()
        {
            // Arrange.
            const int iterations = 100;
            const int length = 10000;
            int[] range = Enumerable.Range(0, length).ToArray();

            CleanupMemory();

            // Act.
            var cacheService = new MemoryCacheService(CacheName, ExpiratoinTime);
            IList<WeakReference> wrList = ProcessCacheService(cacheService, iterations, range);

            // Sleep for one more second after expiration time.
            Thread.Sleep((ExpiratoinTime + 1) * 1000);

            // Force remove expired entries.
            long removed = cacheService.Trim();
            Assert.True(removed <= iterations);

            CleanupMemory();

            // Assert.
            ValidateWeakReference(wrList);
        }

        /// <summary>
        /// Test memory leakage on expired values with checking existance.
        /// </summary>
        [Fact(Skip = "Manual testing")]
        public void MemoryLeakageOnExpirationWithCheckTest()
        {
            // Arrange.
            const int iterations = 100;
            const int length = 10000;
            int[] range = Enumerable.Range(0, length).ToArray();

            CleanupMemory();

            // Act.
            var cacheService = new MemoryCacheService(CacheName, ExpiratoinTime);
            IList<WeakReference> wrList = ProcessCacheService(cacheService, iterations, range);

            // Sleep for one more second after expiration time.
            Thread.Sleep((ExpiratoinTime + 1) * 1000);

            CleanupMemory();

            // Check global expiration.
            ValidateExpiration(cacheService, wrList);

            CleanupMemory();

            // Assert.
            ValidateWeakReference(wrList);
        }

        private static void CleanupMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private static IList<WeakReference> ProcessCacheService(ICacheService cacheService, int iterations, int[] range, bool parallel = false)
        {
            // Act.
            var iterRange = Enumerable.Range(0, iterations);
            return parallel
                ? iterRange.AsParallel().Select(i => IterateWeakReference(cacheService, range)).ToList()
                : iterRange.Select(i => IterateWeakReference(cacheService, range)).ToList();
        }

        private static WeakReference IterateWeakReference(ICacheService cacheService, IEnumerable<int> range)
        {
            var obj = Iterate(cacheService, range);
            return new WeakReference(obj);
        }

        private static SomeClass Iterate(ICacheService cacheService, IEnumerable<int> range)
        {
            // Arrange.
            string key = Guid.NewGuid().ToString("D");
            const int expectedIntValue = 101;
            const string expectedStringValue = "String value new";

            var newItem = new SomeClass
            {
                Key = key,
                IntValue = expectedIntValue,
                StringValue = expectedStringValue,
                Tags = range.Select(i => $"Tag #{i} ({Guid.NewGuid()}{Guid.NewGuid()}{Guid.NewGuid()})").ToList(),
            };

            // Act.
            if (!cacheService.SetToCache(key, newItem))
            {
                throw new Exception("Error while adding item to cache.");
            }

            // Assert.
            Assert.True(cacheService.Exists(key));

            return newItem;
        }

        private static void ValidateExpiration(ICacheService cacheService, IList<WeakReference> wrList)
        {
            foreach (var wr in wrList)
            {
                if (wr.IsAlive)
                {
                    if (wr.Target is SomeClass obj)
                    {
                        Assert.False(cacheService.Exists(obj.Key));
                    }
                    else
                    {
                        throw new Exception("WR указавает на объект некорректного типа.");
                    }
                }
            }
        }

        private static void ValidateWeakReference(IList<WeakReference> wrList)
        {
            Assert.All(
                wrList,
                wr =>
                {
                    if (wr.IsAlive)
                    {
                        throw new Exception("GC не смог освободить объект.");
                    }
                });
        }
    }
}
