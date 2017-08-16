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
        /// Testing behavior of <see cref="MemoryCacheService.SetToCache{T}(string,T)"/> method.
        /// </summary>
        [Fact]
        public void SetToCacheGenericTest()
        {
            // Arrange.
            var key = "085c8372-b741-4ea0-b7ac-7b7e69bf6638";
            var expectedIntValue = 101;
            var expectedStringValue = "String value new";

            var newItem = new SomeClass
            {
                Key = key,
                IntValue = expectedIntValue,
                StringValue = expectedStringValue,
                Tags = null
            };

            // Act.
            if (!CacheService.SetToCache(key, newItem))
            {
                throw new Exception("Error while adding item to cache.");
            }

            // Assert.
            var itemFromCache = CacheService.GetFromCache<SomeClass>(key);
            Assert.Equal(expectedIntValue, itemFromCache.IntValue);
            Assert.Equal(expectedStringValue, itemFromCache.StringValue);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.SetToCache{T}(string,T)"/> method in case of type of caching value is primitive.
        /// </summary>
        [Fact]
        public void SetToCacheGenericWithPrimitiveTypeTest()
        {
            // Arrange.
            var key = "085c8372-b741-4ea0-b7ac-7b7e69bf6638";
            int expectedValue = 101;

            // Act.
            if (!CacheService.SetToCache(key, expectedValue))
            {
                throw new Exception("Error while adding item to cache.");
            }

            // Assert.
            var itemFromCache = CacheService.GetFromCache<int>(key);
            Assert.Equal(expectedValue, itemFromCache);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.SetToCache{T}(string,T)"/> method in case of key is null or value is null.
        /// </summary>
        [Fact]
        public void SetToCacheGenericWhenKeyIsNullOrValueIsNullTest()
        {
            // Arrange.
            var key = "085c8372-b741-4ea0-b7ac-7b7e69bf6638";

            var newItem = new SomeClass
            {
                Key = "Some key value",
                StringValue = "Some value",
                IntValue = 101,
                Tags = null
            };

            var expectedParamNameForKey = "key";

            // Act.
            CacheService.SetToCache<SomeClass>(key, null);
            var exForNullKey = Assert.Throws<ArgumentNullException>(() => CacheService.SetToCache(null, newItem));

            // Assert.
            Assert.Equal(expectedParamNameForKey, exForNullKey.ParamName);

            var itemFromCache = CacheService.GetFromCache<SomeClass>(key);
            Assert.Equal(null, itemFromCache);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.SetToCache{T}(string,T,int)"/> method.
        /// </summary>
        [Fact]
        public void SetToCacheGenericExpirationTest()
        {
            // Arrange.
            var key = "085c8372-b741-4ea0-b7ac-7b7e69bf6638";
            var expectedIntValue = 101;
            var expectedStringValue = "String value new";
            var expiratoinTime = 3; // In seconds.

            var newItem = new SomeClass
            {
                Key = key,
                IntValue = expectedIntValue,
                StringValue = expectedStringValue,
                Tags = null
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
        /// Testing behavior of <see cref="MemoryCacheService.SetToCache{T}(string,T,int)"/> method in case of expiration time is negative.
        /// </summary>
        [Fact]
        public void SetToCacheGenericNegativeExpirationTimeTest()
        {
            // Arrange.
            var key = "085c8372-b741-4ea0-b7ac-7b7e69bf6638";
            var expiratoinTime = -1; // In seconds.
            var expectedParamNameForExpirationTime = "expirationTime";

            var newItem = new SomeClass
            {
                Key = "Some key value",
                StringValue = "Some value",
                IntValue = 101,
                Tags = null
            };

            // Act.
            var ex = Assert.Throws<ArgumentException>(() => CacheService.SetToCache(key, newItem, expiratoinTime));

            // Assert.
            Assert.Equal(expectedParamNameForExpirationTime, ex.ParamName);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.SetToCache{T}(string,T)"/> method in case of replacing existing item.
        /// </summary>
        [Fact]
        public void SetToCacheGenericWithItemReplacingTest()
        {
            // Arrange.
            var key = "3d10fe87-2f92-4ff2-9e13-fffe2d59dad1";
            var expectedIntValueBeforeReplace = 100;
            var expectedStringValueBeforeReplace = "String value 1";
            var expectedIntValueAfterReplace = 101;
            var expectedStringValueAfterReplace = "String value new";

            var newItem = new SomeClass
            {
                Key = key,
                IntValue = expectedIntValueAfterReplace,
                StringValue = expectedStringValueAfterReplace,
                Tags = null
            };

            // Act and Assert.

            // Check cached item before replace.
            var itemFromCache = CacheService.GetFromCache<SomeClass>(key);
            var intValueBeforeReplace = itemFromCache.IntValue;
            var stringValueBeforeReplace = itemFromCache.StringValue;
            Assert.Equal(intValueBeforeReplace, expectedIntValueBeforeReplace);
            Assert.Equal(stringValueBeforeReplace, expectedStringValueBeforeReplace);

            // Replace existing item.
            if (!CacheService.SetToCache(key, newItem))
            {
                throw new Exception("Error while adding item to cache.");
            }

            // Check cached item after replace.
            itemFromCache = CacheService.GetFromCache<SomeClass>(key);
            var intValueAfterReplace = itemFromCache.IntValue;
            var stringValueAfterReplace = itemFromCache.StringValue;
            Assert.Equal(expectedIntValueAfterReplace, intValueAfterReplace);
            Assert.Equal(expectedStringValueAfterReplace, stringValueAfterReplace);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetFromCache{T}"/> method.
        /// </summary>
        [Fact]
        public void GetFromCacheGenericTest()
        {
            // Arrange.
            var key = "3d10fe87-2f92-4ff2-9e13-fffe2d59dad1";
            var expectedIntValue = 100;
            var expectedStringValue = "String value 1";

            // Act.
            var item = CacheService.GetFromCache<SomeClass>(key);

            // Assert.
            Assert.Equal(expectedIntValue, item.IntValue);
            Assert.Equal(expectedStringValue, item.StringValue);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetFromCache{T}"/> method in case of key of getting item is not found in cache.
        /// </summary>
        [Fact]
        public void GetFromCacheGenericWhenKeyIsNotFoundTest()
        {
            // Arrange.

            var key = "some wrong key";
            var expectedExceptionMessage = $"Key \"{key}\" is not found in cache.";

            // Act.
            var ex = Assert.Throws<KeyNotFoundException>(() => CacheService.GetFromCache<SomeClass>(key));

            // Assert.
            Assert.Equal(expectedExceptionMessage, ex.Message);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetFromCache{T}"/> method in case of key of getting item is null.
        /// </summary>
        [Fact]
        public void GetFromCacheGenericWhenKeyIsNullTest()
        {
            // Arrange.
            var expectedParamNameForKey = "key";

            // Act.
            var ex = Assert.Throws<ArgumentNullException>(() => CacheService.GetFromCache<SomeClass>(null));

            // Assert.
            Assert.Equal(expectedParamNameForKey, ex.ParamName);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetFromCacheByTag{T}"/> method.
        /// </summary>
        [Fact]
        public void GetFromCacheByTagGenericTest()
        {
            // Arrange.
            var tag = "tag2";
            string[] expectedKeys =
            {
                "3d10fe87-2f92-4ff2-9e13-fffe2d59dad1",
                "b4f5ac0e-9766-4198-8914-90947816dcc2",
                "e9a8aa9c-a979-4616-a732-58eefdbd7d9e",
                "48ced5b3-92b1-40d4-bf5d-d946181c07d1",
                "618f9624-417e-47a1-a53e-d370d8ed8611"
            };
            var expectedItemsCount = 5;

            // Act.
            var items = CacheService.GetFromCacheByTag<SomeClass>(tag);

            // Assert.
            AssertItems(items, expectedKeys, new int[0], expectedItemsCount);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetFromCacheByTag{T}"/> method in case of tag is not exists.
        /// </summary>
        [Fact]
        public void GetFromCacheByTagGenericWithTagIsNotExistsTest()
        {
            // Arrange.
            var tag = "wrongTag";
            var expectedItemsCount = 0;

            // Act.
            var items = CacheService.GetFromCacheByTag<SomeClass>(tag);

            // Assert.
            AssertItems(items, null, new int[0], expectedItemsCount);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetFromCacheByTag{T}"/> method in case of tag of getting items is null.
        /// </summary>
        [Fact]
        public void GetFromCacheByTagGenericWhenTagIsNullTest()
        {
            // Arrange.
            var expectedParamNameForTag = "tags";

            // Act.
            var ex = Assert.Throws<ArgumentNullException>(() => CacheService.GetFromCacheByTag<SomeClass>(null));

            // Assert.
            Assert.Equal(expectedParamNameForTag, ex.ParamName);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetFromCacheByTags{T}"/> method.
        /// </summary>
        [Fact]
        public void GetFromCacheByTagsGenericTest()
        {
            // Arrange.
            var tags = new List<string> { "tag2", "tag4" };
            string[] expectedKeys =
            {
                "3d10fe87-2f92-4ff2-9e13-fffe2d59dad1",
                "b4f5ac0e-9766-4198-8914-90947816dcc2",
                "e9a8aa9c-a979-4616-a732-58eefdbd7d9e",
                "48ced5b3-92b1-40d4-bf5d-d946181c07d1",
                "618f9624-417e-47a1-a53e-d370d8ed8611",
                "526f8d7f-e535-48b0-b5c5-fc0c9e155089"
            };
            var expectedItemsCount = 6;

            // Act.
            var items = CacheService.GetFromCacheByTags<SomeClass>(tags);

            // Assert.
            AssertItems(items, expectedKeys, new int[0], expectedItemsCount);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetFromCacheByTags{T}"/> method in case of tags are not exists.
        /// </summary>
        [Fact]
        public void GetFromCacheByTagsGenericWithTagsAreNotExistsTest()
        {
            // Arrange.
            var tags1 = new List<string> { "wrongTag1", "wrongTag2" };
            var tags2 = new List<string> { "tag2", "wrongTag1", "wrongTag2" };
            var expectedItemsCountForTags1 = 0;

            string[] expectedKeys =
            {
                "3d10fe87-2f92-4ff2-9e13-fffe2d59dad1",
                "b4f5ac0e-9766-4198-8914-90947816dcc2",
                "e9a8aa9c-a979-4616-a732-58eefdbd7d9e",
                "48ced5b3-92b1-40d4-bf5d-d946181c07d1",
                "618f9624-417e-47a1-a53e-d370d8ed8611"
            };

            var expectedItemsCountForTags2 = 5;

            // Act.
            var items1 = CacheService.GetFromCacheByTags<SomeClass>(tags1);
            var items2 = CacheService.GetFromCacheByTags<SomeClass>(tags2);

            // Assert.
            AssertItems(items1, expectedKeys, new int[0], expectedItemsCountForTags1);
            AssertItems(items2, expectedKeys, new int[0], expectedItemsCountForTags2);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.GetFromCacheByTags{T}"/> method in case of tags of getting items are null.
        /// </summary>
        [Fact]
        public void GetFromCacheByTagGenericWhenTagsAreNullTest()
        {
            // Arrange.
            var expectedParamNameForTag = "tags";

            // Act.
            var ex = Assert.Throws<ArgumentNullException>(() => CacheService.GetFromCacheByTags<SomeClass>(null));

            // Assert.
            Assert.Equal(expectedParamNameForTag, ex.ParamName);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.UpdateInCache{T}"/> method.
        /// </summary>
        [Fact]
        public void UpdateInCacheGenericTest()
        {
            // Arrange.
            var key = "3d10fe87-2f92-4ff2-9e13-fffe2d59dad1";
            var expectedIntValueBeforeUpdate = 100;
            var expectedStringValueBeforeUpdate = "String value 1";
            string[] expectedTagsBeforeUpdateArray =
            {
                "tag1",
                "tag2"
            };

            var expectedIntValueAfterUpdate = 101;
            var expectedStringValueAfterUpdate = "String value new";
            var expiratoinTimeAfterUpdate = 3; // In seconds.
            var additionalTagsForUpdate = new List<string>
            {
                "tag5",
                "tag6"
            };

            var expectedTagsAfterUpdate = new List<string>
            {
                "tag1",
                "tag2",
                "tag5",
                "tag6"
            };

            var expectedTagsAfterUpdateArray = expectedTagsAfterUpdate.ToArray();

            var newItemForUpdate = new SomeClass
            {
                Key = key,
                IntValue = expectedIntValueAfterUpdate,
                StringValue = expectedStringValueAfterUpdate,
                Tags = expectedTagsAfterUpdate
            };

            // Act and Assert.

            // Initially expiration time is not set, so check that item is not removed from cache after some pause.
            Assert.True(CacheService.Exists(key));

            // Sleep specified time.
            Thread.Sleep(expiratoinTimeAfterUpdate * 1000 + 1000);

            // Check that item is still exists in cache.
            Assert.True(CacheService.Exists(key));

            // Check cached item before update.
            var itemFromCache = CacheService.GetFromCache<SomeClass>(key);
            AssertSomeClass(itemFromCache, expectedIntValueBeforeUpdate, expectedStringValueBeforeUpdate);

            var tagsBeforeUpdate = CacheService.GetTagsForItem(key);
            AssertTags(tagsBeforeUpdate, expectedTagsBeforeUpdateArray, expectedTagsBeforeUpdateArray.Length);

            // Update existing item.
            if (!CacheService.UpdateInCache(key, newItemForUpdate, expiratoinTimeAfterUpdate, additionalTagsForUpdate))
            {
                throw new Exception("Error while updating item in cache.");
            }

            // Check cached item after update.
            itemFromCache = CacheService.GetFromCache<SomeClass>(key);
            AssertSomeClass(itemFromCache, expectedIntValueAfterUpdate, expectedStringValueAfterUpdate);

            var tagsAfterUpdate = CacheService.GetTagsForItem(key);
            AssertTags(tagsAfterUpdate, expectedTagsAfterUpdateArray, expectedTagsAfterUpdateArray.Length);

            Assert.True(CacheService.Exists(key));

            // Sleep for one more second after expiration time.
            Thread.Sleep(expiratoinTimeAfterUpdate * 1000 + 1000);

            Assert.False(CacheService.Exists(key));
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.UpdateInCache{T}"/> method in case of key is null or value is null.
        /// </summary>
        [Fact]
        public void UpdateInCacheGenericWhenKeyIsNullOrValueIsNullTest()
        {
            // Arrange.
            var key = "3d10fe87-2f92-4ff2-9e13-fffe2d59dad1";

            var newItemForUpdate = new SomeClass
            {
                Key = key,
                IntValue = 101,
                StringValue = "String value new",
                Tags = null
            };

            var expectedParamNameForKey = "key";

            // Act.
            CacheService.UpdateInCache<SomeClass>(key, null, 0, null);
            var exForNullKey = Assert.Throws<ArgumentNullException>(() => CacheService.UpdateInCache(null, newItemForUpdate, 0, null));

            // Assert.
            Assert.Equal(expectedParamNameForKey, exForNullKey.ParamName);

            var itemFromCache = CacheService.GetFromCache<SomeClass>(key);
            Assert.Equal(null, itemFromCache);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.UpdateInCache{T}"/> method in case of expiration time is negative.
        /// </summary>
        [Fact]
        public void UpdateInCacheGenericNegativeExpirationTimeTest()
        {
            // Arrange.
            var key = "085c8372-b741-4ea0-b7ac-7b7e69bf6638";
            var expiratoinTime = -1; // In seconds.
            var expectedParamNameForExpirationTime = "expirationTime";

            var newItemForUpdate = new SomeClass
            {
                Key = "Some key value",
                StringValue = "Some value",
                IntValue = 101,
                Tags = null
            };

            // Act.
            var ex = Assert.Throws<ArgumentException>(() => CacheService.UpdateInCache(key, newItemForUpdate, expiratoinTime, null));

            // Assert.
            Assert.Equal(expectedParamNameForExpirationTime, ex.ParamName);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.UpdateInCache{T}"/> method in case of additional tags are null or empty.
        /// </summary>
        [Fact]
        public void UpdateInCacheGenericWithAdditionalTagsAreNullOrEmptyTest()
        {
            // Arrange.
            var key = "3d10fe87-2f92-4ff2-9e13-fffe2d59dad1";
            string[] expectedTagsBeforeUpdateArray =
            {
                "tag1",
                "tag2"
            };

            var expectedIntValueAfterUpdate = 101;
            var expectedStringValueAfterUpdate = "String value new";
            var additionalTagsForUpdate = new List<string>();

            var expectedTagsAfterUpdate = new List<string>
            {
                "tag1",
                "tag2"
            };

            var expectedTagsAfterUpdateArray = expectedTagsAfterUpdate.ToArray();

            var newItemForUpdate = new SomeClass
            {
                Key = key,
                IntValue = expectedIntValueAfterUpdate,
                StringValue = expectedStringValueAfterUpdate,
                Tags = expectedTagsAfterUpdate
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
        /// Testing behavior of <see cref="MemoryCacheService.TryGetFromCache{T}"/> method.
        /// </summary>
        [Fact]
        public void TryGetFromCacheGenericTest()
        {
            // Arrange.
            var key = "3d10fe87-2f92-4ff2-9e13-fffe2d59dad1";
            var expectedIntValue = 100;
            var expectedStringValue = "String value 1";

            var wrongKey = "i'm wrong";

            // Act.
            SomeClass itemOfSomeClassType;
            var itemOfSomeClassTypeWasSuccessfullyGotten = CacheService.TryGetFromCache(key, out itemOfSomeClassType);

            SomeClass someObjectWithWrongKey;
            var itemWithWrongKeyWasSuccessfullyGotten = CacheService.TryGetFromCache(wrongKey, out someObjectWithWrongKey);

            SomeClass someObjectForNull;
            var itemOfNullTypeWasSuccessfullyGotten = CacheService.TryGetFromCache(null, out someObjectForNull);

            // Assert.
            Assert.True(itemOfSomeClassTypeWasSuccessfullyGotten);
            Assert.False(itemWithWrongKeyWasSuccessfullyGotten);
            Assert.False(itemOfNullTypeWasSuccessfullyGotten);

            Assert.Equal(expectedIntValue, itemOfSomeClassType.IntValue);
            Assert.Equal(expectedStringValue, itemOfSomeClassType.StringValue);

            Assert.Equal(null, someObjectWithWrongKey);
            Assert.Equal(null, someObjectForNull);
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.TryGetFromCacheByTag{T}"/> method.
        /// </summary>
        [Fact]
        public void TryGetFromCacheByTagGenericTest()
        {
            // Arrange.
            var tag = "tag2";
            string[] expectedKeys =
            {
                "3d10fe87-2f92-4ff2-9e13-fffe2d59dad1",
                "b4f5ac0e-9766-4198-8914-90947816dcc2",
                "e9a8aa9c-a979-4616-a732-58eefdbd7d9e",
                "48ced5b3-92b1-40d4-bf5d-d946181c07d1",
                "618f9624-417e-47a1-a53e-d370d8ed8611"
            };

            var expectedItemsCount = 5;

            var wrongTag = "i'm wrong";

            // Act.
            IEnumerable<SomeClass> itemsWithExistingTag;
            var itemsOfAnonymousTypeWasSuccessfullyGotten = CacheService.TryGetFromCacheByTag(tag, out itemsWithExistingTag);

            IEnumerable<SomeClass> someObjectsWithWrongTag;
            var itemsWithWrongTagWasSuccessfullyGotten = CacheService.TryGetFromCacheByTag(wrongTag, out someObjectsWithWrongTag);

            IEnumerable<SomeClass> someObjectsForNull;
            var itemsOfNullTypeWasSuccessfullyGotten = CacheService.TryGetFromCacheByTag(null, out someObjectsForNull);

            // Assert.
            AssertItems(itemsWithExistingTag, expectedKeys, new int[0], expectedItemsCount);
            Assert.True(itemsOfAnonymousTypeWasSuccessfullyGotten);
            Assert.True(itemsWithWrongTagWasSuccessfullyGotten);
            Assert.False(itemsOfNullTypeWasSuccessfullyGotten);

            Assert.Equal(0, someObjectsWithWrongTag.Count());
            Assert.Equal(0, someObjectsForNull.Count());
        }

        /// <summary>
        /// Testing behavior of <see cref="MemoryCacheService.TryGetFromCacheByTags{T}"/> method.
        /// </summary>
        [Fact]
        public void TryGetFromCacheByTagsGenericTest()
        {
            // Arrange.
            var tags1 = new List<string> { "tag2", "tag4" };
            string[] expectedKeys1 =
            {
                "3d10fe87-2f92-4ff2-9e13-fffe2d59dad1",
                "b4f5ac0e-9766-4198-8914-90947816dcc2",
                "e9a8aa9c-a979-4616-a732-58eefdbd7d9e",
                "48ced5b3-92b1-40d4-bf5d-d946181c07d1",
                "618f9624-417e-47a1-a53e-d370d8ed8611",
                "526f8d7f-e535-48b0-b5c5-fc0c9e155089"
            };

            var expectedItemsCountForTags1 = 6;

            var tags2 = new List<string> { "tag2", "wrongTag1", "wrongTag2" };

            string[] expectedKeys2 =
            {
                "3d10fe87-2f92-4ff2-9e13-fffe2d59dad1",
                "b4f5ac0e-9766-4198-8914-90947816dcc2",
                "e9a8aa9c-a979-4616-a732-58eefdbd7d9e",
                "48ced5b3-92b1-40d4-bf5d-d946181c07d1",
                "618f9624-417e-47a1-a53e-d370d8ed8611",
            };

            var expectedItemsCountForTags2 = 5;

            var tags3 = new List<string> { "wrongTag1", "wrongTag2" };
            var expectedItemsCountForTags3 = 0;

            // Act.
            IEnumerable<SomeClass> itemsWithExistingTags;
            var itemsWithExistingTagsWasSuccessfullyGotten = CacheService.TryGetFromCacheByTags(tags1, out itemsWithExistingTags);

            IEnumerable<SomeClass> someObjectsWithSomeWrongTags;
            var itemsWithSomeWrongTagsWasSuccessfullyGotten = CacheService.TryGetFromCacheByTags(tags2, out someObjectsWithSomeWrongTags);

            IEnumerable<SomeClass> someObjectsWithWrongTags;
            var itemsWithWrongTagsWasSuccessfullyGotten = CacheService.TryGetFromCacheByTags(tags3, out someObjectsWithWrongTags);

            IEnumerable<SomeClass> someObjectsForNull;
            var itemsOfNullTypeWasSuccessfullyGotten = CacheService.TryGetFromCacheByTags(null, out someObjectsForNull);

            // Assert.
            AssertItems(itemsWithExistingTags, expectedKeys1, new int[0], expectedItemsCountForTags1);
            AssertItems(someObjectsWithSomeWrongTags, expectedKeys2, new int[0], expectedItemsCountForTags2);
            AssertItems(someObjectsWithWrongTags, null, new int[0], expectedItemsCountForTags3);
            AssertItems(someObjectsForNull, null, new int[0], 0);
            Assert.True(itemsWithExistingTagsWasSuccessfullyGotten);
            Assert.True(itemsWithSomeWrongTagsWasSuccessfullyGotten);
            Assert.True(itemsWithWrongTagsWasSuccessfullyGotten);
            Assert.False(itemsOfNullTypeWasSuccessfullyGotten);

        }
    }
}
