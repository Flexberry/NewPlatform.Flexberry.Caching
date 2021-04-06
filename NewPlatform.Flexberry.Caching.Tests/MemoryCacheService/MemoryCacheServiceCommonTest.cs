namespace NewPlatform.Flexberry.Caching.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using Xunit;

    public partial class MemoryCacheServiceTest
    {
        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetCount"/> method.
        /// </summary>
        [Fact]
        public void GetCountTest()
        {
            // Arrange.

            // 13 cached items + 5 tags
            var initialNumberOfItems = InitialNumberOfItemsInCache[CacheService.CacheName];
            var initialNumberOfTags = InitialNumberOfTagsInCache[CacheService.CacheName];
            long expectedTagsCachedItemsCount = initialNumberOfTags;
            long expectedTotalCachedItemsCount = initialNumberOfItems + expectedTagsCachedItemsCount;

            // Act.
            var totalCachedItemsCount = CacheService.GetCount();

            //Assert.
            Assert.Equal(expectedTotalCachedItemsCount, totalCachedItemsCount);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetTagsCount"/> method.
        /// </summary>
        [Fact]
        public void GetTagsCountTest()
        {
            // Arrange.

            var initialNumberOfTags = InitialNumberOfTagsInCache[CacheService.CacheName];
            long expectedTagsCachedItemsCount = initialNumberOfTags;

            // Act.
            var totalCachedTagsItemsCount = CacheService.GetTagsCount();

            //Assert.
            Assert.Equal(expectedTagsCachedItemsCount, totalCachedTagsItemsCount);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetTagsForItem"/> method.
        /// </summary>
        [Fact]
        public void GetTagsForItemTest()
        {
            // Arrange.
            var key = "e07d5d4f-e531-4e7a-8840-21c2e9482404";
            string[] expectedTags =
            {
                "tag1",
                "tag5"
            };

            var expectedTagsCount = 2;

            // Act.
            var tags = CacheService.GetTagsForItem(key);

            // Assert.
            var enumerable = tags as string[] ?? tags.ToArray();
            Assert.Equal(expectedTagsCount, enumerable.Length);

            foreach (var item in enumerable)
            {
                Assert.Contains(item, expectedTags);
            }
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetTagsForItem"/> method in case of key of getting item is null.
        /// </summary>
        [Fact]
        public void GetTagsForItemWhenKeyIsNullTest()
        {
            // Arrange.
            var expectedParamNameForKey = "key";

            // Act.
            var ex = Assert.Throws<ArgumentNullException>(() => CacheService.GetTagsForItem(null));

            // Assert.
            Assert.Equal(expectedParamNameForKey, ex.ParamName);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetTagsForItem"/> method in case of getting item has no tags.
        /// </summary>
        [Fact]
        public void GetTagsForItemWhenItemHasNoTagsTest()
        {
            // Arrange.
            var key1 = "66d79b6a-5225-4a5c-9d2f-f774d484ec05";
            var key2 = "9f2933b6-5ed4-4069-8ee6-aa041bb15716";

            var expectedTagsCount = 0;

            // Act.
            var tags1 = CacheService.GetTagsForItem(key1);
            var tags2 = CacheService.GetTagsForItem(key2);

            // Assert.
            var enumerable1 = tags1 as string[] ?? tags1.ToArray();
            var enumerable2 = tags2 as string[] ?? tags2.ToArray();

            Assert.Equal(expectedTagsCount, enumerable1.Length);
            Assert.Equal(expectedTagsCount, enumerable2.Length);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.ClearCache"/> method.
        /// </summary>
        [Fact(Skip = "Some test runs fail, test disabled until investigation this problem.")]
        public void ClearCacheTest()
        {
            // Arrange.

            // 13 cached items + 5 tags
            var initialNumberOfItems = InitialNumberOfItemsInCache[CacheService.CacheName];
            var initialNumberOfTags = InitialNumberOfTagsInCache[CacheService.CacheName];
            long expectedTagsCachedItemsCountBeforeClear = initialNumberOfTags;
            long expectedTotalCachedItemsCountBeforeClear = initialNumberOfItems + expectedTagsCachedItemsCountBeforeClear;

            long expectedTagsCachedItemsCountAfterClear = 0;
            long expectedTotalCachedItemsCountAfterClear = 0;

            // Act and assert.
            Assert.Equal(expectedTotalCachedItemsCountBeforeClear, CacheService.GetCount());
            Assert.Equal(expectedTagsCachedItemsCountBeforeClear, CacheService.GetTagsCount());

            // Clearing all items in cache.
            CacheService.ClearCache();

            Assert.Equal(expectedTotalCachedItemsCountAfterClear, CacheService.GetCount());
            Assert.Equal(expectedTagsCachedItemsCountAfterClear, CacheService.GetTagsCount());
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.Exists"/> method.
        /// </summary>
        [Fact]
        public void ExistsTest()
        {
            // Arrange.
            var existingKey = "66d79b6a-5225-4a5c-9d2f-f774d484ec05";
            var nonExistingKey = "I'm not exist.";

            // Act.
            var result1 = CacheService.Exists(existingKey);
            var result2 = CacheService.Exists(nonExistingKey);
            var result3 = CacheService.Exists(null);
            var result4 = CacheService.Exists(string.Empty);

            // Assert.
            Assert.True(result1);
            Assert.False(result2);
            Assert.False(result3);
            Assert.False(result4);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.ExistsByTag"/> method.
        /// </summary>
        [Fact]
        public void ExistsByTagTest()
        {
            // Arrange.
            string[] existingTags = { "tag1", "tag2", "tag3", "tag4", "tag5" };
            var nonExistingTag = "tag6";

            // Act and assert.
            foreach (var item in existingTags)
            {
                var result1 = CacheService.ExistsByTag(item);
                Assert.True(result1);
            }

            var result2 = CacheService.ExistsByTag(nonExistingTag);
            var result3 = CacheService.ExistsByTag(null);
            var result4 = CacheService.ExistsByTag(string.Empty);
            Assert.False(result2);
            Assert.False(result3);
            Assert.False(result4);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.DeleteFromCache"/> method.
        /// </summary>
        [Fact]
        public void DeleteFromCacheTest()
        {
            // Arrange.
            var key = "66d79b6a-5225-4a5c-9d2f-f774d484ec05";

            // Act and assert.
            Assert.True(CacheService.Exists(key));

            var somethingWasDeleted = CacheService.DeleteFromCache(key);
            Assert.True(somethingWasDeleted);

            Assert.False(CacheService.Exists(key));
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.DeleteFromCache"/> method in case of item with specified key is not exists in cache.
        /// </summary>
        [Fact]
        public void DeleteFromCacheWithItemIsNotExistsTest()
        {
            // Arrange.
            var key = "some wrong key";

            // Act and assert.
            Assert.False(CacheService.Exists(key));

            var somethingWasDeleted = CacheService.DeleteFromCache(key);
            Assert.False(somethingWasDeleted);

            Assert.False(CacheService.Exists(key));
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.DeleteFromCache"/> method in case of key of getting item is null.
        /// </summary>
        [Fact]
        public void DeleteFromCacheWhenKeyIsNullTest()
        {
            // Arrange.
            var expectedParamNameForKey = "key";

            // Act.
            var ex = Assert.Throws<ArgumentNullException>(() => CacheService.DeleteFromCache(null));

            // Assert.
            Assert.Equal(expectedParamNameForKey, ex.ParamName);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.DeleteFromCacheByTag"/> method.
        /// </summary>
        [Fact]
        public void DeleteFromCacheByTagTest()
        {
            // Arrange.
            var tag = "tag1";

            // 13 cached items + 5 tags
            var initialNumberOfItems = InitialNumberOfItemsInCache[CacheService.CacheName];
            var initialNumberOfTags = InitialNumberOfTagsInCache[CacheService.CacheName];
            long expectedTotalCachedTagsCountBeforeDelete = initialNumberOfTags;
            long expectedTotalCachedItemsCountBeforeDelete = initialNumberOfItems + expectedTotalCachedTagsCountBeforeDelete;

            // 7 cached items + 5 tags.
            // Information about tags deletes from cache only when chache is totally clearing.
            long expectedTotalCachedTagsCountAfterDelete = initialNumberOfTags;

            var deletedItemsCountExpected = 6;
            long expectedTotalCachedItemsCountAfterDelete = initialNumberOfItems - deletedItemsCountExpected + expectedTotalCachedTagsCountAfterDelete;

            // Act and assert.
            Assert.Equal(expectedTotalCachedItemsCountBeforeDelete, CacheService.GetCount());
            Assert.Equal(expectedTotalCachedTagsCountBeforeDelete, CacheService.GetTagsCount());
            Assert.True(CacheService.ExistsByTag(tag));

            var somethingWasDeleted = CacheService.DeleteFromCacheByTag(tag);
            Assert.True(somethingWasDeleted);

            Assert.Equal(expectedTotalCachedItemsCountAfterDelete, CacheService.GetCount());
            Assert.Equal(expectedTotalCachedTagsCountAfterDelete, CacheService.GetTagsCount());
            Assert.False(CacheService.ExistsByTag(tag));
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.DeleteFromCacheByTag"/> method in case of item with specified tag is not exists in cache.
        /// </summary>
        [Fact]
        public void DeleteFromCacheByTagWithTagIsNotExistsTest()
        {
            // Arrange.
            var tag = "someWrongTag";

            // Act and assert.
            Assert.False(CacheService.ExistsByTag(tag));

            var somethingWasDeleted = CacheService.DeleteFromCacheByTag(tag);
            Assert.False(somethingWasDeleted);

            Assert.False(CacheService.ExistsByTag(tag));
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.DeleteFromCacheByTag"/> method in case of tag of getting item is null.
        /// </summary>
        [Fact]
        public void DeleteFromCacheByTagWhenTagIsNullTest()
        {
            // Arrange.
            var expectedParamNameForKey = "tag";

            // Act.
            var ex = Assert.Throws<ArgumentNullException>(() => CacheService.DeleteFromCacheByTag(null));

            // Assert.
            Assert.Equal(expectedParamNameForKey, ex.ParamName);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.DeleteFromCacheByTags"/> method.
        /// </summary>
        [Fact]
        public void DeleteFromCacheByTagsTest()
        {
            // Arrange.
            List<string> tags = new List<string> { "tag1", "tag2"};

            // 13 cached items + 5 tags
            var initialNumberOfItems = InitialNumberOfItemsInCache[CacheService.CacheName];
            var initialNumberOfTags = InitialNumberOfTagsInCache[CacheService.CacheName];
            long expectedTotalCachedTagsCountBeforeDelete = initialNumberOfTags;
            long expectedTotalCachedItemsCountBeforeDelete = initialNumberOfItems + expectedTotalCachedTagsCountBeforeDelete;

            // 5 cached items + 5 tags.
            // Information about tags deletes from cache only when chache is totally clearing.
            long expectedTotalCachedTagsCountAfterDelete = initialNumberOfTags;

            var deletedItemsCountExpected = 8;
            long expectedTotalCachedItemsCountAfterDelete = initialNumberOfItems - deletedItemsCountExpected + expectedTotalCachedTagsCountAfterDelete;

            // Act and assert.
            Assert.Equal(expectedTotalCachedItemsCountBeforeDelete, CacheService.GetCount());
            Assert.Equal(expectedTotalCachedTagsCountBeforeDelete, CacheService.GetTagsCount());
            foreach (var tag in tags)
            {
                Assert.True(CacheService.ExistsByTag(tag));
            }

            var somethingWasDeleted = CacheService.DeleteFromCacheByTags(tags);
            Assert.True(somethingWasDeleted);

            Assert.Equal(expectedTotalCachedItemsCountAfterDelete, CacheService.GetCount());
            Assert.Equal(expectedTotalCachedTagsCountAfterDelete, CacheService.GetTagsCount());
            foreach (var tag in tags)
            {
                Assert.False(CacheService.ExistsByTag(tag));
            }
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.DeleteFromCacheByTags"/> method in case of items with specified tags are not exists in cache.
        /// </summary>
        [Fact]
        public void DeleteFromCacheByTagsWithTagsAreNotExistTest()
        {
            // Arrange.
            List<string> tags = new List<string> { "wrongTag1", "wrongTag2" };

            var existingTagName = "tag1";
            List<string> tagsWithExistingTag = new List<string> { "wrongTag1", existingTagName };

            // 13 cached items + 5 tags
            var initialNumberOfItems = InitialNumberOfItemsInCache[CacheService.CacheName];
            var initialNumberOfTags = InitialNumberOfTagsInCache[CacheService.CacheName];
            long expectedTotalCachedTagsCountBeforeDelete = initialNumberOfTags;
            long expectedTotalCachedItemsCountBeforeDelete = initialNumberOfItems + expectedTotalCachedTagsCountBeforeDelete;

            // 7 cached items + 5 tags.
            // Information about tags deletes from cache only when chache is totally clearing.
            long expectedTotalCachedTagsCountAfterDelete = initialNumberOfTags;

            var deletedItemsCountExpected = 6;
            long expectedTotalCachedItemsCountAfterDelete = initialNumberOfItems - deletedItemsCountExpected + expectedTotalCachedTagsCountAfterDelete;

            // Act and assert.
            Assert.Equal(expectedTotalCachedItemsCountBeforeDelete, CacheService.GetCount());
            Assert.Equal(expectedTotalCachedTagsCountBeforeDelete, CacheService.GetTagsCount());
            foreach (var tag in tags)
            {
                Assert.False(CacheService.ExistsByTag(tag));
            }

            foreach (var tag in tagsWithExistingTag)
            {
                if (tag == existingTagName)
                {
                    Assert.True(CacheService.ExistsByTag(tag));
                }
                else
                {
                    Assert.False(CacheService.ExistsByTag(tag));
                }
            }

            var somethingWasDeleted = CacheService.DeleteFromCacheByTags(tags);
            Assert.False(somethingWasDeleted);
            somethingWasDeleted = CacheService.DeleteFromCacheByTags(tagsWithExistingTag);
            Assert.True(somethingWasDeleted);

            Assert.Equal(expectedTotalCachedItemsCountAfterDelete, CacheService.GetCount());
            Assert.Equal(expectedTotalCachedTagsCountAfterDelete, CacheService.GetTagsCount());
            foreach (var tag in tags)
            {
                Assert.False(CacheService.ExistsByTag(tag));
            }

            foreach (var tag in tagsWithExistingTag)
            {
                Assert.False(CacheService.ExistsByTag(tag));
            }
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.DeleteFromCacheByTags"/> method in case of tags of getting item are null.
        /// </summary>
        [Fact]
        public void DeleteFromCacheByTagsWhenTagsAreNullTest()
        {
            // Arrange.
            var expectedParamNameForKey = "tags";

            // Act.
            var ex = Assert.Throws<ArgumentNullException>(() => CacheService.DeleteFromCacheByTags(null));

            // Assert.
            Assert.Equal(expectedParamNameForKey, ex.ParamName);
        }

        /// <summary>
        /// Testing interaction of several <see cref="MemoryCacheService"/> instances.
        /// </summary>
        [Fact]
        public void InteractionOfSeveralMemoryCacheServiceInstancesTest()
        {
            // Arrange.
            var expectedItemsCountForDefaultSet = InitialNumberOfItemsInCache[CacheService.CacheName] + InitialNumberOfTagsInCache[CacheService.CacheName];

            // Act.

            // If no cache name specified as a param of constructor then default MemoryChache instance
            // would be used as a cache and data would be shared between several MemoryCacheService instances.
            var anotherDefaultMemoryCacheService = new MemoryCacheService();

            var anotherMemoryCacheService = new MemoryCacheService("anotherCache");

            var itemsCountForCacheService = CacheService.GetCount();
            var itemsCountForAnoterDefaultCacheService = anotherDefaultMemoryCacheService.GetCount();

            var itemsCountForAnoterCacheServiceBeforeInit = anotherMemoryCacheService.GetCount();
            Initialize(anotherMemoryCacheService);
            var itemsCountForAnoterCacheServiceAfterInit = anotherMemoryCacheService.GetCount();

            // Assert.
            Assert.Equal(expectedItemsCountForDefaultSet, itemsCountForCacheService);
            Assert.Equal(expectedItemsCountForDefaultSet, itemsCountForAnoterDefaultCacheService);
            Assert.Equal(0, itemsCountForAnoterCacheServiceBeforeInit);
            Assert.Equal(expectedItemsCountForDefaultSet, itemsCountForAnoterCacheServiceAfterInit);
        }

        /// <summary>
        /// Testing adding of items after some items were deleted by tag.
        /// </summary>
        [Fact]
        public void AddItemsAfterDeleteByTagTest()
        {
            // Arrange.
            var expectedItemsCountForTag1BeforeDelete = 6;
            var items = new List<SomeClass>
            {
                new SomeClass
                {
                    Key = "4b59afed-46ba-4c7f-aa99-2c15a07701aa",
                    IntValue = 101,
                    StringValue = "String value 1.1",
                    Tags = new List<string> {"tag1", "tag2"}
                },
                new SomeClass
                {
                    Key = "60b67caf-4cdc-486e-a142-76f93156f5cc",
                    IntValue = 201,
                    StringValue = "String value 2.2",
                    Tags = new List<string> {"tag1"}
                },
                new SomeClass
                {
                    Key = "3f0fe9d4-726f-43d2-ade9-86c6a0a12996",
                    IntValue = 301,
                    StringValue = "String value 3.3",
                    Tags = new List<string> {"tag1", "tag2", "tag3"}
                }
            };
            var expectedKeysForTag1AfterReAddingItemsWithTag1 = items.Select(i => i.Key).ToArray();
            var expectedItemsCountForTag1AfterReAddingItemsWithTag1 = items.Count;

            var tag = "tag1";

            var expectedTagsCount = InitialNumberOfTagsInCache[CacheService.CacheName];
            var expectedItemsCountBeforeDelete = InitialNumberOfItemsInCache[CacheService.CacheName] + InitialNumberOfTagsInCache[CacheService.CacheName];
            var expectedItemsCountAfterReAddingItems = expectedItemsCountBeforeDelete - expectedItemsCountForTag1BeforeDelete + expectedItemsCountForTag1AfterReAddingItemsWithTag1;

            // Act and Assert.
            var itemsCountBeforeDelete = CacheService.GetCount();
            var itemsCountForTag1BeforeDelete = CacheService.GetFromCacheByTag(tag).Count();
            Assert.Equal(expectedItemsCountBeforeDelete, itemsCountBeforeDelete);
            Assert.Equal(expectedItemsCountForTag1BeforeDelete, itemsCountForTag1BeforeDelete);
            if (!CacheService.DeleteFromCacheByTag(tag))
            {
                throw new Exception($"Error while deleting items from cache by tag \"{tag}\".");
            }

            var expectedItemsCountForTag1AfterDelete = CacheService.GetFromCacheByTag(tag).Count();
            Assert.Equal(0, expectedItemsCountForTag1AfterDelete);

            foreach (var item in items)
            {
                if (!CacheService.SetToCache(item.Key, item, item.Tags))
                {
                    throw new Exception("Error while initializing cache for tests.");
                }
            }

            var itemsCountAfterReAddingItems = CacheService.GetCount();
            Assert.Equal(expectedItemsCountAfterReAddingItems, itemsCountAfterReAddingItems);

            var itemsWithExistingTag = CacheService.GetFromCacheByTag<SomeClass>(tag);
            AssertItems(itemsWithExistingTag, expectedKeysForTag1AfterReAddingItemsWithTag1, new int[0], expectedItemsCountForTag1AfterReAddingItemsWithTag1);

            var tagsCount = CacheService.GetTagsCount();
            Assert.Equal(expectedTagsCount, tagsCount);
        }
    }
}
