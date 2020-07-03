namespace NewPlatform.Flexberry.Caching.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using NewPlatform.Flexberry.ORM.Tests;

    using Xunit;
    using Xunit.Abstractions;

    /// <summary>
    /// Многопоточные тесты <see cref="MemoryCacheService" />.
    /// </summary>
    public class MemoryCacheServiceMultiThreadTests
    {
        private const int ExpiratoinTime = 3; // In seconds.

        private const string CacheName = "multithread";

        private readonly ITestOutputHelper Output;

        /// <summary>
        /// Конструктор.
        /// </summary>
        public MemoryCacheServiceMultiThreadTests(ITestOutputHelper output)
        {
            Output = output;
        }

        /// <summary>
        /// Многопоточный тест для <see cref="ParallelEnumerable.AsParallel" />.
        /// </summary>
        [Fact]
        public void GetSetAsParallelTest()
        {
            // Arrange.
            const int length = 100;
            IEnumerable<int> range = Enumerable.Range(0, length);
            var cacheService = new MemoryCacheService(CacheName, ExpiratoinTime);

            // Act.
            range.AsParallel().ForAll(i => Action(cacheService, i));
        }

        /// <summary>
        /// Многопоточный тест с использованием <see cref="MultiThreadingTestTool"/>.
        /// </summary>
        [Fact]
        public void GetSetMultiThreadToolTest()
        {
            // Arrange.
            const int length = 100;
            var cacheService = new MemoryCacheService(CacheName, ExpiratoinTime);
            var multiThreadingTestTool = new MultiThreadingTestTool(MultiThreadMethod);
            multiThreadingTestTool.StartThreads(length, cacheService);

            // Assert.
            var exception = multiThreadingTestTool.GetExceptions();
            if (exception != null)
            {
                foreach (var item in exception.InnerExceptions)
                {
                    Output.WriteLine($"Thread {item.Key}: {item.Value}");
                }

                // Пусть так.
                Assert.Empty(exception.InnerExceptions);
            }
        }

        private static void MultiThreadMethod(object sender)
        {
            var parametersDictionary = sender as Dictionary<string, object>;
            ICacheService cacheService = parametersDictionary[MultiThreadingTestTool.ParamNameSender] as ICacheService;
            Dictionary<string, Exception> exceptions = parametersDictionary[MultiThreadingTestTool.ParamNameExceptions] as Dictionary<string, Exception>;

            // Act.
            try
            {
                Action(cacheService);
            }
            catch (Exception exception)
            {
                exceptions.Add(Thread.CurrentThread.Name, exception);
                parametersDictionary[MultiThreadingTestTool.ParamNameWorking] = false;
                return;
            }
        }

        private static void Action(ICacheService cacheService, int i = 0)
        {
            // Arrange.
            var obj = CreateObject(i);

            // Act.
            if (!cacheService.SetToCache(obj.Key, obj))
            {
                throw new Exception("Error while adding item to cache.");
            }

            var objFromCache = cacheService.GetFromCache(obj.Key);

            // Assert.
            Assert.NotNull(objFromCache);
            Assert.Equal(obj, objFromCache);
        }

        private static SomeClass CreateObject(int i = 0)
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
                Tags = new List<string> { $"Tag #{i} ({Guid.NewGuid()}{Guid.NewGuid()}{Guid.NewGuid()})" },
            };

            return newItem;
        }
    }
}
