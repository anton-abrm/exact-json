// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    public class JsonArrayTests
    {
        [Test]
        public void Ctor()
        {
            var ja = new JsonArray();

            Assert.That(ja.Count, Is.EqualTo(0));
            Assert.That(ja.NodeType, Is.EqualTo(JsonNodeType.Array));
            Assert.That(ja, Is.EquivalentTo(new JsonNode[] { }));
            Assert.That(((ICollection<JsonNode>) ja).IsReadOnly, Is.False);
        }

        [Test]
        public void Clone()
        {
            var ja = new JsonArray { 1 };

            var clone = ja.Clone();

            Assert.That(clone, Is.Not.SameAs(ja));
            Assert.That(clone, Is.EquivalentTo(ja));
        }

        [Test]
        public void Add()
        {
            var ja = new JsonArray { 1 };

            Assert.That(ja.Count, Is.EqualTo(1));
            Assert.That(ja, Is.EquivalentTo(new JsonNode[] { 1 }));
        }

        [Test]
        public void Add_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(()
                => new JsonArray().Add(null));
        }

        [Test]
        public void Indexer()
        {
            var ja = new JsonArray { 1 };

            Assert.That(ja[0], Is.EqualTo((JsonNumber) 1));

            ja[0] = 2;

            Assert.That(ja[0], Is.EqualTo((JsonNumber) 2));
            Assert.That(ja, Is.EquivalentTo(new JsonNode[] { 2 }));
        }

        [Test]
        public void Clear()
        {
            var ja = new JsonArray { 1 };

            ja.Clear();

            Assert.That(ja.Count, Is.EqualTo(0));
            Assert.That(ja, Is.EquivalentTo(new JsonNode[] { }));
        }

        [Test]
        public void Contains()
        {
            var ja = new JsonArray { 1 };

            Assert.That(ja.Contains(1), Is.True);
            Assert.That(ja.Contains(2), Is.False);
        }

        [Test]
        public void Contains_Null_ThrowsArgumentNullException()
        {
            var ja = new JsonArray { 1 };

            Assert.Throws<ArgumentNullException>(() =>
            {
                bool _ = ja.Contains(null);
            });
        }

        [Test]
        public void CopyTo()
        {
            // TODO: Rework

            var ja = new JsonArray { 1, 2, 3 };

            var copy = new JsonNode [ja.Count];

            ja.CopyTo(copy, 0);

            Assert.That(ja, Is.EquivalentTo(copy));
        }

        [Test]
        public void GetEnumerator()
        {
            var ja = new JsonArray { 1, 2, 3 };

            Assert.That(ja.GetEnumerator(), Is.Not.Null);
        }

        [Test]
        public void IndexOf()
        {
            var ja = new JsonArray { 1, 2, 3 };

            Assert.That(ja.IndexOf(2), Is.EqualTo(1));
        }

        [Test]
        public void Insert()
        {
            var ja = new JsonArray { 1, 2, 3 };

            ja.Insert(1, 4);

            Assert.That(ja.Count, Is.EqualTo(4));
            Assert.That(ja, Is.EquivalentTo(new JsonNode[] { 1, 4, 2, 3 }));
        }

        [Test]
        public void Insert_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new JsonArray().Insert(0, null);
            });
        }

        [Test]
        public void Remove()
        {
            var ja = new JsonArray { 1, 2, 3 };

            Assert.That(ja.Remove(2), Is.True);
            Assert.That(ja.Remove(4), Is.False);

            Assert.That(ja.Count, Is.EqualTo(2));
            Assert.That(ja, Is.EquivalentTo(new JsonNode[] { 1, 3 }));
        }

        [Test]
        public void Remove_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new JsonArray().Remove(null);
            });
        }

        [Test]
        public void RemoveAt()
        {
            var ja = new JsonArray { 1, 2, 3 };

            ja.RemoveAt(1);

            Assert.That(ja.Count, Is.EqualTo(2));
            Assert.That(ja, Is.EquivalentTo(new JsonNode[] { 1, 3 }));
        }

        [Test]
        public void WriteTo()
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            new JsonArray { 1, 2, 3 }.WriteTo(jw);

            Assert.That(sw.ToString(), Is.EqualTo("[1,2,3]"));
        }

        [Test]
        public void WriteTo_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(()
                => new JsonArray().WriteTo((JsonWriter) null));
        }

        [Test]
        public void DeepEquals()
        {
            var a = new JsonArray { 1, 2, 3 };
            var b = new JsonArray { 1, 2, 3 };
            var c = new JsonArray { 1, 2 };
            var d = new JsonArray { 1, 2, 4 };

            Assert.That(a.DeepEquals(b), Is.True);
            Assert.That(a.DeepEquals(c), Is.False);
            Assert.That(a.DeepEquals(d), Is.False);
            Assert.That(a.DeepEquals(null), Is.False);
        }
    }
}