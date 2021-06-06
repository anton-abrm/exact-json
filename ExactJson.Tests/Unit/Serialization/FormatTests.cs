using System;
using System.Collections.Generic;
using System.Globalization;

using ExactJson.Serialization;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization
{
    public class FormatTests
    {
        private class CustomCultureAttributeClass
        {
            [JsonNode("foo")]
            [JsonFormat("D")]
            [JsonCulture("de-DE")]
            public DateTime Foo { get; set; }
        }
        
        [Test]
        public void Ctor_DefaultCulture()
        {
            var serializer = new JsonSerializer();
            
            Assert.That(serializer.FormatProvider, Is.EqualTo(CultureInfo.InvariantCulture));
        }
        
        [Test]
        public void Serialize_FormattedNumber()
        {
            var serializer = new JsonSerializer();

            var s = serializer.Serialize<decimal>(1.0m, new JsonNodeSerializationContext() {
                Format = ".2"
            });

            Assert.That(s, Is.EqualTo("1.00"));
        }
        
        [Test]
        public void Deserialize_FormattedNumber()
        {
            var serializer = new JsonSerializer();

            var result = serializer.Deserialize<decimal>("1.00", new JsonNodeSerializationContext() {
                Format = ".2"
            });

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void Serialize_MultipleFormattedNumbers()
        {
            var serializer = new JsonSerializer();

            var s = serializer.Serialize<List<int>>(
                new List<int> { 1, 2, 3, }, new JsonNodeSerializationContext() {
                    ItemContext = new JsonItemSerializationContext() {
                        Format = "2"
                    }
                });

            Assert.That(s, Is.EqualTo("[01,02,03]"));
        }
        
        [Test]
        public void Deserialize_MultipleFormattedNumbers()
        {
            var serializer = new JsonSerializer();

            var result = serializer.Deserialize<List<int>>("[01,02,03]", 
                new JsonNodeSerializationContext() { 
                    ItemContext = new JsonItemSerializationContext() {
                        Format = "2"
                }
            });

            Assert.That(result, Is.EquivalentTo(new int[]{ 1, 2, 3,}));
        }

        [Test]
        public void Serialize_DefaultCulture()
        {
            var result =new JsonSerializer().Serialize<DateTime>(new DateTime(2020, 1, 1), new JsonNodeSerializationContext() {
                Format = "D",
            });

            Assert.That(result, Is.EqualTo("\"Wednesday, 01 January 2020\""));
        }
        
        [Test]
        public void Deserialize_DefaultCulture()
        {
            var result =new JsonSerializer().Deserialize<DateTime>("\"Wednesday, 01 January 2020\"", new JsonNodeSerializationContext() {
                Format = "D",
            });

            Assert.That(result, Is.EqualTo(new DateTime(2020, 1, 1)));
        }
        
        [Test]
        public void Serialize_CustomContextCulture()
        {
            var result =new JsonSerializer().Serialize<DateTime>(new DateTime(2020, 1, 1), new JsonNodeSerializationContext() {
                Format = "D",
                FormatProvider = CultureInfo.CreateSpecificCulture("de-DE")
            });

            Assert.That(result, Is.EqualTo("\"Mittwoch, 1. Januar 2020\""));
        }
        
        [Test]
        public void Deserialize_CustomContextCulture()
        {
            var result =new JsonSerializer().Deserialize<DateTime>("\"Mittwoch, 1. Januar 2020\"", new JsonNodeSerializationContext() {
                Format = "D",
                FormatProvider = CultureInfo.CreateSpecificCulture("de-DE")
            });

            Assert.That(result, Is.EqualTo(new DateTime(2020, 1, 1)));
        }
        
        [Test]
        public void Serialize_CustomCulture()
        {
            var result =new JsonSerializer {
                FormatProvider =  CultureInfo.CreateSpecificCulture("de-DE")
            }.Serialize<DateTime>(new DateTime(2020, 1, 1), new JsonNodeSerializationContext() {
                Format = "D"
            });

            Assert.That(result, Is.EqualTo("\"Mittwoch, 1. Januar 2020\""));
        }
        
        [Test]
        public void Deserialize_CustomCulture()
        {
            var result =new JsonSerializer {
                FormatProvider =  CultureInfo.CreateSpecificCulture("de-DE")
            }.Deserialize<DateTime>("\"Mittwoch, 1. Januar 2020\"", new JsonNodeSerializationContext() {
                Format = "D"
            });

            Assert.That(result, Is.EqualTo(new DateTime(2020, 1, 1)));
        }
        
        [Test]
        public void Serialize_ClassWithCustomCultureAttribute()
        {
            var result = new JsonSerializer().Serialize<CustomCultureAttributeClass>(
                new CustomCultureAttributeClass {
                    Foo = new DateTime(2020, 1, 1)
            });

            Assert.That(result, Is.EqualTo("{\"foo\":\"Mittwoch, 1. Januar 2020\"}"));
        }
        
        [Test]
        public void Deserialize_ClassWithCustomCultureAttribute()
        {
            var result = new JsonSerializer().Deserialize<CustomCultureAttributeClass>(
                "{\"foo\":\"Mittwoch, 1. Januar 2020\"}");
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Foo, Is.EqualTo(new DateTime(2020, 1, 1)));
        }
    }
}