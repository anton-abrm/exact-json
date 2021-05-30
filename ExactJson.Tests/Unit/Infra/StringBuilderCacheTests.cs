using System;

using ExactJson.Infra;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Infra
{
    internal sealed class StringBuilderCacheTests
    {
        [Test]
        public void Release_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => StringBuilderCache.Release(null));
        }

        [Test]
        public void GetStringAndRelease_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => StringBuilderCache.GetStringAndRelease(null));
        }
    }
}