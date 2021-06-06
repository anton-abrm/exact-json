using ExactJson.Serialization;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization
{
    public class NullPropertySerializationTests
    {
        private sealed class DemoClass
        {
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
        public void SerializeNullProperty_False()
        {
            var demo = new DemoClass();
            
            var serializer = new JsonSerializer() {
                SerializeNullProperty = false
            };

            var json = serializer.Serialize<DemoClass>(demo);
            
            Assert.That(json, Is.EqualTo("{}"));
        }
        
        [Test]
        public void SerializeNullProperty_True()
        {
            var demo = new DemoClass();
            
            var serializer = new JsonSerializer() {
                SerializeNullProperty = true
            };

            var json = serializer.Serialize<DemoClass>(demo);
            
            Assert.That(json, Is.EqualTo("{\"foo\":null}"));
        }
    }
}