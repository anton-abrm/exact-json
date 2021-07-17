using System;
using System.Collections.Generic;

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
    }
}