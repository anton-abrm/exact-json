using ExactJson.Serialization;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization
{
    public class NullPropertySerializationTests
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
        public void SerializeNullProperty_FalseByDefault()
        {
            var serializer = new JsonSerializer();
            
            Assert.That(serializer.SerializeNullProperty, Is.False);
        }
        
        [Test]
        public void NoNullAttribute_SerializeNullProperty_False()
        {
            var obj = new NoNullAttributeClass();
            
            var serializer = new JsonSerializer() {
                SerializeNullProperty = false
            };

            var json = serializer.Serialize<NoNullAttributeClass>(obj);
            
            Assert.That(json, Is.EqualTo("{}"));
        }
        
        [Test]
        public void NoNullAttribute_SerializeNullProperty_True()
        {
            var obj = new NoNullAttributeClass();
            
            var serializer = new JsonSerializer() {
                SerializeNullProperty = true
            };

            var json = serializer.Serialize<NoNullAttributeClass>(obj);
            
            Assert.That(json, Is.EqualTo("{\"foo\":null}"));
        }

        [Test]
        public void DefaultNullAttribute()
        {
            var obj = new DefaultNullAttributeClass();
            
            var serializer = new JsonSerializer();

            var json = serializer.Serialize<DefaultNullAttributeClass>(obj);
            
            Assert.That(json, Is.EqualTo("{\"foo\":null}"));
        }
        
        [Test]
        public void TrueNullAttribute()
        {
            var demo = new TrueNullAttributeClass();
            
            var serializer = new JsonSerializer();

            var json = serializer.Serialize<TrueNullAttributeClass>(demo);
            
            Assert.That(json, Is.EqualTo("{\"foo\":null}"));
        }
        
        [Test]
        public void FalseNullAttribute()
        {
            var obj = new FalseNullAttributeClass();
            
            var serializer = new JsonSerializer();

            var json = serializer.Serialize<FalseNullAttributeClass>(obj);
            
            Assert.That(json, Is.EqualTo("{}"));
        }
    }
}