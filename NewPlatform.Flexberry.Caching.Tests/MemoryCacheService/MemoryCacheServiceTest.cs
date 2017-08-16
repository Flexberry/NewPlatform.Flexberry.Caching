namespace NewPlatform.Flexberry.Caching.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using Xunit;

    /// <summary>
    /// Tests for <see cref="MemoryCacheService"/> class.
    /// </summary>
    public partial class MemoryCacheServiceTest : BaseCacheServiceTest
    {
        private const int TotalCacheItemsNumber = 1000;
        private const int MaximumStringLength = 256;

        // Create instance of service with default MemoryCache instance.
        public MemoryCacheServiceTest() : base(new MemoryCacheService())
        {
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.SetToCache(string,object)"/> method.
        /// </summary>
        [Fact]
        public void SetToCacheTest()
        {
            // Arrange.
            var key = "085c8372-b741-4ea0-b7ac-7b7e69bf6638";
            var expectedValue = "String value";

            object newItem = new
            {
                Value = expectedValue
            };

            // Act.
            if (!CacheService.SetToCache(key, newItem))
            {
                throw new Exception("Error while adding item to cache.");
            }

            // Assert.
            var itemFromCache = CacheService.GetFromCache(key);
            var value = (string)itemFromCache.GetType().GetProperty("Value", typeof(string)).GetValue(itemFromCache);
            Assert.Equal(expectedValue, value);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.SetToCache(string,object)"/> method in case of key is null or value is null.
        /// </summary>
        [Fact]
        public void SetToCacheWhenKeyIsNullOrValueIsNullTest()
        {
            // Arrange.
            var key = "085c8372-b741-4ea0-b7ac-7b7e69bf6638";

            object newItem = new
            {
                Value = "Some value"
            };

            var expectedParamNameForKey = "key";

            // Act.
            CacheService.SetToCache(key, null);
            var exForNullKey = Assert.Throws<ArgumentNullException>(() => CacheService.SetToCache(null, newItem));

            // Assert.
            Assert.Equal(expectedParamNameForKey, exForNullKey.ParamName);

            var itemFromCache = CacheService.GetFromCache(key);
            Assert.Equal(null, itemFromCache);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.SetToCache(string,object,int)"/> method.
        /// </summary>
        [Fact]
        public void SetToCacheExpirationTest()
        {
            // Arrange.
            var key = "085c8372-b741-4ea0-b7ac-7b7e69bf6638";
            var expectedValue = 555;
            var expiratoinTime = 3; // In seconds.

            object newItem = new
            {
                Value = expectedValue
            };

            // Act.
            if (!CacheService.SetToCache(key, newItem, expiratoinTime))
            {
                throw new Exception("Error while adding item to cache.");
            }

            // Assert.
            Assert.True(CacheService.Exists(key));

            // Sleep for one more second after expiration time.
            Thread.Sleep(expiratoinTime * 1000 + 1000);

            Assert.False(CacheService.Exists(key));
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.SetToCache(string,object,int)"/> method in case of expiration time is negative.
        /// </summary>
        [Fact]
        public void SetToCacheNegativeExpirationTimeTest()
        {
            // Arrange.
            var key = "085c8372-b741-4ea0-b7ac-7b7e69bf6638";
            var expiratoinTime = -1; // In seconds.
            var expectedParamNameForExpirationTime = "expirationTime";

            object newItem = new
            {
                Value = "Some value"
            };

            // Act.
            var ex = Assert.Throws<ArgumentException>(() => CacheService.SetToCache(key, newItem, expiratoinTime));

            // Assert.
            Assert.Equal(expectedParamNameForExpirationTime, ex.ParamName);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.SetToCache(string,object)"/> method in case of type of caching value is primitive.
        /// </summary>
        [Fact]
        public void SetToCacheWithPrimitiveTypeTest()
        {
            // Arrange.
            var key = "085c8372-b741-4ea0-b7ac-7b7e69bf6638";
            object expectedValue = 101;

            // Act.
            if (!CacheService.SetToCache(key, expectedValue))
            {
                throw new Exception("Error while adding item to cache.");
            }

            // Assert.
            var itemFromCache = (int)CacheService.GetFromCache(key);
            Assert.Equal(expectedValue, itemFromCache);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.SetToCache(string,object)"/> method in case of replacing existing item.
        /// </summary>
        [Fact]
        public void SetToCacheWithItemReplacingTest()
        {
            // Arrange.
            var key = "e07d5d4f-e531-4e7a-8840-21c2e9482404";
            var expectedValueBeforeReplace = 111;
            var expectedValueAfterReplace = "String value";

            object newItem = new
            {
                Value = expectedValueAfterReplace
            };

            // Act and Assert.

            // Check cached item before replace.
            var itemFromCache = CacheService.GetFromCache(key);
            var valueBeforeReplace = (int)itemFromCache.GetType().GetProperty("Value", typeof(int)).GetValue(itemFromCache);
            Assert.Equal(expectedValueBeforeReplace, valueBeforeReplace);

            // Replace existing item.
            if (!CacheService.SetToCache(key, newItem))
            {
                throw new Exception("Error while adding item to cache.");
            }

            // Check cached item after replace.
            itemFromCache = CacheService.GetFromCache(key);
            var valueAfterReplace = (string)itemFromCache.GetType().GetProperty("Value", typeof(string)).GetValue(itemFromCache);
            Assert.Equal(expectedValueAfterReplace, valueAfterReplace);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetFromCache"/> method.
        /// </summary>
        [Fact]
        public void GetFromCacheTest()
        {
            // Arrange.
            var keyOfAnonymousObject = "e07d5d4f-e531-4e7a-8840-21c2e9482404";
            var expectedValueOfAnonymousObject = 111;

            var key = "3d10fe87-2f92-4ff2-9e13-fffe2d59dad1";
            var expectedIntValue = 100;
            var expectedStringValue = "String value 1";

            // Act.
            var itemOfAnonymousType = CacheService.GetFromCache(keyOfAnonymousObject);
            var valueOfAnonymousType = (int)itemOfAnonymousType.GetType().GetProperty("Value", typeof(int)).GetValue(itemOfAnonymousType);

            var item = (SomeClass)CacheService.GetFromCache(key);

            // Assert.
            Assert.Equal(expectedValueOfAnonymousObject, valueOfAnonymousType);
            Assert.Equal(expectedIntValue, item.IntValue);
            Assert.Equal(expectedStringValue, item.StringValue);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetFromCache"/> method in case of key of getting item is not found in cache.
        /// </summary>
        [Fact]
        public void GetFromCacheWhenKeyIsNotFoundTest()
        {
            // Arrange.
            var key = "some wrong key";
            var expectedExceptionMessage = $"Key \"{key}\" is not found in cache.";

            // Act.
            var ex = Assert.Throws<KeyNotFoundException>(() => CacheService.GetFromCache(key));

            // Assert.
            Assert.Equal(expectedExceptionMessage, ex.Message);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetFromCache"/> method in case of key of getting item is null.
        /// </summary>
        [Fact]
        public void GetFromCacheWhenKeyIsNullTest()
        {
            // Arrange.
            var expectedParamNameForKey = "key";

            // Act.
            var ex = Assert.Throws<ArgumentNullException>(() => CacheService.GetFromCache(null));

            // Assert.
            Assert.Equal(expectedParamNameForKey, ex.ParamName);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetFromCacheByTag"/> method.
        /// </summary>
        [Fact]
        public void GetFromCacheByTagTest()
        {
            // Arrange.
            var tag = "tag5";
            int[] expectedValues = { 111, 222 };
            var expectedItemsCount = 2;

            // Act.
            var items = CacheService.GetFromCacheByTag(tag);

            // Assert.
            AssertItems(items, null, expectedValues, expectedItemsCount);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetFromCacheByTag"/> method in case of tag is not exists.
        /// </summary>
        [Fact]
        public void GetFromCacheByTagWithTagIsNotExistsTest()
        {
            // Arrange.
            var tag = "wrongTag";
            var expectedItemsCount = 0;

            // Act.
            var items = CacheService.GetFromCacheByTag(tag);

            // Assert.
            var enumerable = items as object[] ?? items.ToArray();
            Assert.Equal(expectedItemsCount, enumerable.Length);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetFromCacheByTag"/> method in case of tag of getting items is null.
        /// </summary>
        [Fact]
        public void GetFromCacheByTagWhenTagIsNullTest()
        {
            // Arrange.
            var expectedParamNameForTag = "tags";

            // Act.
            var ex = Assert.Throws<ArgumentNullException>(() => CacheService.GetFromCacheByTag(null));

            // Assert.
            Assert.Equal(expectedParamNameForTag, ex.ParamName);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetFromCacheByTag"/> method in case of getting items have different type.
        /// </summary>
        [Fact]
        public void GetFromCacheByTagWithDifferentItemsTypeTest()
        {
            // Arrange.
            var tag = "tag1";
            string[] expectedKeys =
            {
                "3d10fe87-2f92-4ff2-9e13-fffe2d59dad1",
                "fc3866d1-bf8f-4582-84c5-f8ddf9ae632c",
                "b4f5ac0e-9766-4198-8914-90947816dcc2",
                "33ac831c-c2a8-4d49-9e17-5b0030459d10",
                "618f9624-417e-47a1-a53e-d370d8ed8611"
            };

            int[] expectedValues = { 111 };
            var expectedItemsCount = 6;

            // Act.
            var items = CacheService.GetFromCacheByTag(tag);

            // Assert.
            AssertItems(items, expectedKeys, expectedValues, expectedItemsCount);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetFromCacheByTags"/> method.
        /// </summary>
        [Fact]
        public void GetFromCacheByTagsTest()
        {
            // Arrange.
            var tags = new List<string> { "tag1", "tag5" };
            string[] expectedKeys =
            {
                "3d10fe87-2f92-4ff2-9e13-fffe2d59dad1",
                "fc3866d1-bf8f-4582-84c5-f8ddf9ae632c",
                "b4f5ac0e-9766-4198-8914-90947816dcc2",
                "33ac831c-c2a8-4d49-9e17-5b0030459d10",
                "618f9624-417e-47a1-a53e-d370d8ed8611"
            };

            int[] expectedValues = { 111, 222 };
            var expectedItemsCount = 7;

            // Act.
            var items = CacheService.GetFromCacheByTags(tags);

            // Assert.
            AssertItems(items, expectedKeys, expectedValues, expectedItemsCount);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetFromCacheByTags"/> method in case of tags are not exists.
        /// </summary>
        [Fact]
        public void GetFromCacheByTagsWithTagsAreNotExistsTest()
        {
            // Arrange.
            var tags1 = new List<string> { "wrongTag1", "wrongTag2" };
            var tags2 = new List<string> { "tag1", "wrongTag1", "wrongTag2" };
            var expectedItemsCountForTags1 = 0;

            string[] expectedKeys =
            {
                "3d10fe87-2f92-4ff2-9e13-fffe2d59dad1",
                "fc3866d1-bf8f-4582-84c5-f8ddf9ae632c",
                "b4f5ac0e-9766-4198-8914-90947816dcc2",
                "33ac831c-c2a8-4d49-9e17-5b0030459d10",
                "618f9624-417e-47a1-a53e-d370d8ed8611"
            };

            int[] expectedValues = { 111 };
            var expectedItemsCountForTags2 = 6;

            // Act.
            var items1 = CacheService.GetFromCacheByTags(tags1);
            var items2 = CacheService.GetFromCacheByTags(tags2);

            // Assert.
            AssertItems(items1, null, new int[0], expectedItemsCountForTags1);
            AssertItems(items2, expectedKeys, expectedValues, expectedItemsCountForTags2);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetFromCacheByTags"/> method in case of tags of getting items are null.
        /// </summary>
        [Fact]
        public void GetFromCacheByTagWhenTagsAreNullTest()
        {
            // Arrange.
            var expectedParamNameForTag = "tags";

            // Act.
            var ex = Assert.Throws<ArgumentNullException>(() => CacheService.GetFromCacheByTags(null));

            // Assert.
            Assert.Equal(expectedParamNameForTag, ex.ParamName);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.UpdateInCache"/> method.
        /// </summary>
        [Fact]
        public void UpdateInCacheTest()
        {
            // Arrange.
            var key = "e07d5d4f-e531-4e7a-8840-21c2e9482404";
            var expectedValueBeforeUpdate = 111;
            string[] expectedTagsBeforeUpdateArray =
            {
                "tag1",
                "tag5"
            };

            var expectedValueAfterUpdate = 121;
            var expiratoinTimeAfterUpdate = 3; // In seconds.
            var additionalTagsForUpdate = new List<string>
            {
                "tag6",
                "tag7"
            };

            var expectedTagsAfterUpdate = new List<string>
            {
                "tag1",
                "tag5",
                "tag6",
                "tag7"
            };

            var expectedTagsAfterUpdateArray = expectedTagsAfterUpdate.ToArray();

            object newItemForUpdate = new
            {
                Value = expectedValueAfterUpdate
            };

            // Act and Assert.

            // Initially expiration time is not set, so check that item is not removed from cache after some pause.
            Assert.True(CacheService.Exists(key));

            // Sleep specified time.
            Thread.Sleep(expiratoinTimeAfterUpdate * 1000 + 1000);

            // Check that item is still exists in cache.
            Assert.True(CacheService.Exists(key));

            // Check cached item before update.
            var itemFromCache = CacheService.GetFromCache(key);
            var value = (int)itemFromCache.GetType().GetProperty("Value", typeof(int)).GetValue(itemFromCache);
            Assert.Equal(expectedValueBeforeUpdate, value);

            var tagsBeforeUpdate = CacheService.GetTagsForItem(key);
            AssertTags(tagsBeforeUpdate, expectedTagsBeforeUpdateArray, expectedTagsBeforeUpdateArray.Length);

            // Update existing item.
            if (!CacheService.UpdateInCache(key, newItemForUpdate, expiratoinTimeAfterUpdate, additionalTagsForUpdate))
            {
                throw new Exception("Error while updating item in cache.");
            }

            // Check cached item after update.
            itemFromCache = CacheService.GetFromCache(key);
            value = (int)itemFromCache.GetType().GetProperty("Value", typeof(int)).GetValue(itemFromCache);
            Assert.Equal(expectedValueAfterUpdate, value);

            var tagsAfterUpdate = CacheService.GetTagsForItem(key);
            AssertTags(tagsAfterUpdate, expectedTagsAfterUpdateArray, expectedTagsAfterUpdateArray.Length);

            Assert.True(CacheService.Exists(key));

            // Sleep for one more second after expiration time.
            Thread.Sleep(expiratoinTimeAfterUpdate * 1000 + 1000);

            Assert.False(CacheService.Exists(key));
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.UpdateInCache"/> method in case of key is null or value is null.
        /// </summary>
        [Fact]
        public void UpdateInCacheWhenKeyIsNullOrValueIsNullTest()
        {
            // Arrange.
            var key = "e07d5d4f-e531-4e7a-8840-21c2e9482404";
            var expectedValueAfterUpdate = 121;

            object newItemForUpdate = new
            {
                Value = expectedValueAfterUpdate
            };

            var expectedParamNameForKey = "key";

            // Act.
            CacheService.UpdateInCache(key, null, 0, null);
            var exForNullKey = Assert.Throws<ArgumentNullException>(() => CacheService.UpdateInCache(null, newItemForUpdate, 0, null));

            // Assert.
            Assert.Equal(expectedParamNameForKey, exForNullKey.ParamName);

            var itemFromCache = CacheService.GetFromCache(key);
            Assert.Equal(null, itemFromCache);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.UpdateInCache"/> method in case of expiration time is negative.
        /// </summary>
        [Fact]
        public void UpdateInCacheNegativeExpirationTimeTest()
        {
            // Arrange.
            var key = "e07d5d4f-e531-4e7a-8840-21c2e9482404";
            var expiratoinTime = -1; // In seconds.
            var expectedParamNameForExpirationTime = "expirationTime";

            object newItemForUpdate = new
            {
                Value = 121
            };

            // Act.
            var ex = Assert.Throws<ArgumentException>(() => CacheService.UpdateInCache(key, newItemForUpdate, expiratoinTime, null));

            // Assert.
            Assert.Equal(expectedParamNameForExpirationTime, ex.ParamName);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.UpdateInCache"/> method in case of additional tags are null or empty.
        /// </summary>
        [Fact]
        public void UpdateInCacheWithAdditionalTagsAreNullOrEmptyTest()
        {
            // Arrange.
            var key = "e07d5d4f-e531-4e7a-8840-21c2e9482404";
            string[] expectedTagsBeforeUpdateArray =
            {
                "tag1",
                "tag5"
            };

            var expectedValueAfterUpdate = 121;
            var additionalTagsForUpdate = new List<string>();

            var expectedTagsAfterUpdate = new List<string>
            {
                "tag1",
                "tag5"
            };

            var expectedTagsAfterUpdateArray = expectedTagsAfterUpdate.ToArray();

            object newItemForUpdate = new
            {
                Value = expectedValueAfterUpdate
            };

            // Act and Assert.

            // Check cached item before update.
            var tagsBeforeUpdate = CacheService.GetTagsForItem(key);
            AssertTags(tagsBeforeUpdate, expectedTagsBeforeUpdateArray, expectedTagsBeforeUpdateArray.Length);

            // Update existing item with no additional tags.
            if (!CacheService.UpdateInCache(key, newItemForUpdate, 0, additionalTagsForUpdate))
            {
                throw new Exception("Error while updating item in cache.");
            }

            // Check cached item after update.
            var tagsAfterUpdate1 = CacheService.GetTagsForItem(key);
            AssertTags(tagsAfterUpdate1, expectedTagsAfterUpdateArray, expectedTagsAfterUpdateArray.Length);

            // Update existing item with specifying that additional tags are null.
            if (!CacheService.UpdateInCache(key, newItemForUpdate, 0, null))
            {
                throw new Exception("Error while updating item in cache.");
            }

            // Check cached item after update.
            var tagsAfterUpdate2 = CacheService.GetTagsForItem(key);
            AssertTags(tagsAfterUpdate2, expectedTagsAfterUpdateArray, expectedTagsAfterUpdateArray.Length);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.TryGetFromCache"/> method.
        /// </summary>
        [Fact]
        public void TryGetFromCacheTest()
        {
            // Arrange.
            var keyOfAnonymousObject = "e07d5d4f-e531-4e7a-8840-21c2e9482404";
            var expectedValueOfAnonymousObject = 111;

            var key = "3d10fe87-2f92-4ff2-9e13-fffe2d59dad1";
            var expectedIntValue = 100;
            var expectedStringValue = "String value 1";

            var wrongKey = "i'm wrong";

            // Act.
            object itemOfAnonymousType;
            var itemOfAnonymousTypeWasSuccessfullyGotten = CacheService.TryGetFromCache(keyOfAnonymousObject, out itemOfAnonymousType);
            var valueOfAnonymousType = (int)itemOfAnonymousType.GetType().GetProperty("Value", typeof(int)).GetValue(itemOfAnonymousType);

            object itemOfSomeClassType;
            var itemOfSomeClassTypeWasSuccessfullyGotten = CacheService.TryGetFromCache(key, out itemOfSomeClassType);
            var item = (SomeClass)itemOfSomeClassType;

            object someObjectWithWrongKey;
            var itemWithWrongKeyWasSuccessfullyGotten = CacheService.TryGetFromCache(wrongKey, out someObjectWithWrongKey);

            object someObjectForNull;
            var itemOfNullTypeWasSuccessfullyGotten = CacheService.TryGetFromCache(null, out someObjectForNull);

            // Assert.
            Assert.True(itemOfAnonymousTypeWasSuccessfullyGotten);
            Assert.True(itemOfSomeClassTypeWasSuccessfullyGotten);
            Assert.False(itemWithWrongKeyWasSuccessfullyGotten);
            Assert.False(itemOfNullTypeWasSuccessfullyGotten);

            Assert.Equal(expectedValueOfAnonymousObject, valueOfAnonymousType);
            Assert.Equal(expectedIntValue, item.IntValue);
            Assert.Equal(expectedStringValue, item.StringValue);

            Assert.Equal(null, someObjectWithWrongKey);
            Assert.Equal(null, someObjectForNull);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.TryGetFromCacheByTag"/> method.
        /// </summary>
        [Fact]
        public void TryGetFromCacheByTagTest()
        {
            // Arrange.
            var tag = "tag5";
            int[] expectedValues = { 111, 222 };
            var expectedItemsCount = 2;

            var wrongTag = "i'm wrong";

            // Act.
            IEnumerable<object> itemsWithExistingTag;
            var itemsOfAnonymousTypeWasSuccessfullyGotten = CacheService.TryGetFromCacheByTag(tag, out itemsWithExistingTag);

            IEnumerable<object> someObjectsWithWrongTag;
            var itemsWithWrongTagWasSuccessfullyGotten = CacheService.TryGetFromCacheByTag(wrongTag, out someObjectsWithWrongTag);

            IEnumerable<object> someObjectsForNull;
            var itemsOfNullTypeWasSuccessfullyGotten = CacheService.TryGetFromCacheByTag(null, out someObjectsForNull);

            //Assert.
            AssertItems(itemsWithExistingTag, null, expectedValues, expectedItemsCount);
            Assert.True(itemsOfAnonymousTypeWasSuccessfullyGotten);
            Assert.True(itemsWithWrongTagWasSuccessfullyGotten);
            Assert.False(itemsOfNullTypeWasSuccessfullyGotten);

            Assert.Equal(0, someObjectsWithWrongTag.Count());
            Assert.Equal(0, someObjectsForNull.Count());
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.TryGetFromCacheByTags"/> method.
        /// </summary>
        [Fact]
        public void TryGetFromCacheByTagsTest()
        {
            // Arrange.
            var tags1 = new List<string> { "tag1", "tag5" };
            string[] expectedKeys1 =
            {
                "3d10fe87-2f92-4ff2-9e13-fffe2d59dad1",
                "fc3866d1-bf8f-4582-84c5-f8ddf9ae632c",
                "b4f5ac0e-9766-4198-8914-90947816dcc2",
                "33ac831c-c2a8-4d49-9e17-5b0030459d10",
                "618f9624-417e-47a1-a53e-d370d8ed8611"
            };

            int[] expectedValues1 = { 111, 222 };
            var expectedItemsCountForTags1 = 7;

            var tags2 = new List<string> { "tag1", "wrongTag1", "wrongTag2" };

            string[] expectedKeys2 =
            {
                "3d10fe87-2f92-4ff2-9e13-fffe2d59dad1",
                "fc3866d1-bf8f-4582-84c5-f8ddf9ae632c",
                "b4f5ac0e-9766-4198-8914-90947816dcc2",
                "33ac831c-c2a8-4d49-9e17-5b0030459d10",
                "618f9624-417e-47a1-a53e-d370d8ed8611"
            };

            int[] expectedValues2 = { 111 };
            var expectedItemsCountForTags2 = 6;

            var tags3 = new List<string> { "wrongTag1", "wrongTag2" };
            var expectedItemsCountForTags3 = 0;

            // Act..
            IEnumerable<object> itemsWithExistingTags;
            var itemsWithExistingTagsWasSuccessfullyGotten = CacheService.TryGetFromCacheByTags(tags1, out itemsWithExistingTags);

            IEnumerable<object> someObjectsWithSomeWrongTags;
            var itemsWithSomeWrongTagsWasSuccessfullyGotten = CacheService.TryGetFromCacheByTags(tags2, out someObjectsWithSomeWrongTags);

            IEnumerable<object> someObjectsWithWrongTags;
            var itemsWithWrongTagsWasSuccessfullyGotten = CacheService.TryGetFromCacheByTags(tags3, out someObjectsWithWrongTags);

            IEnumerable<object> someObjectsForNull;
            var itemsOfNullTypeWasSuccessfullyGotten = CacheService.TryGetFromCacheByTags(null, out someObjectsForNull);

            // Assert.
            AssertItems(itemsWithExistingTags, expectedKeys1, expectedValues1, expectedItemsCountForTags1);
            AssertItems(someObjectsWithSomeWrongTags, expectedKeys2, expectedValues2, expectedItemsCountForTags2);
            AssertItems(someObjectsWithWrongTags, null, new int[0], expectedItemsCountForTags3);
            AssertItems(someObjectsForNull, null, new int[0], 0);
            Assert.True(itemsWithExistingTagsWasSuccessfullyGotten);
            Assert.True(itemsWithSomeWrongTagsWasSuccessfullyGotten);
            Assert.True(itemsWithWrongTagsWasSuccessfullyGotten);
            Assert.False(itemsOfNullTypeWasSuccessfullyGotten);

        }
    }
}
