// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using ExactJson.Serialization;
using ExactJson.Serialization.Converters;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization.Features
{
    public class ConverterTests
    {
        private sealed class UriConverter : JsonConverter
        {
            public override string GetString(object value, JsonConverterContext context)
            {
               return ((Uri) value).ToString();
            }

            public override object GetValue(string s, JsonConverterContext context)
            {
                return new Uri(s);
            }
        }

        private sealed class Profile
        {
            [JsonNode("link")]
            [JsonOptional]
            [JsonConverter(typeof(UriConverter))]
            public Uri Link { get; set; }
        }
        
        private sealed class Transaction
        {
            [JsonConverter(typeof(JsonNumberConverter), SkipForNonStringValues = true)]
            [JsonNode("amount")]
            public decimal Amount { get; set; }
        }

        [Test]
        public void Serialize_PropertyWithConverter_UsesConverter()
        {
            var p = new Profile
            {
                Link = new Uri("http://localhost"),
            };

            var serializer = new JsonSerializer();

            var result = serializer.Serialize<Profile>(p);

            Assert.That(result, Is.EqualTo("{\"link\":\"http://localhost/\"}"));
        }

        [Test]
        public void Deserialize_PropertyWithConverter_UsesConverter()
        {
            var serializer = new JsonSerializer();

            var result = serializer.Deserialize<Profile>("{\"link\":\"http://localhost/\"}");

            Assert.That(result.Link, Is.EqualTo(new Uri("http://localhost")));
        }
        
        [Test]
        public void Serialize_NullPropertyWithConverter()
        {
            var p = new Profile();

            var serializer = new JsonSerializer();

            var result = serializer.Serialize<Profile>(p);

            Assert.That(result, Is.EqualTo("{}"));
        }

        [Test]
        public void Deserialize_NullPropertyWithConverter()
        {
            var serializer = new JsonSerializer();

            var result = serializer.Deserialize<Profile>("{}");

            Assert.That(result.Link, Is.Null);
        }
        
        [Test]
        public void Serialize_ExplicitNullPropertyWithConverter()
        {
            var p = new Profile();

            var serializer = new JsonSerializer() {
                SerializeNullProperty = true
            };

            var result = serializer.Serialize<Profile>(p);

            Assert.That(result, Is.EqualTo("{\"link\":null}"));
        }

        [Test]
        public void Deserialize_ExplicitNullPropertyWithConverter()
        {
            var serializer = new JsonSerializer();

            var result = serializer.Deserialize<Profile>("{\"link\":null}");

            Assert.That(result.Link, Is.Null);
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
        
        
        
        [Test]
        public void Serialize_SkipForNonStringValues()
        {
            var t = new Transaction {
               Amount = 123.45m
            };

            var serializer = new JsonSerializer();

            var result = serializer.Serialize<Transaction>(t);

            Assert.That(result, Is.EqualTo("{\"amount\":\"123.45\"}"));
        }
        
        [Test]
        public void Deserialize_SkipForNonStringValues_String()
        {
            var serializer = new JsonSerializer();

            var result = serializer.Deserialize<Transaction>("{\"amount\":\"123.45\"}");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Amount, Is.EqualTo(123.45m));
        }
        
        [Test]
        public void Deserialize_SkipForNonStringValues_Number()
        {
            var serializer = new JsonSerializer();

            var result = serializer.Deserialize<Transaction>("{\"amount\":123.45}");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Amount, Is.EqualTo(123.45m));
        }
    }
}