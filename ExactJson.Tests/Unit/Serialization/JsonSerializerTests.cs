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
        
      
        [Test]
        public void SetContext_TypeNull_ThrowsArgumentNullException()
        {
            var serializer = new JsonSerializer();

            Assert.Throws<ArgumentNullException>(() 
                => serializer.SetContext(null, new JsonNodeSerializationContext()));
        }
        
        [Test]
        public void SetContext_ContextNull_ThrowsArgumentNullException()
        {
            var serializer = new JsonSerializer();

            Assert.Throws<ArgumentNullException>(() 
                => serializer.SetContext(typeof(object), null));
        }
        
        [Test]
        public void SetContext_NullableType_ThrowsArgumentException()
        {
            var serializer = new JsonSerializer();
            
            Assert.Throws<ArgumentException>(() 
                => serializer.SetContext(typeof(int?), new JsonNodeSerializationContext()));
        }

        [Test]
        public void GetContext_Generic()
        {
            var serializer = new JsonSerializer();

            var ctx = new JsonNodeSerializationContext();
            
            serializer.SetContext<object>(ctx);
            
            Assert.That(serializer.GetContext<object>(), Is.SameAs(ctx));
        }
        
        [Test]
        public void GetContext()
        {
            var serializer = new JsonSerializer();

            var ctx = new JsonNodeSerializationContext();
            
            serializer.SetContext(typeof(object), ctx);
            
            Assert.That(serializer.GetContext(typeof(object)), Is.SameAs(ctx));
        }
        
        [Test]
        public void GetContext_HasNotBeenSet_ReturnsNull()
        {
            var serializer = new JsonSerializer();

            Assert.That(serializer.GetContext<object>(), Is.Null);
        }
        
        [Test]
        public void GetContext_TypeNull_ThrowsArgumentNullException()
        {
            var serializer = new JsonSerializer();

            Assert.Throws<ArgumentNullException>(() 
                => serializer.GetContext(null));
        }
        
        [Test]
        public void Serialize_ToWriter()
        {
            var serializer = new JsonSerializer();

            var sw = new StringWriter();
            var jw = new JsonTextWriter(sw);

            serializer.Serialize(typeof(object), jw, new object());
            
            Assert.That(sw.ToString(), Is.EqualTo("{}"));
        }

        [Test]
        public void Serialize_Generic_ToWriter()
        {
            var serializer = new JsonSerializer();

            var sw = new StringWriter();
            var jw = new JsonTextWriter(sw);

            serializer.Serialize<object>(jw, new object());
            
            Assert.That(sw.ToString(), Is.EqualTo("{}"));
        }
    }
}