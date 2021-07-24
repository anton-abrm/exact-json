// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using Moq;

using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    public sealed class JsonStringTests
    {
        [TestCase("")]
        [TestCase("foo")]
        public void Create_GivenNonNull_ValueInitialized(string value)
        {
            var s = JsonString.Create(value);

            Assert.That(s, Is.Not.Null);
            Assert.That(s.Value, Is.EqualTo(value));
            Assert.That(s.NodeType, Is.EqualTo(JsonNodeType.String));
        }

        [Test]
        public void Create_GivenNull_ArgumentNullExceptionThrown()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                JsonString.Create(null);
            });
        }

        [TestCase("", "", true)]
        [TestCase("a", "b", false)]
        [TestCase("a", "a", true)]
        public void Equals_GivenNonNull_MatchExpected(string a, string b, bool expected)
        {
            var ja = JsonString.Create(a);
            var jb = JsonString.Create(b);

            bool result = ja.Equals((object) jb);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Equals_GivenSameInstances_ValuesEqual()
        {
            var a = JsonString.Create("foo");
            var b = a;

            bool result = a.Equals((object) b);

            Assert.That(result, Is.True);
        }

        [Test]
        public void Equals_GivenInstanceAndNull_ValuesNotEqual()
        {
            var a = JsonString.Create("foo");

            bool result = a.Equals((object) null);

            Assert.That(result, Is.False);
        }

        [TestCase("", "")]
        [TestCase("a", "a")]
        public void GetHashCode_GivenEqualValues_HashesMatch(string a, string b)
        {
            int hashA = JsonString.Create(a).GetHashCode();
            int hashB = JsonString.Create(b).GetHashCode();

            Assert.That(hashA, Is.EqualTo(hashB));
        }

        [TestCase("foo")]
        public void Write_GivenNonNull_WriteStringCalled(string value)
        {
            var mock = new Mock<JsonWriter>();

            JsonString.Create(value).WriteTo(mock.Object);

            mock.Verify(w => w.WriteString(value), Times.Once);
            mock.VerifyNoOtherCalls();
        }

        [Test]
        public void Write_GivenNull_ArgumentNullExceptionThrown()
        {
            var value = JsonString.Create("");

            Assert.Throws<ArgumentNullException>(
                () => value.WriteTo((JsonWriter) null));
        }
    }
}