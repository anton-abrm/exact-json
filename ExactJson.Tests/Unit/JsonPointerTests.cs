// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    internal sealed class JsonPointerTests
    {
        [Test]
        public void Evaluate_FromRfc_Root()
        {
            var jo = new JsonObject();

            Assert.That(jo.EvaluatePointer(""), Is.SameAs(jo));
        }

        [TestCase("/foo", "[\"bar\",\"baz\"]")]
        [TestCase("/foo/0", "\"bar\"")]
        [TestCase("/", "0")]
        [TestCase("/a~1b", "1")]
        [TestCase("/c%d", "2")]
        [TestCase("/e^f", "3")]
        [TestCase("/g|h", "4")]
        [TestCase("/i\\j", "5")]
        [TestCase("/ ", "7")]
        [TestCase("/m~0n", "8")]
        public void Evaluate_FromRfc(string pointerString, string expectedJson)
        {
            var jo = new JsonObject
            {
                ["foo"] = new JsonArray { "bar", "baz" },
                [""] = 0,
                ["a/b"] = 1,
                ["c%d"] = 2,
                ["e^f"] = 3,
                ["g|h"] = 4,
                ["i\\j"] = 5,
                ["k\"l"] = 6,
                [" "] = 7,
                ["m~n"] = 8,
            };

            var pointer = JsonPointer.Parse(pointerString);

            Assert.That(pointer.Evaluate(jo).ToString(), Is.EqualTo(expectedJson));
        }

        [TestCase("/0", "0")]
        [TestCase("/1", null)]
        [TestCase("/a/0", "0")]
        [TestCase("/a/1", null)]
        [TestCase("/foo", "\"bar\"")]
        [TestCase("/bar", null)]
        public void Evaluate(string pointerString, string expectedJson)
        {
            var jo = new JsonObject
            {
                ["0"] = 0,
                ["a"] = new JsonArray { 0 },
                ["foo"] = "bar",
            };

            var pointer = JsonPointer.Parse(pointerString);

            Assert.That(pointer.Evaluate(jo)?.ToString(), Is.EqualTo(expectedJson));
        }

        [Test]
        public void Evaluate_Null_ThrowsArgumentNullException()
        {
            var pointer = JsonPointer.Root();

            Assert.Throws<ArgumentNullException>(()
                => pointer.Evaluate(null));
        }

        [TestCase("")]
        [TestCase("/")]
        [TestCase("/foo/0")]
        [TestCase("/a~1b")]
        [TestCase("/m~0n")]
        public void ParseAndToString(string pointer)
        {
            Assert.That(JsonPointer.Parse(pointer).ToString(), Is.EqualTo(pointer));
        }

        [Test]
        public void Parse_Root()
        {
            Assert.That(JsonPointer.Parse(""), Is.EqualTo(JsonPointer.Root()));
        }

        [TestCase("foo")]
        public void Parse_Invalid_ThrowsFormatException(string value)
        {
            Assert.Throws<FormatException>(()
                => JsonPointer.Parse(value));
        }

        [Test]
        public void Parse_EmptyProperty()
        {
            Assert.That(JsonPointer.Parse("/"), Is.EqualTo(JsonPointer.Root().Attach("")));
        }

        [Test]
        public void Parse_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(()
                => JsonPointer.Parse(null));
        }

        [TestCase("", "", true)]
        [TestCase("/foo/0", "/foo/0", true)]
        [TestCase("/foo/0", "/foo/1", false)]
        [TestCase("/foo/0", "/bar/0", false)]
        [TestCase("/foo/0", "/0/0", false)]
        [TestCase("/foo/0", "/foo/foo", false)]
        public void EqualsAndGetHashCode(string s1, string s2, bool equal)
        {
            var p1 = JsonPointer.Parse(s1);
            var p2 = JsonPointer.Parse(s2);

            Assert.That(p1.Equals((object) p2), Is.EqualTo(equal));

            if (equal) {
                Assert.That(p1.GetHashCode(), Is.EqualTo(p2.GetHashCode()));
            }
        }

        [Test]
        public void Attach_String_Null_ThrowsArgumentNUllException()
        {
            Assert.Throws<ArgumentNullException>(()
                => JsonPointer.Root().Attach(null));
        }

        [Test]
        public void Attach()
        {
            var pointer = JsonPointer.Root()
                                     .Attach("foo")
                                     .Attach(1);

            Assert.That(pointer.ToString(), Is.EqualTo("/foo/1"));
        }
    }
}