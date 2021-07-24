// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;

using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    public class JsonTextWriterTests
    {
        private sealed class MockTextWriterForDisposing : TextWriter
        {
            public bool IsClosed { get; set; }

            protected override void Dispose(bool disposing)
            {
                if (disposing) {
                    IsClosed = true;
                }
            }

            public override Encoding Encoding { get; } = Encoding.UTF8;
        }

        [Test]
        public void Ctor()
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            Assert.That(jw.Formatted, Is.False);
            Assert.That(jw.EscapeSolidus, Is.False);
            Assert.That(jw.EscapeNonAsciiChars, Is.False);
            Assert.That(jw.WriteHexInLowerCase, Is.False);

            Assert.That(jw.IndentSize, Is.EqualTo(2));
            Assert.That(jw.NewLine, Is.EqualTo("\n"));
        }
        
        [Test]
        public void Ctor_WriterNull_TrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new JsonTextWriter(null);
            });
        }

        [Test]
        public void WriteNull()
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            jw.WriteNull();

            Assert.That(sw.ToString(), Is.EqualTo("null"));
        }

        [TestCase(true, "true")]
        [TestCase(false, "false")]
        public void WriteBool(bool value, string expectedJson)
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            jw.WriteBool(value);

            Assert.That(sw.ToString(), Is.EqualTo(expectedJson));
        }

        [TestCase("foo", "\"foo\"")]
        [TestCase("", "\"\"")]
        [TestCase("\r", "\"\\r\"")]
        [TestCase("\n", "\"\\n\"")]
        [TestCase("\f", "\"\\f\"")]
        [TestCase("\t", "\"\\t\"")]
        [TestCase("\b", "\"\\b\"")]
        [TestCase("\\", "\"\\\\\"")]
        [TestCase("\"", "\"\\\"\"")]
        [TestCase("/", "\"/\"")]
        [TestCase("\u0401", "\"\u0401\"")]
        [TestCase("\u001A", "\"\\u001A\"")]
        public void WriteString_DefaultSettings(string value, string expectedJson)
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            jw.WriteString(value);

            Assert.That(sw.ToString(), Is.EqualTo(expectedJson));
        }

        [TestCase("/", "\"\\/\"")]
        public void WriteString_EscapeSolidus(string value, string expectedJson)
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw)
            {
                EscapeSolidus = true,
            };

            jw.WriteString(value);

            Assert.That(sw.ToString(), Is.EqualTo(expectedJson));
        }

        [TestCase("\u0401", "\"\\u0401\"")]
        public void WriteString_EscapeNonAsciiChars(string value, string expectedJson)
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw)
            {
                EscapeNonAsciiChars = true,
            };

            jw.WriteString(value);

            Assert.That(sw.ToString(), Is.EqualTo(expectedJson));
        }

        [TestCase("\u001A", "\"\\u001a\"")]
        public void WriteString_WriteHexInLowerCase(string value, string expectedJson)
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw)
            {
                WriteHexInLowerCase = true,
            };

            jw.WriteString(value);

            Assert.That(sw.ToString(), Is.EqualTo(expectedJson));
        }

        [Test]
        public void WriteString_Null_ThrowArgumentNullException()
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            Assert.Throws<ArgumentNullException>(() => jw.WriteString(null));
        }

        [Test]
        public void WriteNumber()
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            jw.WriteNumber(0);

            Assert.That(sw.ToString(), Is.EqualTo("0"));
        }

        [Test]
        public void WriteProperty_Null_ThrowsArgumentNullException()
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            jw.WriteStartObject();

            Assert.Throws<ArgumentNullException>(() => jw.WriteProperty(null));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Dispose_CloseOutput(bool closeOutput)
        {
            using var tw = new MockTextWriterForDisposing();
            using var jw = new JsonTextWriter(tw, closeOutput);

            jw.Dispose();

            Assert.That(tw.IsClosed, Is.EqualTo(closeOutput));
        }

        [Test]
        public void IndentSize_OutOfRange_ThrowsValueOutOfRangeException()
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            Assert.Throws<ArgumentOutOfRangeException>(() => jw.IndentSize = -1);
        }

        [Test]
        public void IndentSize()
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw)
            {
                IndentSize = 4,
                Formatted = true,
            };

            jw.WriteStartArray();
            jw.WriteBool(true);
            jw.WriteEndArray();

            Assert.That(sw.ToString(), Is.EqualTo("[\n    true\n]"));
        }

        [Test]
        public void WriteCarriageReturn()
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw)
            {
                NewLine = "\r\n",
                Formatted = true,
            };

            jw.WriteStartArray();
            jw.WriteBool(true);
            jw.WriteEndArray();

            Assert.That(sw.ToString(), Is.EqualTo("[\r\n  true\r\n]"));
        }

        [Test]
        public void WriteObject_Empty()
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            jw.WriteStartObject();
            jw.WriteEndObject();

            Assert.That(sw.ToString(), Is.EqualTo("{}"));
        }

        [Test]
        public void WriteArray_Empty()
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            jw.WriteStartArray();
            jw.WriteEndArray();

            Assert.That(sw.ToString(), Is.EqualTo("[]"));
        }

        [Test]
        public void WriteArray()
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            jw.WriteStartArray();

            jw.WriteNumber(1);
            jw.WriteNull();
            jw.WriteBool(true);
            jw.WriteBool(false);
            jw.WriteString("foo");
            jw.WriteStartArray();
            jw.WriteEndArray();
            jw.WriteStartObject();
            jw.WriteEndObject();

            jw.WriteEndArray();

            Assert.That(sw.ToString(), Is.EqualTo(
                "[1,null,true,false,\"foo\",[],{}]"));
        }

        [Test]
        public void WriteArray_Formatted()
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw)
            {
                Formatted = true,
            };

            jw.WriteStartArray();

            jw.WriteNumber(1);
            jw.WriteNull();
            jw.WriteBool(true);
            jw.WriteBool(false);
            jw.WriteString("foo");
            jw.WriteStartArray();
            jw.WriteEndArray();
            jw.WriteStartObject();
            jw.WriteEndObject();

            jw.WriteEndArray();

            Assert.That(sw.ToString(), Is.EqualTo(
                "[\n" +
                "  1,\n" +
                "  null,\n" +
                "  true,\n" +
                "  false,\n" +
                "  \"foo\",\n" +
                "  [],\n" +
                "  {}\n" +
                "]"));
        }

        [Test]
        public void WriteObject()
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            jw.WriteStartObject();

            jw.WriteProperty("i1");
            jw.WriteNumber(1);
            jw.WriteProperty("n1");
            jw.WriteNull();
            jw.WriteProperty("t1");
            jw.WriteBool(true);
            jw.WriteProperty("f1");
            jw.WriteBool(false);
            jw.WriteProperty("s1");
            jw.WriteString("foo");
            jw.WriteProperty("a1");
            jw.WriteStartArray();
            jw.WriteEndArray();
            jw.WriteProperty("o1");
            jw.WriteStartObject();
            jw.WriteEndObject();

            jw.WriteEndObject();

            Assert.That(sw.ToString(), Is.EqualTo(
                "{" +
                "\"i1\":1," +
                "\"n1\":null," +
                "\"t1\":true," +
                "\"f1\":false," +
                "\"s1\":\"foo\"," +
                "\"a1\":[]," +
                "\"o1\":{}" +
                "}"));
        }

        [Test]
        public void WriteObject_Formatted()
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw)
            {
                Formatted = true,
            };

            jw.WriteStartObject();

            jw.WriteProperty("i1");
            jw.WriteNumber(1);
            jw.WriteProperty("n1");
            jw.WriteNull();
            jw.WriteProperty("t1");
            jw.WriteBool(true);
            jw.WriteProperty("f1");
            jw.WriteBool(false);
            jw.WriteProperty("s1");
            jw.WriteString("foo");
            jw.WriteProperty("a1");
            jw.WriteStartArray();
            jw.WriteEndArray();
            jw.WriteProperty("o1");
            jw.WriteStartObject();
            jw.WriteEndObject();

            jw.WriteEndObject();

            Assert.That(sw.ToString(), Is.EqualTo(
                "{\n" +
                "  \"i1\": 1,\n" +
                "  \"n1\": null,\n" +
                "  \"t1\": true,\n" +
                "  \"f1\": false,\n" +
                "  \"s1\": \"foo\",\n" +
                "  \"a1\": [],\n" +
                "  \"o1\": {}\n" +
                "}"));
        }

        [Test]
        public void WriteNumber_Multiple()
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            jw.WriteNumber(1);
            jw.WriteNumber(2);

            Assert.That(sw.ToString(), Is.EqualTo("1\n2"));
        }

        [Test]
        public void WriteX_Root_Invalid_ThrowsInvalidOperationException()
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            Assert.Throws<InvalidOperationException>(() => jw.WriteProperty("foo"));
            Assert.Throws<InvalidOperationException>(() => jw.WriteEndArray());
            Assert.Throws<InvalidOperationException>(() => jw.WriteEndObject());
        }

        [Test]
        public void WriteX_Property_Invalid_ThrowsInvalidOperationException()
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            jw.WriteStartObject();
            jw.WriteProperty("foo");

            Assert.Throws<InvalidOperationException>(() => jw.WriteProperty("foo"));
            Assert.Throws<InvalidOperationException>(() => jw.WriteEndArray());
            Assert.Throws<InvalidOperationException>(() => jw.WriteEndObject());
        }

        [Test]
        public void WriteX_StartArray_Invalid_ThrowsInvalidOperationException()
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            jw.WriteStartArray();

            Assert.Throws<InvalidOperationException>(() => jw.WriteProperty("foo"));
        }

        [Test]
        public void WriteX_StartObject_Invalid_ThrowsInvalidOperationException()
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            jw.WriteStartObject();

            Assert.Throws<InvalidOperationException>(() => jw.WriteNumber(1));
            Assert.Throws<InvalidOperationException>(() => jw.WriteNull());
            Assert.Throws<InvalidOperationException>(() => jw.WriteBool(true));
            Assert.Throws<InvalidOperationException>(() => jw.WriteString("foo"));
            Assert.Throws<InvalidOperationException>(() => jw.WriteStartObject());
            Assert.Throws<InvalidOperationException>(() => jw.WriteStartArray());
            Assert.Throws<InvalidOperationException>(() => jw.WriteEndArray());
        }
        
        [Test]
        public void WriteX_AlreadyClosed_TrowsObjectDisposedException()
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);
            
            jw.Dispose();
            
            Assert.Throws<ObjectDisposedException>(() => jw.WriteProperty("foo"));
            Assert.Throws<ObjectDisposedException>(() => jw.WriteNumber(1));
            Assert.Throws<ObjectDisposedException>(() => jw.WriteNull());
            Assert.Throws<ObjectDisposedException>(() => jw.WriteBool(true));
            Assert.Throws<ObjectDisposedException>(() => jw.WriteString("bar"));
            Assert.Throws<ObjectDisposedException>(() => jw.WriteStartObject());
            Assert.Throws<ObjectDisposedException>(() => jw.WriteStartArray());
            Assert.Throws<ObjectDisposedException>(() => jw.WriteEndArray());
        }

        [Test]
        public void WriteRaw()
        {
            var sw = new StringWriter();
            var jw = new JsonTextWriter(sw);
            
            jw.WriteRaw("|");
            jw.WriteNumber(1);
            jw.WriteRaw("|");
            
            Assert.That(sw.ToString(), Is.EqualTo("|1|"));
        }

        [Test]
        public void NewLine_Default()
        {
            var sw = new StringWriter();
            var jw = new JsonTextWriter(sw);
            
            Assert.That(jw.NewLine, Is.EqualTo("\n"));
        }

        [Test] 
        public void NewLine_Set_Null_ThrowsArgumentNullException()
        {
            var sw = new StringWriter();
            var jw = new JsonTextWriter(sw);
            
            Assert.Throws<ArgumentNullException>(
                ()=> jw.NewLine = null);
        }
        
        [TestCase("\n")]
        [TestCase("\r\n")]
        public void NewLine_Set_Valid(string value)
        {
            var sw = new StringWriter();
            var jw = new JsonTextWriter(sw) {
                NewLine = value
            };
            
            Assert.That(jw.NewLine, Is.EqualTo(value));
        }
        
        [TestCase("")]
        public void NewLine_Set_Invalid_ThrowsArgumentOutOfRangeException(string value)
        {
            var sw = new StringWriter();
            var jw = new JsonTextWriter(sw);
            
            Assert.Throws<ArgumentOutOfRangeException>(
                ()=> jw.NewLine = value);
        }
        
        [Test]
        public void JsonSeparator_Default()
        {
            var sw = new StringWriter();
            var jw = new JsonTextWriter(sw);
            
            Assert.That(jw.JsonSeparator, Is.EqualTo("\n"));
        }
        
        [TestCase(" ")]
        [TestCase("\n")]
        [TestCase("\r\n")]
        public void JsonSeparator_Set_Valid(string value)
        {
            var sw = new StringWriter();
            var jw = new JsonTextWriter(sw) {
                JsonSeparator = value
            };
            
            Assert.That(jw.JsonSeparator, Is.EqualTo(value));
        }
        
        [TestCase("")]
        [TestCase("x")]
        public void JsonSeparator_Set_Invalid_ThrowsArgumentOutOfRangeException(string value)
        {
            var sw = new StringWriter();
            var jw = new JsonTextWriter(sw);
            
            Assert.Throws<ArgumentOutOfRangeException>(
                ()=> jw.JsonSeparator = value);
        }
        
        [Test] 
        public void JsonSeparator_Set_Null_ThrowsArgumentNullException()
        {
            var sw = new StringWriter();
            var jw = new JsonTextWriter(sw);
            
            Assert.Throws<ArgumentNullException>(
                ()=> jw.JsonSeparator = null);
        }
        
        [Test] 
        public void JsonSeparator_Custom()
        {
            var sw = new StringWriter();
            var jw = new JsonTextWriter(sw);

            jw.JsonSeparator = "\r\n";
            
            jw.WriteNumber(1);
            jw.WriteNumber(2);
            
            Assert.That(sw.ToString(), Is.EqualTo("1\r\n2"));
        }
    }
}