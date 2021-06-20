using System;

using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    public sealed class JsonNodeWriterTests
    {
        [Test]
        public void WriteNull()
        {
            using var jw = new JsonNodeWriter();

            jw.WriteNull();

            Assert.That(jw.GetNode(), Is.EqualTo(JsonNull.Create()));
        }
        
        [Test]
        public void WriteNull_StateAtStartObject_ThrowsInvalidOperationException()
        {
            using var jw = new JsonNodeWriter();
            
            jw.WriteStartObject();
            
            Assert.Throws<InvalidOperationException>(
                () => jw.WriteNull());
        }
        
        [TestCase(true)]
        [TestCase(false)]
        public void WriteBool(bool value)
        {
            using var jw = new JsonNodeWriter();

            jw.WriteBool(value);

            Assert.That(jw.GetNode(), Is.EqualTo(JsonBool.Create(value)));
        }
        
        [Test]
        public void WriteBool_StateAtStartObject_ThrowsInvalidOperationException()
        {
            using var jw = new JsonNodeWriter();
            
            jw.WriteStartObject();
            
            Assert.Throws<InvalidOperationException>(
                () => jw.WriteBool(true));
        }
        
        [TestCase("foo")]
        [TestCase("")]
        [TestCase("\r")]
        [TestCase("\n")]
        [TestCase("\f")]
        [TestCase("\t")]
        [TestCase("\b")]
        [TestCase("\\")]
        [TestCase("\"")]
        [TestCase("/")]
        [TestCase("\u0401")]
        [TestCase("\u001A")]
        public void WriteString(string value)
        {
            using var jw = new JsonNodeWriter();

            jw.WriteString(value);

            Assert.That(jw.GetNode(), Is.EqualTo(JsonString.Create(value)));
        }
        
        [Test]
        public void WriteString_StateAtStartObject_ThrowsInvalidOperationException()
        {
            using var jw = new JsonNodeWriter();
            
            jw.WriteStartObject();
            
            Assert.Throws<InvalidOperationException>(
                () => jw.WriteString("foo"));
        }
        
        [TestCase(1, null)]
        [TestCase(2, ".2")]
        public void WriteNumber(int value, string format)
        {
            using var jw = new JsonNodeWriter();

            jw.WriteNumber(value, format);

            var node = (JsonNumber) jw.GetNode();

            var expectedFormat = format is not null
                ? JsonNumberFormat.Parse(format)
                : JsonNumberFormat.For(value);
            
            Assert.That(node, Is.EqualTo(JsonNumber.Create(value, format)));
            Assert.That(node.Format, Is.EqualTo(expectedFormat));
        }
        
        [Test]
        public void WriteNumber_StateAtStartObject_ThrowsInvalidOperationException()
        {
            using var jw = new JsonNodeWriter();
            
            jw.WriteStartObject();
            
            Assert.Throws<InvalidOperationException>(
                () => jw.WriteNumber(1));
        }
        
        [Test]
        public void WriteObject_Empty()
        {
            using var jw = new JsonNodeWriter();

            jw.WriteStartObject();
            jw.WriteEndObject();

            Assert.That(jw.GetNode().DeepEquals(new JsonObject()));
        }
        
        [Test]
        public void WriteObject_FlatContent()
        {
            using var jw = new JsonNodeWriter();

            jw.WriteStartObject();
            jw.WriteProperty("1");
            jw.WriteNumber(1);
            jw.WriteProperty("2");
            jw.WriteNumber(2);
            jw.WriteProperty("3");
            jw.WriteNumber(3);
            jw.WriteEndObject();
            
            var expected = new JsonObject {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };

            Assert.That(jw.GetNode().DeepEquals(expected));
        }
        
        [Test]
        public void WriteObject_NestedContent()
        {
            using var jw = new JsonNodeWriter();

            jw.WriteStartObject();
            jw.WriteProperty("1");
            jw.WriteNumber(1);
            jw.WriteProperty("2");
            jw.WriteStartArray();
            jw.WriteEndArray();
            jw.WriteProperty("3");
            jw.WriteStartObject();
            jw.WriteEndObject();
            jw.WriteEndObject();
            
            var expected = new JsonObject {
                ["1"] = 1,
                ["2"] = new JsonArray(),
                ["3"] = new JsonObject(),
            };

            Assert.That(jw.GetNode().DeepEquals(expected));
        }

        [Test]
        public void WriteStartObject_StateAtStartObject_ThrowsInvalidOperationException()
        {
            using var jw = new JsonNodeWriter();
            
            jw.WriteStartObject();
            
            Assert.Throws<InvalidOperationException>(
                () => jw.WriteStartObject());
        }
        
        [Test]
        public void WriteArray_Empty()
        {
            using var jw = new JsonNodeWriter();

            jw.WriteStartArray();
            jw.WriteEndArray();

            Assert.That(jw.GetNode().DeepEquals(new JsonArray()));
        }
        
        [Test]
        public void WriteArray_FlatContent()
        {
            using var jw = new JsonNodeWriter();

            jw.WriteStartArray();
            jw.WriteNumber(1);
            jw.WriteNumber(2);
            jw.WriteNumber(3);
            jw.WriteEndArray();

            var expected = new JsonArray { 1, 2, 3 };

            Assert.That(jw.GetNode().DeepEquals(expected));
        }
        
        [Test]
        public void WriteArray_NestedContent()
        {
            using var jw = new JsonNodeWriter();

            jw.WriteStartArray();
            jw.WriteNumber(1);
            jw.WriteStartArray();
            jw.WriteEndArray();
            jw.WriteStartObject();
            jw.WriteEndObject();

            var expected = new JsonArray { 
                1, 
                new JsonArray(),
                new JsonObject()
            };

            Assert.That(jw.GetNode().DeepEquals(expected));
        }

        [Test]
        public void WriteStartArray_StateAtStartObject_ThrowsInvalidOperationException()
        {
            using var jw = new JsonNodeWriter();
            
            jw.WriteStartObject();
            
            Assert.Throws<InvalidOperationException>(
                () => jw.WriteStartArray());
        }
        
        [Test]
        public void WriteProperty()
        {
            using var jw = new JsonNodeWriter();

            jw.WriteStartObject();
            jw.WriteProperty("foo");
            jw.WriteString("bar");
            jw.WriteEndObject();

            var expected = new JsonObject {
                ["foo"] = "bar"
            };
            
            Assert.That(jw.GetNode().DeepEquals(expected));
        }

        [Test]
        public void WriteEndArray_StateAtRoot_ThrowsInvalidOperationException()
        {
            using var jw = new JsonNodeWriter();

            Assert.Throws<InvalidOperationException>(() => jw.WriteEndArray());
        }
        
        [Test]
        public void WriteEndArray_StateAtStartObject_ThrowsInvalidOperationException()
        {
            using var jw = new JsonNodeWriter();
            
            jw.WriteStartObject();

            Assert.Throws<InvalidOperationException>(() => jw.WriteEndArray());
        }

        [Test]
        public void WriteEndObject_StateAtRoot_ThrowsInvalidOperationException()
        {
            using var jw = new JsonNodeWriter();

            Assert.Throws<InvalidOperationException>(() => jw.WriteEndObject());
        }
        
        [Test]
        public void WriteEndObject_StateAtStartArray_ThrowsInvalidOperationException()
        {
            using var jw = new JsonNodeWriter();
            
            jw.WriteStartArray();

            Assert.Throws<InvalidOperationException>(() => jw.WriteEndObject());
        }
        
        [Test]
        public void WriteEndObject_StateAtProperty_ThrowsInvalidOperationException()
        {
            using var jw = new JsonNodeWriter();
            
            jw.WriteStartObject();
            jw.WriteProperty("foo");

            Assert.Throws<InvalidOperationException>(() => jw.WriteEndObject());
        }
        
        [Test]
        public void WriteProperty_StateAtRoot_ThrowsInvalidOperationException()
        {
            using var jw = new JsonNodeWriter();

            Assert.Throws<InvalidOperationException>(() => jw.WriteProperty("foo"));
        }
        
        [Test]
        public void WriteProperty_StateAtStartArray_ThrowsInvalidOperationException()
        {
            using var jw = new JsonNodeWriter();
            
            jw.WriteStartArray();

            Assert.Throws<InvalidOperationException>(() => jw.WriteProperty("foo"));
        }
        
        [Test]
        public void WriteProperty_StateAtProperty_ThrowsInvalidOperationException()
        {
            using var jw = new JsonNodeWriter();
            
            jw.WriteStartObject();
            jw.WriteProperty("foo");

            Assert.Throws<InvalidOperationException>(() => jw.WriteProperty("foo"));
        }
        
        [Test]
        public void Ctor_JsonContainerIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new JsonNodeWriter(null);
            });
        }

        [Test]
        public void GetNode_NoNodes_ThrowsInvalidOperationException()
        {
            using var jw = new JsonNodeWriter();

            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = jw.GetNode();
            });
        }
        
        [Test]
        public void GetNode_MultipleNodes_ThrowsInvalidOperationException()
        {
            using var jw = new JsonNodeWriter();

            jw.WriteNull();
            jw.WriteNull();
            
            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = jw.GetNode();
            });
        }
        
        [Test]
        public void GetNode_CreatedUsingContainer_ReturnsSameNode()
        {
            var container = new JsonArray();

            using var jw = new JsonNodeWriter(container);
            
            jw.WriteNumber(1);
            jw.WriteNumber(2);
            jw.WriteNumber(3);
            
            Assert.That(jw.GetNode(), Is.SameAs(container));
            Assert.That(jw.GetNode(), Is.EqualTo(new JsonArray{ 1, 2, 3}));
        }

        [Test]
        public void GetNode_ClosesOpenContainers()
        {
            using var jw = new JsonNodeWriter();
            
            jw.WriteStartObject();
            jw.WriteProperty("foo");
            jw.WriteStartArray();
            jw.WriteNumber(1);

            var expected = new JsonObject {
                ["foo"] = new JsonArray { 1 }
            };

            Assert.That(jw.GetNode().DeepEquals(expected));
        }
        
        [Test]
        public void GetNodes()
        {
            using var jw = new JsonNodeWriter();

            jw.WriteNumber(1);

            jw.WriteStartArray();
            jw.WriteNumber(1);
            jw.WriteNumber(2);
            jw.WriteEndArray();
            
            jw.WriteStartObject();
            jw.WriteProperty("1");
            jw.WriteNumber(1);
            jw.WriteProperty("2");
            jw.WriteNumber(2);
            jw.WriteEndObject();

            var nodes = jw.GetNodes();
            
            Assert.That(nodes.Length, Is.EqualTo(3));
            
            Assert.That(nodes[0].DeepEquals(JsonNumber.Create(1)));
            Assert.That(nodes[1].DeepEquals(new JsonArray { 1,2 }));
            Assert.That(nodes[2].DeepEquals(new JsonObject {
                ["1"] = 1,
                ["2"] = 2
            }));
        }
    }

}