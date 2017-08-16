namespace NewPlatform.Flexberry.Caching.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using Xunit;

    public partial class MemoryCacheServiceTest
    {
        private void AssertItems(IEnumerable<object> testingCollection, string[] expectedKeys, int[] expectedValues, int expectedCollectionLength)
        {
            if (testingCollection == null)
            {
                throw new ArgumentNullException(nameof(testingCollection));
            }

            var enumerable = testingCollection as object[] ?? testingCollection.ToArray();
            Assert.Equal(expectedCollectionLength, enumerable.Length);

            foreach (var item in enumerable)
            {
                var @class = item as SomeClass;
                if (@class != null)
                {
                    var key = @class.Key;
                    Assert.Contains(key, expectedKeys);
                }
                else
                {
                    var value = (int)item.GetType().GetProperty("Value", typeof(int)).GetValue(item);
                    Assert.Contains(value, expectedValues);
                }
            }
        }

        private void AssertItems(IEnumerable<object> testingCollection, string[] expectedKeys, string[] expectedValues, int expectedCollectionLength)
        {
            if (testingCollection == null)
            {
                throw new ArgumentNullException(nameof(testingCollection));
            }

            var enumerable = testingCollection as object[] ?? testingCollection.ToArray();
            Assert.Equal(expectedCollectionLength, enumerable.Length);

            foreach (var item in enumerable)
            {
                var @class = item as SomeClass;
                if (@class != null)
                {
                    var key = @class.Key;
                    Assert.Contains(key, expectedKeys);
                }
                else
                {
                    var value = (string)item.GetType().GetProperty("Value", typeof(string)).GetValue(item);
                    Assert.Contains(value, expectedValues);
                }
            }
        }
        private void AssertTags(IEnumerable<string> testingCollection, string[] expectedTags, int expectedCollectionLength)
        {
            if (testingCollection == null)
            {
                throw new ArgumentNullException(nameof(testingCollection));
            }

            var enumerable = testingCollection as string[] ?? testingCollection.ToArray();
            Assert.Equal(expectedCollectionLength, enumerable.Length);

            foreach (var item in enumerable)
            {
                Assert.Contains(item, expectedTags);
            }
        }

        private void AssertSomeClass(SomeClass itemFromCache, int expectedIntValue, string expectedStringValue)
        {
            var intValueBeforeUppdate = itemFromCache.IntValue;
            var stringValueBeforeUpdate = itemFromCache.StringValue;
            Assert.Equal(intValueBeforeUppdate, expectedIntValue);
            Assert.Equal(stringValueBeforeUpdate, expectedStringValue);
        }
    }
}
