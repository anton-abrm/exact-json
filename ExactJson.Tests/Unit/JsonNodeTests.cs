// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.IO;

using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    internal sealed class JsonNodeTests
    {
        #region Implicit Casting

        [TestCase("foo")]
        public void Cast_Implicit_Boolean(string value)
        {
            Assert.That((JsonNode) value, Is.EqualTo(JsonString.Create(value)));
            Assert.That((JsonNode) (string) null, Is.EqualTo(JsonNull.Create()));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Cast_Implicit_Boolean(bool value)
        {
            Assert.That((JsonNode) value, Is.EqualTo(JsonBool.Create(value)));
            Assert.That((JsonNode) (bool?) value, Is.EqualTo(JsonBool.Create(value)));
            Assert.That((JsonNode) (bool?) null, Is.EqualTo(JsonNull.Create()));
        }

        [TestCase((byte) 0)]
        public void Cast_Implicit_UInt8(byte value)
        {
            Assert.That((JsonNode) value, Is.EqualTo(JsonNumber.Create(value)));
            Assert.That((JsonNode) (byte?) value, Is.EqualTo(JsonNumber.Create(value)));
            Assert.That((JsonNode) (byte?) null, Is.EqualTo(JsonNull.Create()));
        }

        [TestCase((ushort) 0)]
        public void Cast_Implicit_UInt16(ushort value)
        {
            Assert.That((JsonNode) value, Is.EqualTo(JsonNumber.Create(value)));
            Assert.That((JsonNode) (ushort?) value, Is.EqualTo(JsonNumber.Create(value)));
            Assert.That((JsonNode) (ushort?) null, Is.EqualTo(JsonNull.Create()));
        }

        [TestCase(0u)]
        public void Cast_Implicit_UInt32(uint value)
        {
            Assert.That((JsonNode) value, Is.EqualTo(JsonNumber.Create(value)));
            Assert.That((JsonNode) (uint?) value, Is.EqualTo(JsonNumber.Create(value)));
            Assert.That((JsonNode) (uint?) null, Is.EqualTo(JsonNull.Create()));
        }

        [TestCase(0ul)]
        public void Cast_Implicit_UInt64(ulong value)
        {
            Assert.That((JsonNode) value, Is.EqualTo(JsonNumber.Create(value)));
            Assert.That((JsonNode) (ulong?) value, Is.EqualTo(JsonNumber.Create(value)));
            Assert.That((JsonNode) (ulong?) null, Is.EqualTo(JsonNull.Create()));
        }

        [TestCase((sbyte) 0)]
        public void Cast_Implicit_Int8(sbyte value)
        {
            Assert.That((JsonNode) value, Is.EqualTo(JsonNumber.Create(value)));
            Assert.That((JsonNode) (sbyte?) value, Is.EqualTo(JsonNumber.Create(value)));
            Assert.That((JsonNode) (sbyte?) null, Is.EqualTo(JsonNull.Create()));
        }

        [TestCase((short) 0)]
        public void Cast_Implicit_Int16(short value)
        {
            Assert.That((JsonNode) value, Is.EqualTo(JsonNumber.Create(value)));
            Assert.That((JsonNode) (short?) value, Is.EqualTo(JsonNumber.Create(value)));
            Assert.That((JsonNode) (short?) null, Is.EqualTo(JsonNull.Create()));
        }

        [TestCase(0)]
        public void Cast_Implicit_Int32(int value)
        {
            Assert.That((JsonNode) value, Is.EqualTo(JsonNumber.Create(value)));
            Assert.That((JsonNode) (int?) value, Is.EqualTo(JsonNumber.Create(value)));
            Assert.That((JsonNode) (int?) null, Is.EqualTo(JsonNull.Create()));
        }

        [TestCase(0L)]
        public void Cast_Implicit_Int64(long value)
        {
            Assert.That((JsonNode) value, Is.EqualTo(JsonNumber.Create(value)));
            Assert.That((JsonNode) (long?) value, Is.EqualTo(JsonNumber.Create(value)));
            Assert.That((JsonNode) (long?) null, Is.EqualTo(JsonNull.Create()));
        }

        [TestCase(0.0)]
        public void Cast_Implicit_Double(double value)
        {
            Assert.That((JsonNode) value, Is.EqualTo(JsonNumber.Create(value)));
            Assert.That((JsonNode) (double?) value, Is.EqualTo(JsonNumber.Create(value)));
            Assert.That((JsonNode) (double?) null, Is.EqualTo(JsonNull.Create()));
        }

        [TestCase(0.0f)]
        public void Cast_Implicit_Single(float value)
        {
            Assert.That((JsonNode) value, Is.EqualTo(JsonNumber.Create(value)));
            Assert.That((JsonNode) (float?) value, Is.EqualTo(JsonNumber.Create(value)));
            Assert.That((JsonNode) (float?) null, Is.EqualTo(JsonNull.Create()));
        }

        [TestCase("0.0")]
        public void Cast_Implicit_Decimal(string s)
        {
            decimal value = decimal.Parse(s, CultureInfo.InvariantCulture);

            Assert.That((JsonNode) value, Is.EqualTo(JsonNumber.Create(value)));
            Assert.That((JsonNode) (decimal?) value, Is.EqualTo(JsonNumber.Create(value)));
            Assert.That((JsonNode) (decimal?) null, Is.EqualTo(JsonNull.Create()));
        }

        #endregion

        [Test]
        public void Parse()
        {
            var expected = new JsonObject
            {
                ["_i"] = 1,
                ["_t"] = true,
                ["_f"] = false,
                ["_n"] = (string) null,
                ["_s"] = "foo",
                ["_a"] = new JsonArray { 1, 2, 3 },
            };

            var parsed = JsonNode.Parse(
                "{" +
                "\"_i\":1," +
                "\"_t\":true," +
                "\"_f\":false," +
                "\"_n\":null," +
                "\"_s\":\"foo\"," +
                "\"_a\":[1,2,3]" +
                "}");

            Assert.That(parsed.DeepEquals(expected), Is.True);
        }

        [Test]
        public void Load_EndObject_ThrowsInvalidOperationException()
        {
            using var jr = new JsonStringReader("{}");

            Assert.That(jr.Read(), Is.True);
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.StartObject));

            Assert.That(jr.Read(), Is.True);
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.EndObject));

            Assert.Throws<InvalidOperationException>(() => JsonNode.Load(jr));
        }

        [Test]
        public void Load_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() 
                => JsonNode.Load(null));
        }

        [Test]
        public void Load_EOF_ThrowsEndStreamException()
        {
            Assert.Throws<EndOfStreamException>(() 
                => JsonNode.Load(new JsonStringReader("")));
        }

        [Test]
        public void Load_JsonReaderExceptionThrown_ThrowsJsonNodeLoadException()
        {
            using var jr = new JsonStringReader("[[}");

            var ex = Assert.Throws<JsonNodeLoadException>(() => JsonNode.Load(jr));

            Assert.That(ex.Pointer, Is.EqualTo("/0"));
        }

        [Test]
        public void WriteTo_ViaToString()
        {
            var node = new JsonObject
            {
                ["_i"] = 1,
                ["_t"] = true,
                ["_f"] = false,
                ["_n"] = (string) null,
                ["_s"] = "foo",
                ["_a"] = new JsonArray { 1, 2, 3 },
            };

            Assert.That(node.ToString(), Is.EqualTo(
                "{" +
                "\"_i\":1," +
                "\"_t\":true," +
                "\"_f\":false," +
                "\"_n\":null," +
                "\"_s\":\"foo\"," +
                "\"_a\":[1,2,3]" +
                "}"));
        }

        [Test]
        public void WriteTo_TextWriter_Null_ThrowsArgumentNullException()
        {
            var node = new JsonObject();

            Assert.Throws<ArgumentNullException>(
                () => node.WriteTo((TextWriter) null));
        }

        [Test]
        public void WriteTo_JsonWriter_Null_ThrowsArgumentNullException()
        {
            var node = new JsonObject();

            Assert.Throws<ArgumentNullException>(
                () => node.WriteTo((JsonWriter) null));
        }

        [Test]
        public void EvaluatePointer_JsonPointer_Null_ThrowsArgumentNullException()
        {
            var node = new JsonObject();

            Assert.Throws<ArgumentNullException>(
                () => node.EvaluatePointer((JsonPointer) null));
        }

        [Test]
        public void EvaluatePointer_String_Null_ThrowsArgumentNullException()
        {
            var node = new JsonObject();

            Assert.Throws<ArgumentNullException>(
                () => node.EvaluatePointer((string) null));
        }

        [Test]
        public void EvaluatePointer()
        {
            var node = new JsonObject();

            Assert.That(node.EvaluatePointer(""), Is.SameAs(node));
        }

        [Test]
        public void Diff_Equals()
        {
            var node1 = new JsonObject
            {
                ["_i"] = 1,
                ["_t"] = true,
                ["_f"] = false,
                ["_n"] = (string) null,
                ["_s"] = "foo",
                ["_a"] = new JsonArray { 1, 2, 3 },
            };

            var node2 = new JsonObject
            {
                ["_i"] = 1,
                ["_t"] = true,
                ["_f"] = false,
                ["_n"] = (string) null,
                ["_s"] = "foo",
                ["_a"] = new JsonArray { 1, 2, 3 },
            };

            var diffs = node1.Diff(node2);

            Assert.That(diffs.Length, Is.EqualTo(0));
        }

        [TestCase("1", "2")]
        [TestCase("1", null)]
        [TestCase("1", "true")]
        public void Diff_Value(string left, string right)
        {
            var n1 = JsonNode.Parse(left);
            var n2 = right is not null
                ? JsonNode.Parse(right)
                : null;

            var diffs = n1.Diff(n2);

            Assert.That(diffs.Length, Is.EqualTo(1));

            Assert.That(diffs[0].Pointer, Is.EqualTo(JsonPointer.Root()));
            Assert.That(diffs[0].Self, Is.SameAs(n1));
            Assert.That(diffs[0].Other, Is.SameAs(n2));
        }

        [Test]
        public void Diff_Array_Array_ExtraOnOther()
        {
            var n1 = new JsonArray { 1, 2 };
            var n2 = new JsonArray { 1, 3, 4 };

            var diffs = n1.Diff(n2);

            Assert.That(diffs.Length, Is.EqualTo(2));

            Assert.That(diffs[0].Pointer, Is.EqualTo(JsonPointer.Parse("/1")));
            Assert.That(diffs[0].Self, Is.EqualTo(JsonNumber.Create(2)));
            Assert.That(diffs[0].Other, Is.EqualTo(JsonNumber.Create(3)));

            Assert.That(diffs[1].Pointer, Is.EqualTo(JsonPointer.Parse("/2")));
            Assert.That(diffs[1].Self, Is.Null);
            Assert.That(diffs[1].Other, Is.EqualTo(JsonNumber.Create(4)));
        }

        [Test]
        public void Diff_Array_Array_ExtraOnSelf()
        {
            var n1 = new JsonArray { 1, 2, 4 };
            var n2 = new JsonArray { 1, 3 };

            var diffs = n1.Diff(n2);

            Assert.That(diffs.Length, Is.EqualTo(2));

            Assert.That(diffs[0].Pointer, Is.EqualTo(JsonPointer.Parse("/1")));
            Assert.That(diffs[0].Self, Is.EqualTo(JsonNumber.Create(2)));
            Assert.That(diffs[0].Other, Is.EqualTo(JsonNumber.Create(3)));

            Assert.That(diffs[1].Pointer, Is.EqualTo(JsonPointer.Parse("/2")));
            Assert.That(diffs[1].Self, Is.EqualTo(JsonNumber.Create(4)));
            Assert.That(diffs[1].Other, Is.Null);
        }

        [Test]
        public void Diff_Array_None()
        {
            var n1 = new JsonArray();

            var diffs = n1.Diff(null);

            Assert.That(diffs.Length, Is.EqualTo(1));

            Assert.That(diffs[0].Pointer, Is.EqualTo(JsonPointer.Root()));
            Assert.That(diffs[0].Self, Is.SameAs(n1));
            Assert.That(diffs[0].Other, Is.Null);
        }

        [Test]
        public void Diff_Array_Object()
        {
            var n1 = new JsonArray();
            var n2 = new JsonObject();

            var diffs = n1.Diff(n2);

            Assert.That(diffs.Length, Is.EqualTo(1));

            Assert.That(diffs[0].Pointer, Is.EqualTo(JsonPointer.Root()));
            Assert.That(diffs[0].Self, Is.SameAs(n1));
            Assert.That(diffs[0].Other, Is.SameAs(n2));
        }

        [Test]
        public void Diff_Object_Object()
        {
            var o1 = new JsonObject
            {
                ["_i1"] = 1,
                ["_i2"] = 2,
                ["_i3"] = 3,
            };

            var o2 = new JsonObject
            {
                ["_i1"] = 1,
                ["_i2"] = 3,
                ["_i4"] = 4,
            };

            var diffs = o1.Diff(o2);

            Assert.That(diffs.Length, Is.EqualTo(3));

            Assert.That(diffs[0].Pointer, Is.EqualTo(JsonPointer.Parse("/_i2")));
            Assert.That(diffs[0].Self, Is.EqualTo(JsonNumber.Create(2)));
            Assert.That(diffs[0].Other, Is.EqualTo(JsonNumber.Create(3)));

            Assert.That(diffs[1].Pointer, Is.EqualTo(JsonPointer.Parse("/_i3")));
            Assert.That(diffs[1].Self, Is.EqualTo(JsonNumber.Create(3)));
            Assert.That(diffs[1].Other, Is.Null);

            Assert.That(diffs[2].Pointer, Is.EqualTo(JsonPointer.Parse("/_i4")));
            Assert.That(diffs[2].Self, Is.Null);
            Assert.That(diffs[2].Other, Is.EqualTo(JsonNumber.Create(4)));
        }

        [Test]
        public void Diff_Object_None()
        {
            var n1 = new JsonObject();

            var diffs = n1.Diff(null);

            Assert.That(diffs.Length, Is.EqualTo(1));

            Assert.That(diffs[0].Pointer, Is.EqualTo(JsonPointer.Root()));
            Assert.That(diffs[0].Self, Is.SameAs(n1));
            Assert.That(diffs[0].Other, Is.Null);
        }

        [Test]
        public void Diff_Object_Array()
        {
            var n1 = new JsonObject();
            var n2 = new JsonArray();

            var diffs = n1.Diff(n2);

            Assert.That(diffs.Length, Is.EqualTo(1));

            Assert.That(diffs[0].Pointer, Is.EqualTo(JsonPointer.Root()));
            Assert.That(diffs[0].Self, Is.SameAs(n1));
            Assert.That(diffs[0].Other, Is.SameAs(n2));
        }
        
        
    }
}