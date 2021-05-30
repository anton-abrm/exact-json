using System;
using System.IO;
using System.Text;

using Moq;
using Moq.Protected;

using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    public class JsonReaderTests
    {
        #region Dispose

        [Test]
        public void Dispose_ByDefault_DisposeWithDisposingAsTrueCalled()
        {
            var mock = new Mock<JsonReader>();

            mock.Object.Dispose();

            mock.Protected().Verify("Dispose", Times.Once(), true, true);
            mock.VerifyNoOtherCalls();
        }

        #endregion

        #region Value
        
        [TestCase(JsonTokenType.StartArray)]
        [TestCase(JsonTokenType.EndArray)]
        [TestCase(JsonTokenType.StartObject)]
        [TestCase(JsonTokenType.EndObject)]
        public void Value_OnContainerState_ReturnsNull(JsonTokenType tokenType)
        {
            var mock = new Mock<JsonReader>();

            mock.Setup(r => r.TokenType).Returns(tokenType);

            Assert.That(mock.Object.Value, Is.Null);

            mock.Verify(r => r.TokenType, Times.Once);

            mock.VerifyNoOtherCalls();
        }
        
        [TestCase(JsonTokenType.String)]
        [TestCase(JsonTokenType.Property)]
        public void Value_OnStringOrPropertyState_CallsValueAsString(JsonTokenType tokenType)
        {
            var mock = new Mock<JsonReader>();

            mock.Setup(r => r.TokenType).Returns(tokenType);
            mock.Setup(r => r.ValueAsString).Returns("foo");

            Assert.That(mock.Object.Value, Is.EqualTo("foo"));

            mock.Verify(r => r.TokenType, Times.Once);
            mock.Verify(r => r.ValueAsString, Times.Once);

            mock.VerifyNoOtherCalls();
        }

        [Test]
        public void Value_OnBoolState_CallsValueAsBool()
        {
            var mock = new Mock<JsonReader>();

            mock.Setup(r => r.TokenType).Returns(JsonTokenType.Bool);
            mock.Setup(r => r.ValueAsBool).Returns(true);

            Assert.That(mock.Object.Value, Is.True);

            mock.Verify(r => r.TokenType, Times.Once);
            mock.Verify(r => r.ValueAsBool, Times.Once);

            mock.VerifyNoOtherCalls();
        }

        [Test]
        public void Value_OnNoneState_ReturnsNull()
        {
            var mock = new Mock<JsonReader>();

            mock.Setup(r => r.TokenType).Returns(JsonTokenType.None);

            Assert.That(mock.Object.Value, Is.Null);

            mock.Verify(r => r.TokenType, Times.Once);

            mock.VerifyNoOtherCalls();
        }

        [Test]
        public void Value_OnNullState_ReturnsNull()
        {
            var mock = new Mock<JsonReader>();

            mock.Setup(r => r.TokenType).Returns(JsonTokenType.Null);

            Assert.That(mock.Object.Value, Is.Null);

            mock.Verify(r => r.TokenType, Times.Once);

            mock.VerifyNoOtherCalls();
        }

        [Test]
        public void Value_OnDecimalState_CallsValueAsDecimal()
        {
            var mock = new Mock<JsonReader>();

            mock.Setup(r => r.TokenType).Returns(JsonTokenType.Number);
            mock.Setup(r => r.ValueAsNumber).Returns(1.0M);

            Assert.That(mock.Object.Value, Is.EqualTo((JsonDecimal) 1.0M));

            mock.Verify(r => r.TokenType, Times.Once);
            mock.Verify(r => r.ValueAsNumber, Times.Once);

            mock.VerifyNoOtherCalls();
        }

        #endregion

        #region CopyTo

        [Test]
        public void CopyTo_Null_ThrowsArgumentNullException()
        {
            var jr = new JsonStringReader(
                "{" +
                "\"foo1\":\"bar1\", " +
                "\"foo2\":\"bar2\"" +
                "}");
            
            Assert.Throws<ArgumentNullException>(
                () => jr.CopyTo(null));
        }
        
        [Test]
        public void CopyTo_InvalidState()
        {
            const string json = "{\n" +
                                "  \"i1\": 1,\n" +
                                "  \"n1\": null,\n" +
                                "  \"t1\": true,\n" +
                                "  \"f1\": false,\n" +
                                "  \"s1\": \"foo\",\n" +
                                "  \"a1\": [],\n" +
                                "  \"o1\": {}\n" +
                                "}";

            var sb = new StringBuilder();
            
            using var jr = new JsonStringReader(json);
            using var sw = new StringWriter(sb);
            using var jw = new JsonTextWriter(sw)
            {
                Formatted = true,
            };

            jr.MoveToToken();
            jr.ReadStartObject();
            
            Assert.That(jr.TokenType == JsonTokenType.Property);

            Assert.Throws<InvalidOperationException>(() => jr.CopyTo(jw));
        }
        
        [Test]
        public void CopyTo_Object()
        {
            const string json = "{\n" +
                                "  \"i1\": 1,\n" +
                                "  \"n1\": null,\n" +
                                "  \"t1\": true,\n" +
                                "  \"f1\": false,\n" +
                                "  \"s1\": \"foo\",\n" +
                                "  \"a1\": [],\n" +
                                "  \"o1\": {}\n" +
                                "}";

            var sb = new StringBuilder();

            {
                using var jr = new JsonStringReader(json);
                using var sw = new StringWriter(sb);
                using var jw = new JsonTextWriter(sw)
                {
                    Formatted = true,
                };

                jr.CopyTo(jw);
            }

            Assert.That(sb.ToString(), Is.EqualTo(json));
        }

        [Test]
        public void CopyTo_Array()
        {
            const string json = "[\n" +
                                "  1,\n" +
                                "  null,\n" +
                                "  true,\n" +
                                "  false,\n" +
                                "  \"foo\",\n" +
                                "  [],\n" +
                                "  {}\n" +
                                "]";

            var sb = new StringBuilder();

            {
                using var jr = new JsonStringReader(json);
                using var sw = new StringWriter(sb);
                using var jw = new JsonTextWriter(sw)
                {
                    Formatted = true,
                };

                jr.CopyTo(jw);
            }

            Assert.That(sb.ToString(), Is.EqualTo(json));
        }

        [Test]
        public void CopyTo_WriterNull_ThrowsArgumentNullException()
        {
            using var jr = new JsonStringReader("");

            Assert.Throws<ArgumentNullException>(() => jr.CopyTo(null));
        }

        [Test]
        public void CopyTo_MultipleAtRoot_CopyAll()
        {
            const string json = "1\n2\n3";

            var sb = new StringBuilder();

            {
                using var jr = new JsonStringReader(json);
                using var sw = new StringWriter(sb);
                using var jw = new JsonTextWriter(sw);
                
                jr.CopyTo(jw);
            }

            Assert.That(sb.ToString(), Is.EqualTo(json));
        }
        
        [Test]
        public void CopyTo_MultipleAtRoot_CopyOne()
        {
            const string json = "1\n2\n3";

            var sb = new StringBuilder();

            {
                using var jr = new JsonStringReader(json);
                using var sw = new StringWriter(sb);
                using var jw = new JsonTextWriter(sw);

                jr.Read();
                jr.CopyTo(jw);
            }

            Assert.That(sb.ToString(), Is.EqualTo("1"));
        }

        #endregion
        
        #region MoveToProperty
        
        [Test]
        public void MoveToProperty()
        {
            var jr = new JsonStringReader(
                "{" +
                    "\"foo1\":\"bar1\", " +
                    "\"foo2\":\"bar2\"" +
                "}");

            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.StartObject));

            Assert.That(jr.MoveToProperty("foo2"));

            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.Property));
            Assert.That(jr.ValueAsString, Is.EqualTo("foo2"));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.String));
            Assert.That(jr.ValueAsString, Is.EqualTo("bar2"));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.EndObject));
            Assert.That(jr.Read(), Is.False);
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
        }
        
        [Test]
        public void MoveToProperty_EmptyObject()
        {
            var jr = new JsonStringReader("{}");

            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.StartObject));

            Assert.That(jr.MoveToProperty("foo2"), Is.False);
            
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.EndObject));
            Assert.That(jr.Read(), Is.False);
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
        }
        
        [Test]
        public void MoveToProperty_AlreadyOnSameProperty()
        {
            var jr = new JsonStringReader(
                "{" +
                "\"foo1\":\"bar1\", " +
                "\"foo2\":\"bar2\"" +
                "}");

            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.StartObject));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.Property));
            Assert.That(jr.ValueAsString, Is.EqualTo("foo1"));
            
            
            Assert.That(jr.MoveToProperty("foo1"));
            
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.String));
            Assert.That(jr.ValueAsString, Is.EqualTo("bar1"));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.Property));
            Assert.That(jr.ValueAsString, Is.EqualTo("foo2"));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.String));
            Assert.That(jr.ValueAsString, Is.EqualTo("bar2"));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.EndObject));
            Assert.That(jr.Read(), Is.False);
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
        }
        
        [Test]
        public void MoveToProperty_OnSiblingProperty()
        {
            var jr = new JsonStringReader(
                "{" +
                "\"foo1\":\"bar1\", " +
                "\"foo2\":\"bar2\"" +
                "}");

            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.StartObject));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.Property));
            Assert.That(jr.ValueAsString, Is.EqualTo("foo1"));
            
            Assert.That(jr.MoveToProperty("foo2"));

            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.String));
            Assert.That(jr.ValueAsString, Is.EqualTo("bar2"));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.EndObject));
            Assert.That(jr.Read(), Is.False);
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
        }
        
        [Test]
        public void MoveToProperty_Null_ThrowsArgumentNullException()
        {
            var jr = new JsonStringReader(
                "{" +
                "\"foo1\":\"bar1\", " +
                "\"foo2\":\"bar2\"" +
                "}");
            
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.StartObject));

            Assert.Throws<ArgumentNullException>(
                () => jr.MoveToProperty(null));
        }
        
        [Test]
        public void MoveToProperty_OnStart_ThrowsInvalidOperationException()
        {
            var jr = new JsonStringReader(
                "{" +
                "\"foo1\":\"bar1\", " +
                "\"foo2\":\"bar2\"" +
                "}");
            
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));

            Assert.Throws<InvalidOperationException>(
                () => jr.MoveToProperty("foo"));
        }
        
        #endregion

        #region Skip

        [Test]
        public void Skip_Empty()
        {
            var jr = new JsonStringReader("");
            
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            
            jr.Skip();
            
            Assert.That(jr.Read(), Is.False);
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
        }
        
        [Test]
        public void Skip_All()
        {
            var jr = new JsonStringReader("1\n{}\n[]\n");
            
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            
            jr.Skip();
            
            Assert.That(jr.Read(), Is.False);
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
        }
        
        [Test]
        public void Skip_One()
        {
            var jr = new JsonStringReader("[]{}");
            
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.StartArray));
            
            jr.Skip();
            
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.StartObject));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.EndObject));
            Assert.That(jr.Read(), Is.False);
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
        }
        
        [Test]
        public void Skip_InvalidState()
        {
            var jr = new JsonStringReader("[{}{}{}]");
            
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.StartArray));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.StartObject));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.EndObject));

            Assert.Throws<InvalidOperationException>(() => jr.Skip());
        }

        #endregion
        
        [Test]
        public void ReadX_UnableToReadX_ThrowsInvalidOperationException()
        {
            var reader = new JsonStringReader("");

            Assert.Throws<InvalidOperationException>(() => reader.ReadToken());
            Assert.Throws<InvalidOperationException>(() => reader.ReadProperty());
            Assert.Throws<InvalidOperationException>(() => reader.ReadString());
            Assert.Throws<InvalidOperationException>(() => reader.ReadNull());
            Assert.Throws<InvalidOperationException>(() => reader.ReadBool());
            Assert.Throws<InvalidOperationException>(() => reader.ReadNumber());
            Assert.Throws<InvalidOperationException>(() => reader.ReadStartArray());
            Assert.Throws<InvalidOperationException>(() => reader.ReadEndArray());
            Assert.Throws<InvalidOperationException>(() => reader.ReadStartObject());
            Assert.Throws<InvalidOperationException>(() => reader.ReadEndObject());
        }
    }
}