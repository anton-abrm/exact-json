using System;
using System.Collections.Generic;
using System.IO;

using ExactJson.Serialization;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization
{
    public sealed class JsonSerializerTests
    {
        [Test]
        public void RegisterType_TypeNull_ThrowsArgumentNullException()
        {
            var serializer = new JsonSerializer();

            Assert.Throws<ArgumentNullException>(() 
                => serializer.RegisterType(null, ""));
        }
        
        [Test]
        public void RegisterType_AliasNull_ThrowsArgumentNullException()
        {
            var serializer = new JsonSerializer();
            
            Assert.Throws<ArgumentNullException>(() 
                => serializer.RegisterType(typeof(object), null));
        }
        
        [Test]
        public void RegisterType_NullableType_ThrowsArgumentException()
        {
            var serializer = new JsonSerializer();
            
            Assert.Throws<ArgumentException>(() 
                => serializer.RegisterType(typeof(int?), ""));
        }
        
        [Test]
        public void RegisterType_NotObjectType_ThrowsArgumentException()
        {
            var serializer = new JsonSerializer();
            
            Assert.Throws<ArgumentException>(() 
                => serializer.RegisterType(typeof(List<int>), ""));
        }
        
        [Test]
        public void Deserialize_TypeNull_ThrowsArgumentNullException()
        {
            var serializer = new JsonSerializer();

            Assert.Throws<ArgumentNullException>(() 
                => serializer.Deserialize(null, new JsonStringReader("")));
        }
        
        [Test]
        public void Deserialize_ReaderNull_ThrowsArgumentNullException()
        {
            var serializer = new JsonSerializer();
            
            Assert.Throws<ArgumentNullException>(() 
                => serializer.Deserialize(typeof(object), (JsonReader) null));
        }
        
        [Test]
        public void Deserialize_ReaderAtEOF_ThrowsEndOfStreamException()
        {
            var serializer = new JsonSerializer();
            
            Assert.Throws<EndOfStreamException>(() 
                => serializer.Deserialize(typeof(object), new JsonStringReader("")));
        }
    }
}