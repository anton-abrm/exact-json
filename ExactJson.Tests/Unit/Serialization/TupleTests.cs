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

        [Test]
        public void Serialize_Tuple()
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
    }
}