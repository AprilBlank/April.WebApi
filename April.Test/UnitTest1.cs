using System;
using System.Collections.Generic;
using Xunit;

namespace April.Test
{
    public class UnitTest1
    {
        [Fact]
        public void TestEqual()
        {
            int a = 10, b = 20;
            Assert.Equal(30, Add(a, b));
        }

        [Fact]
        public void TestNull()
        {
            object obj = new { id = 1 };
            Assert.NotNull(obj);
        }

        [Theory]
        [InlineData(new object[] { 1, 2, 3, 4 }, 1)]
        [InlineData(new object[] { "t", "e", "s", "t" }, "t")]
        public void TestContains(object[] objs, object obj)
        {
            Assert.Contains(obj, objs);
        }

        [Theory]
        [MemberData(nameof(tempDatas))]
        public void TestData(int a, int b)
        {
            int result = a + b;
            Assert.True(result == Add(a, b));
        }

        private int Add(int a, int b)
        {
            return a + b;
        }

        public static IEnumerable<object[]> tempDatas
        {
            get
            {
                yield return new object[] { 1, 2 };
                yield return new object[] { 5, 7 };
                yield return new object[] { 12, 12 };
            }
        }
    }
}
