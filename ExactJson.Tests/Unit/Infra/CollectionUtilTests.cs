// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using ExactJson.Infra;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Infra
{
    internal sealed class CollectionUtilTests
    {
        [Test]
        public void CopyTo_SourceNull_ThrowsArgumentNullException()
        {
            var dst = new int[0];

            Assert.Throws<ArgumentNullException>(()
                => CollectionUtil.CopyTo(null, dst, 0));
        }

        [Test]
        public void CopyTo_ArrayNull_ThrowsArgumentNullException()
        {
            var src = new int[0];

            Assert.Throws<ArgumentNullException>(()
                => CollectionUtil.CopyTo(src,  null, 0));
        }

        [TestCase(-1)]
        [TestCase(1)]
        public void CopyTo_ArrayIndexOutOfRange_ThrowsArgumentOutOfRangeException(int arrayIndex)
        {
            var src = new int[0];
            var dst = new int[0];

            Assert.Throws<ArgumentOutOfRangeException>(()
                => CollectionUtil.CopyTo(src, dst, arrayIndex));
        }

        [Test]
        public void CopyTo_NotEnoughSpace_ThrowsArgumentException()
        {
            var src = new int[10];
            var dst = new int[10];

            Assert.Throws<ArgumentException>(()
                => CollectionUtil.CopyTo(src,  dst, 1));
        }

        [Test]
        public void CopyTo_ExtraSpace()
        {
            var src = new int[3] { 1, 2, 3 };
            var dst = new int[5];

            CollectionUtil.CopyTo(src, dst, 1);

            Assert.That(dst, Is.EquivalentTo(new[] { 0, 1, 2, 3, 0 }));
        }

        [Test]
        public void CopyTo_Fits()
        {
            var src = new int[3] { 1, 2, 3 };
            var dst = new int[3];

            CollectionUtil.CopyTo(src, dst, 0);

            Assert.That(dst, Is.EquivalentTo(new[] { 1, 2, 3 }));
        }

        [Test]
        public void CopyTo_Zero()
        {
            var src = new int[0];
            var dst = new int[0];

            CollectionUtil.CopyTo(src, dst, 0);

            Assert.That(dst, Is.EquivalentTo(new int[0]));
        }
    }
}