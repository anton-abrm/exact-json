// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

using ExactJson.Serialization;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization.Features
{
    public class TupleTests
    {
        private abstract class Base { }

        private sealed class Derived : Base
        {
            [JsonNode]
            public string Foo { get; set; }
        }

        private sealed class PersonWithOptionalName
        {
            [JsonNode]
            [JsonOptional]
            public string Name { get; set; }
        }

        private sealed class PersonWithNoSetter
        {
            [JsonNode]
            public string Name { get; } = "Bob";
        }

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
        
        [Test]
        public void Serialize_OptionalField()
        {
            var original = "[null]";

            var serializer = new JsonSerializer() {
                IsNodeTuple = true,
                SerializeNullProperty = true
            };

            var person = serializer.Deserialize<PersonWithOptionalName>(original);
            
            var actual = serializer.Serialize<PersonWithOptionalName>(person);

            Assert.That(actual, Is.EqualTo(original));
        }

        [Test]
        public void Serialize_Derived()
        {
            var original = "[\"DERIVED\",\"Bar\"]";

            var serializer = new JsonSerializer() {
                IsNodeTuple = true,
            };

            serializer.RegisterType<Derived>("DERIVED");

            var obj = serializer.Deserialize<Base>(original);

            var actual = serializer.Serialize<Base>(obj);

            Assert.That(actual, Is.EqualTo(original));
        }

        [Test]
        public void Serialize_NoSetter()
        {
            var serializer = new JsonSerializer() {
                IsNodeTuple = true,
            };

            var obj = serializer.Deserialize<PersonWithNoSetter>("[\"Alice\"]");

            var actual = serializer.Serialize<PersonWithNoSetter>(obj);

            Assert.That(actual, Is.EqualTo("[\"Bob\"]"));
        }

        [TestCase("[1,\"John\", 4]")]
        [TestCase("[1]")]
        public void Deserialize_InvalidFieldCount(string json)
        {
            var serializer = new JsonSerializer();

            var ctx = new JsonNodeSerializationContext {
                IsTuple = true
            };
            
            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Deserialize<Person>(json, ctx));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidValueException>());
        }

        [Test]
        public void Serialize_BoundTuple()
        {
            var json = "[1,\"John\"]";
            
            var serializer = new JsonSerializer();
            
            serializer.SetContext<Person>(new JsonNodeSerializationContext() {
                IsTuple = true
            });

            var person = serializer.Deserialize<Person>(json);
            var result = serializer.Serialize<Person>(person);
            
            Assert.That(result, Is.EqualTo(json));
        }
        
        
    }
}