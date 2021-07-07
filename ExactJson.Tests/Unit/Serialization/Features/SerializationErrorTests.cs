using System;
using System.Collections.Generic;

using ExactJson.Serialization;
using ExactJson.Serialization.Converters;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization.Features
{
    public sealed class SerializationErrorTests
    {
        private enum EnumWithoutAttributes
        {
            Foo
        }
        
        private sealed class ClassWithNoGetter
        {
            [JsonNode]
            public string Foo 
            {
                set {}
            }
        }
        
        private abstract class BaseClass
        {
        }
        
        private sealed class DerivedClass : BaseClass
        {
        }

        [Test]
        public void Serialize_ClassWithNoGetter_ThrowsJsonInvalidTypeException()
        {
            var value = new ClassWithNoGetter();

            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Serialize<ClassWithNoGetter>(value));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidTypeException>());
        }
        
        [Test]
        public void Deserialize_ClassWithNoGetter_ThrowsJsonInvalidTypeException()
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Deserialize<ClassWithNoGetter>("{\"Foo\":\"bar\"}"));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidTypeException>());
        }
        
        [Test]
        public void Serialize_EnumWithoutAttributes_ThrowsJsonInvalidValueException()
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Serialize<EnumWithoutAttributes>(EnumWithoutAttributes.Foo));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidValueException>());
        }
        
        [Test]
        public void Deserialize_EnumWithoutAttributes_ThrowsJsonInvalidValueException()
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Deserialize<EnumWithoutAttributes>("\"Bar\""));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidValueException>());
        }

        [Test]
        public void Serialize_ICollection_JsonInvalidTypeException()
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Serialize<ICollection<string>>(new List<string>()));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidTypeException>());
        }
        
        [Test]
        public void Deserialize_ICollection_JsonInvalidTypeException()
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Deserialize<ICollection<string>>("[]"));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidTypeException>());
        }
        
        [Test]
        public void Serialize_IDictionary_JsonInvalidTypeException()
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Serialize<IDictionary<string, string>>(new Dictionary<string, string>()));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidTypeException>());
        }
        
        [Test]
        public void Deserialize_IDictionary_JsonInvalidTypeException()
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Deserialize<IDictionary<string, string>>("{}"));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidTypeException>());
        }

        [Test]
        public void Serialize_DerivedClassWithoutAlias()
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Serialize<BaseClass>(new DerivedClass()));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidTypeException>());
        }
        
        [Test]
        public void Serialize_DictionaryWithInvalidEnumAsKey()
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Serialize<Dictionary<EnumWithoutAttributes, string>>(new Dictionary<EnumWithoutAttributes, string>() {
                    [EnumWithoutAttributes.Foo] = "Bar"
                }));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidValueException>());
        }
        
        [Test]
        public void Deserialize_DictionaryWithInvalidEnumAsKey()
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Deserialize<Dictionary<EnumWithoutAttributes, string>>("{\"Foo\":\"Bar\"}"));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidValueException>());
        }
        
        [Test]
        public void Deserialize_ConvertedValueNotString()
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Deserialize<int>("true", new JsonNodeSerializationContext() {
                    Converter = new JsonNumberConverter()
                }));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidTypeException>());
        }
    }
}