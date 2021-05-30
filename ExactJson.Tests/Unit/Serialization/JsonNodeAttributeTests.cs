using ExactJson.Serialization;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization
{
    public sealed class JsonNodeAttributeTests
    {
        [Test]
        public void Ctor_Default()
        {
            var attr = new JsonNodeAttribute();
            
            Assert.That(attr.Name, Is.Null);
            Assert.That(attr.Position, Is.Zero);
        }
        
        [Test]
        public void Ctor_Name()
        {
            var attr = new JsonNodeAttribute("foo");
            
            Assert.That(attr.Name, Is.EqualTo("foo"));
            Assert.That(attr.Position, Is.Zero);
        }
        
        [Test]
        public void Ctor_Position()
        {
            var attr = new JsonNodeAttribute(1);
            
            Assert.That(attr.Name, Is.Null);
            Assert.That(attr.Position, Is.EqualTo(1));
        }
        
        [Test]
        public void Ctor_NamePosition()
        {
            var attr = new JsonNodeAttribute("foo", 1);
            
            Assert.That(attr.Name, Is.EqualTo("foo"));
            Assert.That(attr.Position, Is.EqualTo(1));
        }
    }
}