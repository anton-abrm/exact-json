using System.Collections.Generic;

using ExactJson.Serialization;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization.Features
{
    public class TupleTests
    {
        [JsonTuple]
        private sealed class PersonWithTupleAttribute
        {
            [JsonNode]
            public int ID { get; set; }

            [JsonNode]
            public string Name { get; set; }
        }
        
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
        public void Serialize_TupleContext_CustomOrder()
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
        
        [Test]
        public void Serialize_TupleItemContext()
        {
            var original = "[[1,'John']]".Replace('\'', '\"');

            var serializer = new JsonSerializer();

            var ctx = new JsonNodeSerializationContext {
                ItemContext = new JsonItemSerializationContext() {
                    IsTuple = true
                }
            };

            var persons = serializer.Deserialize<List<Person>>(original, ctx);
            
            var actual = serializer.Serialize<List<Person>>(persons, ctx);

            Assert.That(actual, Is.EqualTo(original));
        }
        
        [Test]
        public void Serialize_TupleAttribute()
        {
            var original = "[1,'John']".Replace('\'', '\"');

            var serializer = new JsonSerializer();

            var person = serializer.Deserialize<PersonWithTupleAttribute>(original);
            
            var actual = serializer.Serialize<PersonWithTupleAttribute>(person);

            Assert.That(actual, Is.EqualTo(original));
        }
        
    }
}