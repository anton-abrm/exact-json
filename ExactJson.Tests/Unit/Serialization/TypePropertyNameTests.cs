using System.Collections.Generic;

using ExactJson.Serialization;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization
{
    public sealed class TypePropertyNameTests
    {
        private abstract class Base
        { }
        
        private class Derived : Base
        { }

        [JsonTypePropertyName("$type")]
        private abstract class BaseWithAttribute
        { }
        
        private class DerivedWithAttribute : BaseWithAttribute
        { }

        private static JsonSerializer CreateSerializer()
        {
            var serializer = new JsonSerializer();
            
            serializer.RegisterType<Derived>("DERIVED");
            serializer.RegisterType<DerivedWithAttribute>("DERIVED_WITH_ATTRIBUTE");

            return serializer;
        }
        
        [Test]
        public void TypePropertyName_Default()
        {
            var serializer = new JsonSerializer();
            
            Assert.That(serializer.TypePropertyName, Is.EqualTo("__type"));
        }
        
        [Test]
        public void Serialize_TypePropertyName_DefaultFallback()
        {
            var serializer = CreateSerializer();

            var json = serializer.Serialize<Base>(new Derived());
            
            Assert.That(json, Is.EqualTo("{\"__type\":\"DERIVED\"}"));
        }
        
        [Test]
        public void Deserialize_TypePropertyName_DefaultFallback()
        {
            var serializer = CreateSerializer();

            var result = serializer.Deserialize<Base>("{\"__type\":\"DERIVED\"}");
            
            Assert.That(result, Is.InstanceOf<Derived>());
        }
        
        [Test]
        public void Serialize_TypePropertyName_CustomFallback()
        {
            var serializer = CreateSerializer();

            serializer.TypePropertyName = "$type";

            var json = serializer.Serialize<Base>(new Derived());
            
            Assert.That(json, Is.EqualTo("{\"$type\":\"DERIVED\"}"));
        }
        
        [Test]
        public void Deserialize_TypePropertyName_CustomFallback()
        {
            var serializer = CreateSerializer();
            
            serializer.TypePropertyName = "$type";

            var result = serializer.Deserialize<Base>("{\"$type\":\"DERIVED\"}");
            
            Assert.That(result, Is.InstanceOf<Derived>());
        }
        
        [Test]
        public void Serialize_TypePropertyName_LocalContext()
        {
            var serializer = CreateSerializer();

            var json = serializer.Serialize<Base>(new Derived(), new JsonNodeSerializationContext {
                TypePropertyName = "$type"
            });
            
            Assert.That(json, Is.EqualTo("{\"$type\":\"DERIVED\"}"));
        }
        
        [Test]
        public void Deserialize_TypePropertyName_LocalContext()
        {
            var serializer = CreateSerializer();

            var result = serializer.Deserialize<Base>("{\"$type\":\"DERIVED\"}", new JsonNodeSerializationContext {
                TypePropertyName = "$type"
            });
            
            Assert.That(result, Is.InstanceOf<Derived>());
        }
        
        [Test]
        public void Serialize_TypePropertyName_Attribute()
        {
            var serializer = CreateSerializer();

            var json = serializer.Serialize<BaseWithAttribute>(new DerivedWithAttribute());
            
            Assert.That(json, Is.EqualTo("{\"$type\":\"DERIVED_WITH_ATTRIBUTE\"}"));
        }
        
        [Test]
        public void Deserialize_TypePropertyName_Attribute()
        {
            var serializer = CreateSerializer();

            var result = serializer.Deserialize<BaseWithAttribute>("{\"$type\":\"DERIVED_WITH_ATTRIBUTE\"}");
            
            Assert.That(result, Is.InstanceOf<DerivedWithAttribute>());
        }
        
        [Test]
        public void Serialize_TypePropertyName_LocalItemContext()
        {
            var serializer = CreateSerializer();

            var json = serializer.Serialize<List<Base>>(new List<Base>(){ new Derived() }, new JsonNodeSerializationContext {
                ItemContext = new JsonItemSerializationContext() {
                    TypePropertyName = "$type"
                }
            });
            
            Assert.That(json, Is.EqualTo("[{\"$type\":\"DERIVED\"}]"));
        }
        
        [Test]
        public void Deserialize_TypePropertyName_LocalItemContext()
        {
            var serializer = CreateSerializer();

            var result = serializer.Deserialize<List<Base>>("[{\"$type\":\"DERIVED\"}]", new JsonNodeSerializationContext {
                ItemContext = new JsonItemSerializationContext() {
                    TypePropertyName = "$type"
                }
            });
            
            Assert.That(result, Is.InstanceOf<List<Base>>());
            Assert.That(result[0], Is.InstanceOf<Derived>());
        }
    }

}