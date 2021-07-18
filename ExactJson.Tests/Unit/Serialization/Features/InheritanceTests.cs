using System.IO;

using ExactJson.Serialization;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization.Features
{
    public sealed class InheritanceTests
    {
        private enum ShapeColor
        {
            [JsonEnumValue("BLACK")] Black,
            [JsonEnumValue("WHITE")] White
        }
         
        private abstract class Shape
        {
            [JsonNode("color")]
            public ShapeColor Color { get; set; }
        }
        
        private sealed class Square : Shape
        {
            [JsonNode("width")]
            public double Width { get; set; }

            [JsonNode("height")]
            public double Height { get; set; }
        }
        
        private sealed class Circle : Shape
        {
            [JsonNode("radius")]
            public double Radius { get; set; }
        }

        private static JsonSerializer CreateSerializer()
        {
            var serializer = new JsonSerializer();
            
            serializer.RegisterType<Circle>("CIRCLE");
            serializer.RegisterType<Square>("SQUARE");

            return serializer;
        }

        private static JsonReader CreateReader(string json, bool canSaveState)
        {
            return canSaveState
                ? new JsonStringReader(json)
                : new JsonTextReader(
                    new StringReader(json));
        }
        
        [Test]
        public void Serialize_TargetEqualsValue()
        {
            var serializer = CreateSerializer();
            
            var shape = new Square {
                Width = 1.0,
                Height = 2.0,
            };

            var result = serializer.Serialize<Square>(shape);
            
            Assert.That(result, Is.EqualTo("{\"width\":1.0,\"height\":2.0,\"color\":\"BLACK\"}"));
        }
        
        [Test]
        public void Serialize_TargetNotEqualValue()
        {
            var serializer = CreateSerializer();
            
            var shape = new Square {
                Width = 1.0,
                Height = 2.0,
            };

            var result = serializer.Serialize<Shape>(shape);
            
            Assert.That(result, Is.EqualTo("{\"__type\":\"SQUARE\",\"width\":1.0,\"height\":2.0,\"color\":\"BLACK\"}"));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Deserialize_TargetEqualsValue(bool canSaveState)
        {
            using var jr = CreateReader("{\"width\":1.0,\"height\":2.0,\"color\":\"BLACK\"}", canSaveState);
            
            var serializer = CreateSerializer();

            var result = serializer.Deserialize<Square>(jr);
            
            Assert.That(result.Width, Is.EqualTo(1.0));
            Assert.That(result.Height, Is.EqualTo(2.0));
        }
        
        [TestCase(true)]
        [TestCase(false)]
        public void Deserialize_TargetNotEqualValue(bool canSaveState)
        {
            using var jr = CreateReader("{\"__type\":\"SQUARE\",\"width\":1.0,\"height\":2.0,\"color\":\"BLACK\"}", canSaveState);
            
            var serializer = CreateSerializer();

            var result = (Square) serializer.Deserialize<Shape>(jr);
            
            Assert.That(result.Width, Is.EqualTo(1.0));
            Assert.That(result.Height, Is.EqualTo(2.0));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Deserialize_TypeAliasIsNotString(bool canSaveState)
        {
            using var jr = CreateReader("{\"__type\":1,\"width\":1.0,\"height\":2.0,\"color\":\"BLACK\"}", canSaveState);
            
            var serializer = CreateSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Deserialize<Shape>(jr));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidTypeException>());
        }
        
        [TestCase(true)]
        [TestCase(false)]
        public void Deserialize_TypeAliasNotRegistered(bool canSaveState)
        {
            using var jr = CreateReader("{\"__type\":\"POLYGON\",\"width\":1.0,\"height\":2.0,\"color\":\"BLACK\"}", canSaveState);
            
            var serializer = CreateSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Deserialize<Shape>(jr));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidTypeException>());
        }
        
        [Test]
        public void Deserialize_TargetNotEqualValue_TypeAliasAtLast_AbleToSaveState()
        {
            using var jr = CreateReader("{\"width\":1.0,\"height\":2.0,\"color\":\"BLACK\",\"__type\":\"SQUARE\"}", true);
            
            var serializer = CreateSerializer();

            var result = (Square) serializer.Deserialize<Shape>(jr);
            
            Assert.That(result.Width, Is.EqualTo(1.0));
            Assert.That(result.Height, Is.EqualTo(2.0));
        }
        
        [Test]
        public void Deserialize_TargetNotEqualValue_TypeAliasAtLast_UnableToSaveState()
        {
            using var jr = CreateReader("{\"width\":1.0,\"height\":2.0,\"color\":\"BLACK\",\"__type\":\"SQUARE\"}", false);
            
            var serializer = CreateSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Deserialize<Shape>(jr));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidTypeException>());
        }
        
        [Test]
        public void Deserialize_TargetEqualsValue_TypeAliasAtLast_UnableToSaveState()
        {
            using var jr = CreateReader("{\"width\":1.0,\"height\":2.0,\"color\":\"BLACK\",\"__type\":\"SQUARE\"}", false);
            
            var serializer = CreateSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Deserialize<Square>(jr));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidTypeException>());
        }
        
        
    }
}