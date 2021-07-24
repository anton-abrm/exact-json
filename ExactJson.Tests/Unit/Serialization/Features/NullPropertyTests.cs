// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using ExactJson.Serialization;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization.Features
{
    public class NullPropertyTests
    {
        private sealed class NoNullAttributeClass
        {
            [JsonNode("foo"), JsonOptional]
            public string Foo { get; set; }
        }
        
        private sealed class DefaultNullAttributeClass
        {
            [JsonSerializeNull]
            [JsonNode("foo"), JsonOptional]
            public string Foo { get; set; }
        }
        
        private sealed class TrueNullAttributeClass
        {
            [JsonSerializeNull(true)]
            [JsonNode("foo"), JsonOptional]
            public string Foo { get; set; }
        }
        
        private sealed class FalseNullAttributeClass
        {
            [JsonSerializeNull(false)]
            [JsonNode("foo"), JsonOptional]
            public string Foo { get; set; }
        }
        
        [Test]
        public void Serialize_SerializeNullProperty_FalseByDefault()
        {
            var serializer = new JsonSerializer();
            
            Assert.That(serializer.SerializeNullProperty, Is.False);
        }
        
        [Test]
        public void Serialize_NoNullAttribute_SerializeNullProperty_False()
        {
            var obj = new NoNullAttributeClass();
            
            var serializer = new JsonSerializer() {
                SerializeNullProperty = false
            };

            var json = serializer.Serialize<NoNullAttributeClass>(obj);
            
            Assert.That(json, Is.EqualTo("{}"));
        }
        
        [Test]
        public void Serialize_NoNullAttribute_SerializeNullProperty_True()
        {
            var obj = new NoNullAttributeClass();
            
            var serializer = new JsonSerializer() {
                SerializeNullProperty = true
            };

            var json = serializer.Serialize<NoNullAttributeClass>(obj);
            
            Assert.That(json, Is.EqualTo("{\"foo\":null}"));
        }

        [Test]
        public void Serialize_DefaultNullAttribute()
        {
            var obj = new DefaultNullAttributeClass();
            
            var serializer = new JsonSerializer();

            var json = serializer.Serialize<DefaultNullAttributeClass>(obj);
            
            Assert.That(json, Is.EqualTo("{\"foo\":null}"));
        }
        
        [Test]
        public void Serialize_TrueNullAttribute()
        {
            var demo = new TrueNullAttributeClass();
            
            var serializer = new JsonSerializer();

            var json = serializer.Serialize<TrueNullAttributeClass>(demo);
            
            Assert.That(json, Is.EqualTo("{\"foo\":null}"));
        }
        
        [Test]
        public void Serialize_FalseNullAttribute()
        {
            var obj = new FalseNullAttributeClass();
            
            var serializer = new JsonSerializer();

            var json = serializer.Serialize<FalseNullAttributeClass>(obj);
            
            Assert.That(json, Is.EqualTo("{}"));
        }
        
        [Test]
        public void Serialize_Bound_SerializeNullProperty_True()
        {
            var obj = new NoNullAttributeClass();
            
            var serializer = new JsonSerializer();
            
            serializer.SetContext<string>(new JsonNodeSerializationContext()
            {
                SerializeNullProperty = true
            });

            var json = serializer.Serialize<NoNullAttributeClass>(obj);
            
            Assert.That(json, Is.EqualTo("{\"foo\":null}"));
        }
    }
}