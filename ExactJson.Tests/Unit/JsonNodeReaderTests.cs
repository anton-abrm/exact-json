using System;

using NUnit.Framework;

namespace ExactJson.Tests.Unit
{
    public sealed class JsonNodeReaderTests
    {
        [Test]
        public void Ctor_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new JsonNodeReader(null);
            });
        }

        [Test]
        public void Ctor_ArrayWithNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var _ = new JsonNodeReader(1, null);
            });
        }

        [Test]
        public void Read_MultipleAtRoot_Flat()
        {
            var jr = new JsonNodeReader(1, 2, 3);

            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            Assert.That(jr.Read());
            Assert.That((int) jr.ReadNumber(), Is.EqualTo(1));
            Assert.That((int) jr.ReadNumber(), Is.EqualTo(2));
            Assert.That((int) jr.ReadNumber(), Is.EqualTo(3));
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            Assert.That(jr.Read(), Is.False);
        }

        [Test]
        public void Read_Number()
        {
            var jr = new JsonNodeReader(JsonNumber.Create(1, ".2"));

            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.Number));
            Assert.That(jr.ValueAsNumber, Is.EqualTo((JsonDecimal) 1));
            Assert.That(jr.NumberFormat, Is.EqualTo(JsonNumberFormat.Parse(".2")));
            Assert.That(jr.Read(), Is.False);
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
        }

        [Test]
        public void Read_Null()
        {
            var jr = new JsonNodeReader(JsonNull.Create());

            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.Null));
            Assert.That(jr.Read(), Is.False);
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));

        }

        [TestCase(true)]
        [TestCase(false)]
        public void Read_Bool(bool value)
        {
            var jr = new JsonNodeReader(JsonBool.Create(value));

            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.Bool));
            Assert.That(jr.ValueAsBool, Is.EqualTo(value));
            Assert.That(jr.Read(), Is.False);
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
        }

        [Test]
        public void Read_String()
        {
            var jr = new JsonNodeReader(JsonString.Create("bar"));

            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.String));
            Assert.That(jr.ValueAsString, Is.EqualTo("bar"));
            Assert.That(jr.Read(), Is.False);
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
        }

        [Test]
        public void Read_Object_Empty()
        {
            var jr = new JsonNodeReader(new JsonObject());

            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.StartObject));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.EndObject));
            Assert.That(jr.Read(), Is.False);
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
        }

        [Test]
        public void Read_Object()
        {
            var jr = new JsonNodeReader(new JsonObject {
                ["foo1"] = "bar1",
                ["foo2"] = "bar2"
            });

            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.StartObject));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.Property));
            Assert.That(jr.ValueAsString, Is.EqualTo("foo1"));
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
        public void Read_Array()
        {
            var jr = new JsonNodeReader(new JsonArray { 1, 2, 3 });

            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.StartArray));
            Assert.That(jr.Read());
            Assert.That((int) jr.ValueAsNumber, Is.EqualTo(1));
            Assert.That(jr.Read());
            Assert.That((int) jr.ValueAsNumber, Is.EqualTo(2));
            Assert.That(jr.Read());
            Assert.That((int) jr.ValueAsNumber, Is.EqualTo(3));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.EndArray));
            Assert.That(jr.Read(), Is.False);
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
        }

        [Test]
        public void ValueAsBool_Null_ThrowsInvalidOperationException()
        {
            var jr = new JsonNodeReader(JsonNull.Create());

            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.Null));
            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = jr.ValueAsBool;
            });
        }

        [Test]
        public void ValueAsString_Null_ThrowsInvalidOperationException()
        {
            var jr = new JsonNodeReader(JsonNull.Create());

            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.Null));
            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = jr.ValueAsString;
            });
        }

        [Test]
        public void ValueAsNumber_Null_ThrowsInvalidOperationException()
        {
            var jr = new JsonNodeReader(JsonNull.Create());

            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.Null));
            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = jr.ValueAsNumber;
            });
        }

        [Test]
        public void NumberFormat_Null_ThrowsInvalidOperationException()
        {
            var jr = new JsonNodeReader(JsonNull.Create());

            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.Null));
            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = jr.NumberFormat;
            });
        }

        [Test]
        public void CanSaveState_True()
        {
            var jr = new JsonNodeReader();

            Assert.That(jr.CanSaveState);
        }
        
        [Test]
        public void SaveAndRestoreState_Root()
        {
            var jr = new JsonNodeReader(new JsonArray { 1, 2, 3 });

            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.StartArray));
            Assert.That(jr.Read());
            Assert.That((int) jr.ValueAsNumber, Is.EqualTo(1));
            Assert.That(jr.Read());
            
            var state = jr.SaveState();
            
            Assert.That((int) jr.ValueAsNumber, Is.EqualTo(2));
            Assert.That(jr.Read());
            Assert.That((int) jr.ValueAsNumber, Is.EqualTo(3));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.EndArray));
            Assert.That(jr.Read(), Is.False);
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            
            state.Restore();
            
            Assert.That((int) jr.ValueAsNumber, Is.EqualTo(2));
            Assert.That(jr.Read());
            Assert.That((int) jr.ValueAsNumber, Is.EqualTo(3));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.EndArray));
            Assert.That(jr.Read(), Is.False);
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
        }
        
        [Test]
        public void SaveAndRestoreState_Object()
        {
            var jr = new JsonNodeReader(new JsonObject {
                ["foo1"] = "bar1",
                ["foo2"] = "bar2"
            });

            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.StartObject));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.Property));
            Assert.That(jr.ValueAsString, Is.EqualTo("foo1"));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.String));
            Assert.That(jr.ValueAsString, Is.EqualTo("bar1"));
            Assert.That(jr.Read());

            var state = jr.SaveState();
            
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.Property));
            Assert.That(jr.ValueAsString, Is.EqualTo("foo2"));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.String));
            Assert.That(jr.ValueAsString, Is.EqualTo("bar2"));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.EndObject));
            Assert.That(jr.Read(), Is.False);
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            
            state.Restore();
            
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
        public void MoveToProperty()
        {
            var jr = new JsonNodeReader(new JsonObject {
                ["foo1"] = "bar1",
                ["foo2"] = "bar2"
            });

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
            var jr = new JsonNodeReader(new JsonObject());

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
            var jr = new JsonNodeReader(new JsonObject {
                ["foo1"] = "bar1",
                ["foo2"] = "bar2"
            });

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
            var jr = new JsonNodeReader(new JsonObject {
                ["foo1"] = "bar1",
                ["foo2"] = "bar2"
            });

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
            var jr = new JsonNodeReader(new JsonObject {
                ["foo1"] = "bar1",
                ["foo2"] = "bar2"
            });
            
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));
            Assert.That(jr.Read());
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.StartObject));

            Assert.Throws<ArgumentNullException>(
                () => jr.MoveToProperty(null));
        }
        
        [Test]
        public void MoveToProperty_OnStart_ThrowsInvalidOperationException()
        {
            var jr = new JsonNodeReader(new JsonObject {
                ["foo1"] = "bar1",
                ["foo2"] = "bar2"
            });
            
            Assert.That(jr.TokenType, Is.EqualTo(JsonTokenType.None));

            Assert.Throws<InvalidOperationException>(
                () => jr.MoveToProperty("foo"));
        }
    }
}