using ExactJson.Serialization;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization.Features
{
    public class NecessityTests
    {
        private sealed class EntityRequired
        {
            [JsonNode("foo"), JsonRequired]
            public string Foo { get; set; }
        }
        
        private sealed class EntityOptional
        {
            [JsonNode("foo"), JsonOptional]
            public string Foo { get; set; }
        }
        
        private sealed class EntityUndefined
        {
            [JsonNode("foo")]
            public string Foo { get; set; }
        }

        #region Serialize
        
        [Test]
        public void Serialize_RequiredByDefault_RequiredSet()
        {
            var p = new EntityRequired() {
                Foo = "bar"
            };
            
            var result = new JsonSerializer().Serialize<EntityRequired>(p);
            
            Assert.That(result, Is.EqualTo("{\"foo\":\"bar\"}"));
        }
        
        [Test]
        public void Serialize_RequiredByDefault_RequiredNotSet_ThrowsJsonMissingRequiredPropertyException()
        {
            var p = new EntityRequired();
            
            Assert.That(p.Foo, Is.Null);

            var ex = Assert.Throws<JsonSerializationException>(() =>
            {
                new JsonSerializer().Serialize<EntityRequired>(p);
            });

           Assert.That(ex.Pointer, Is.EqualTo("/foo"));
           Assert.That(ex.InnerException, Is.InstanceOf(typeof(JsonMissingRequiredValueException)));
        }
        
        [Test]
        public void Serialize_OptionalByDefault_RequiredSet()
        {
            var p = new EntityRequired() {
                Foo = "bar"
            };
            
            var result =  new JsonSerializer { IsNodeOptional = true}.Serialize<EntityRequired>(p);
            
            Assert.That(result, Is.EqualTo("{\"foo\":\"bar\"}"));
        }
        
        [Test]
        public void Serialize_OptionalByDefault_RequiredNotSet_ThrowsJsonMissingRequiredPropertyException()
        {
            var p = new EntityRequired();
            
            Assert.That(p.Foo, Is.Null);

            var ex = Assert.Throws<JsonSerializationException>(() =>
            {
                new JsonSerializer { IsNodeOptional = true}.Serialize<EntityRequired>(p);
            });

            Assert.That(ex.Pointer, Is.EqualTo("/foo"));
            Assert.That(ex.InnerException, Is.InstanceOf(typeof(JsonMissingRequiredValueException)));
        }
        
        
        [Test]
        public void Serialize_RequiredByDefault_OptionalSet()
        {
            var p = new EntityOptional {
                Foo = "bar"
            };
            
            var result = new JsonSerializer().Serialize<EntityOptional>(p);
            
            Assert.That(result, Is.EqualTo("{\"foo\":\"bar\"}"));
        }
        
        [Test]
        public void Serialize_RequiredByDefault_OptionalNotSet_ThrowsJsonMissingRequiredPropertyException()
        {
            var p = new EntityOptional();
            
            var result = new JsonSerializer().Serialize<EntityOptional>(p);
            
            Assert.That(result, Is.EqualTo("{}")); 
        }
        
        [Test]
        public void Serialize_OptionalByDefault_OptionalSet()
        {
            var p = new EntityOptional {
                Foo = "bar"
            };
            
            var result =  new JsonSerializer { IsNodeOptional = true}.Serialize<EntityOptional>(p);
            
            Assert.That(result, Is.EqualTo("{\"foo\":\"bar\"}"));
        }
        
        [Test]
        public void Serialize_OptionalByDefault_OptionalNotSet_ThrowsJsonMissingRequiredPropertyException()
        {
            var p = new EntityOptional();
            
            var result =  new JsonSerializer { IsNodeOptional = true}.Serialize<EntityOptional>(p);
            
            Assert.That(result, Is.EqualTo("{}"));
        }
        
        
        [Test]
        public void Serialize_RequiredByDefault_UndefinedSet()
        {
            var p = new EntityUndefined() {
                Foo = "bar"
            };
            
            var result = new JsonSerializer().Serialize<EntityUndefined>(p);
            
            Assert.That(result, Is.EqualTo("{\"foo\":\"bar\"}"));
        }
        
        [Test]
        public void Serialize_RequiredByDefault_UndefinedNotSet_ThrowsJsonMissingRequiredPropertyException()
        {
            var p = new EntityUndefined();
            
            Assert.That(p.Foo, Is.Null);

            var ex = Assert.Throws<JsonSerializationException>(() =>
            {
                new JsonSerializer().Serialize<EntityUndefined>(p);
            });

            Assert.That(ex.Pointer, Is.EqualTo("/foo"));
            Assert.That(ex.InnerException, Is.InstanceOf(typeof(JsonMissingRequiredValueException)));
        }
        
        [Test]
        public void Serialize_OptionalByDefault_UndefinedSet()
        {
            var p = new EntityUndefined() {
                Foo = "bar"
            };
            
            var result =  new JsonSerializer { IsNodeOptional = true}.Serialize<EntityUndefined>(p);
            
            Assert.That(result, Is.EqualTo("{\"foo\":\"bar\"}"));
        }
        
        [Test]
        public void Serialize_OptionalByDefault_UndefinedNotSet_ThrowsJsonMissingRequiredPropertyException()
        {
            var p = new EntityUndefined();
            
            var result =  new JsonSerializer { IsNodeOptional = true}.Serialize<EntityUndefined>(p);
            
            Assert.That(result, Is.EqualTo("{}"));
        }
        
        #endregion
        
        #region Deserialize
        
        [Test]
        public void Deserialize_RequiredByDefault_RequiredSet()
        {
            var result = new JsonSerializer().Deserialize<EntityRequired>(
                "{\"foo\":\"bar\"}");
            
            Assert.That(result.Foo, Is.EqualTo("bar"));
        }
        
        [Test]
        public void Deserialize_RequiredByDefault_RequiredNotSet_ThrowsJsonMissingRequiredPropertyException()
        {
            var ex = Assert.Throws<JsonSerializationException>(() =>
            {
                new JsonSerializer().Deserialize<EntityRequired>("{}");
            });

           Assert.That(ex.Pointer, Is.EqualTo("/foo"));
           Assert.That(ex.InnerException, Is.InstanceOf(typeof(JsonMissingRequiredValueException)));
        }
        
        [Test]
        public void Deserialize_OptionalByDefault_RequiredSet()
        {
            var result = new JsonSerializer{ IsNodeOptional = true }.Deserialize<EntityRequired>(
                "{\"foo\":\"bar\"}");
            
            Assert.That(result.Foo, Is.EqualTo("bar"));
        }
        
        [Test]
        public void Deserialize_OptionalByDefault_RequiredNotSet_ThrowsJsonMissingRequiredPropertyException()
        {
            var ex = Assert.Throws<JsonSerializationException>(() =>
            {
                new JsonSerializer{ IsNodeOptional = true }.Deserialize<EntityRequired>("{}");
            });

            Assert.That(ex.Pointer, Is.EqualTo("/foo"));
            Assert.That(ex.InnerException, Is.InstanceOf(typeof(JsonMissingRequiredValueException)));
        }
        
        
        [Test]
        public void Deserialize_RequiredByDefault_OptionalSet()
        {
            var result = new JsonSerializer().Deserialize<EntityOptional>(
                "{\"foo\":\"bar\"}");
            
            Assert.That(result.Foo, Is.EqualTo("bar"));
        }
        
        [Test]
        public void Deserialize_RequiredByDefault_OptionalNotSet_ThrowsJsonMissingRequiredPropertyException()
        {
            var result = new JsonSerializer().Deserialize<EntityOptional>(
                "{}");
            
            Assert.That(result.Foo, Is.Null);
        }
        
        [Test]
        public void Deserialize_OptionalByDefault_OptionalSet()
        {
            var result = new JsonSerializer { IsNodeOptional = true }.Deserialize<EntityOptional>(
                "{\"foo\":\"bar\"}");
            
            Assert.That(result.Foo, Is.EqualTo("bar"));
        }
        
        [Test]
        public void Deserialize_OptionalByDefault_OptionalNotSet_ThrowsJsonMissingRequiredPropertyException()
        {
            var result = new JsonSerializer { IsNodeOptional = true}
               .Deserialize<EntityOptional>("{}");
            
            Assert.That(result.Foo, Is.Null);
        }
        
        
        [Test]
        public void Deserialize_RequiredByDefault_UndefinedSet()
        {
            var result = new JsonSerializer().Deserialize<EntityUndefined>(
                "{\"foo\":\"bar\"}");
            
            Assert.That(result.Foo, Is.EqualTo("bar"));
        }
        
        [Test]
        public void Deserialize_RequiredByDefault_UndefinedNotSet_ThrowsJsonMissingRequiredPropertyException()
        {
            var ex = Assert.Throws<JsonSerializationException>(() =>
            {
                new JsonSerializer().Deserialize<EntityUndefined>("{}");
            });

            Assert.That(ex.Pointer, Is.EqualTo("/foo"));
            Assert.That(ex.InnerException, Is.InstanceOf(typeof(JsonMissingRequiredValueException)));
        }
        
        [Test]
        public void Deserialize_OptionalByDefault_UndefinedSet()
        {
            var result = new JsonSerializer { IsNodeOptional = true }.Deserialize<EntityUndefined>(
                "{\"foo\":\"bar\"}");
            
            Assert.That(result.Foo, Is.EqualTo("bar"));
        }
        
        [Test]
        public void Deserialize_OptionalByDefault_UndefinedNotSet_ThrowsJsonMissingRequiredPropertyException()
        {
            var result = new JsonSerializer { IsNodeOptional = true}
               .Deserialize<EntityUndefined>("{}");
            
            Assert.That(result.Foo, Is.Null);
        }
        
        #endregion
        
    }
}