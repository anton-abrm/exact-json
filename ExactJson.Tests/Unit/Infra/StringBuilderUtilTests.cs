// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;

using ExactJson.Infra;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Infra
{
    public class StringBuilderUtilTests
    {
        [TestCase("", "")]
        [TestCase("1", "1")]
        [TestCase("12", "21")]
        [TestCase("123", "321")]
        public void Reverse_StringBuilder_ReversesBuilder(string value, string expected)
        {
            var sb = new StringBuilder(value);

            StringBuilderUtil.Reverse(sb, 0, sb.Length);

            Assert.That(sb.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void Reverse_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => StringBuilderUtil.Reverse(null, 0, 0));
        }
    }
}