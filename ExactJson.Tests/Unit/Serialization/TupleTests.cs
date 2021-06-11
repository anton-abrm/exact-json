using ExactJson.Serialization;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization
{
    public class TupleTests
    {
        private sealed class Person
        {
            [JsonNode]
            public int ID { get; set; }

            [JsonNode]
            public string Name { get; set; }
        }
        
        private sealed class CustomOrderPerson
        {
            [JsonNode(2)]
            public int ID { get; set; }

            [JsonNode(1)]
            public string Name { get; set; }
        }

        [Test]
        public void Serialize_TupleContext()
        {
            var original = "[1,'John']".Replace('\'', '\"');

            var serializer = new JsonSerializer();

            var ctx = new JsonNodeSerializationContext {
                IsTuple = true
            };

            var person = serializer.Deserialize<Person>(original, ctx);
            
            var actual = serializer.Serialize<Person>(person, ctx);

            Assert.That(actual, Is.EqualTo(original));
        }
        
        [Test]
        public void Serialize_Tuple_CustomOrder()
        {
            var original = "['John',1]".Replace('\'', '\"');

            var serializer = new JsonSerializer();

            var ctx = new JsonNodeSerializationContext {
                IsTuple = true
            };

            var person = serializer.Deserialize<CustomOrderPerson>(original, ctx);
            
            var actual = serializer.Serialize<CustomOrderPerson>(person, ctx);

            Assert.That(actual, Is.EqualTo(original));
        }
        
        [Test]
        public void Serialize_TupleSettings()
        {
            var original = "[1,'John']".Replace('\'', '\"');

            var serializer = new JsonSerializer()
            {
                IsNodeTuple = true
            };

            var person = serializer.Deserialize<Person>(original);
            
            var actual = serializer.Serialize<Person>(person);

            Assert.That(actual, Is.EqualTo(original));
        }
        
    }
}