using System;
using System.Collections.Generic;
using System.Globalization;

using ExactJson.Serialization;
using ExactJson.Serialization.Converters;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization
{
    public class FormatTests
    {
        private sealed class ClassWithFormattedField
        {
            [JsonNode("foo")]
            [JsonFormat("D")]
            [JsonCulture("de-DE")]
            public DateTime Foo { get; set; }
        }
        
        private sealed class ClassWithFormattedMapKey
        {
            [JsonNode("foo")]
            [JsonFormat("D",ApplyTo = JsonNodeTarget.Key)]
            [JsonCulture("de-DE", ApplyTo = JsonNodeTarget.Key)]
            public Dictionary<DateTime, string> Foo { get; set; }
        }
        
        private sealed class ClassWithFormattedMapItem
        {
            [JsonNode("foo")]
            [JsonFormat("D",ApplyTo = JsonNodeTarget.Item)]
            [JsonCulture("de-DE", ApplyTo = JsonNodeTarget.Item)]
            public Dictionary<string, DateTime> Foo { get; set; }
        }

        private static JsonSerializer CreateSerializer()
        {
            var serializer = new JsonSerializer();

            serializer.SetContext<DateTime>(new JsonNodeSerializationContext {
                Converter = JsonDateTimeConverter.Default
            });

            return serializer;
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
            var result = CreateSerializer().Serialize<DateTime>(new DateTime(2020, 1, 1), new JsonNodeSerializationContext() {
                Format = "D",
            });

            Assert.That(result, Is.EqualTo("\"Wednesday, 01 January 2020\""));
        }
        
        [Test]
        public void Deserialize_DefaultCulture()
        {
            var result = CreateSerializer().Deserialize<DateTime>("\"Wednesday, 01 January 2020\"", new JsonNodeSerializationContext() {
                Format = "D",
            });

            Assert.That(result, Is.EqualTo(new DateTime(2020, 1, 1)));
        }
        
        [Test]
        public void Serialize_CustomContextCulture()
        {
            var result = CreateSerializer().Serialize<DateTime>(new DateTime(2020, 1, 1), new JsonNodeSerializationContext() {
                Format = "D",
                FormatProvider = CultureInfo.CreateSpecificCulture("de-DE")
            });

            Assert.That(result, Is.EqualTo("\"Mittwoch, 1. Januar 2020\""));
        }
        
        [Test]
        public void Deserialize_CustomContextCulture()
        {
            var result = CreateSerializer().Deserialize<DateTime>("\"Mittwoch, 1. Januar 2020\"", new JsonNodeSerializationContext() {
                Format = "D",
                FormatProvider = CultureInfo.CreateSpecificCulture("de-DE")
            });

            Assert.That(result, Is.EqualTo(new DateTime(2020, 1, 1)));
        }
        
        [Test]
        public void Serialize_CustomCulture()
        {
            var serializer = CreateSerializer();

            serializer.FormatProvider = CultureInfo.CreateSpecificCulture("de-DE");
            
            var result = serializer.Serialize<DateTime>(new DateTime(2020, 1, 1), new JsonNodeSerializationContext() {
                Format = "D"
            });

            Assert.That(result, Is.EqualTo("\"Mittwoch, 1. Januar 2020\""));
        }
        
        [Test]
        public void Deserialize_CustomCulture()
        {
            var serializer = CreateSerializer();
            
            serializer.FormatProvider = CultureInfo.CreateSpecificCulture("de-DE");
            
            var result = serializer.Deserialize<DateTime>("\"Mittwoch, 1. Januar 2020\"", new JsonNodeSerializationContext() {
                Format = "D"
            });

            Assert.That(result, Is.EqualTo(new DateTime(2020, 1, 1)));
        }
        
        [Test]
        public void Serialize_CultureAttribute()
        {
            var serializer = CreateSerializer();

            var result = serializer.Serialize<ClassWithFormattedField>(
                new ClassWithFormattedField {
                    Foo = new DateTime(2020, 1, 1)
            });

            Assert.That(result, Is.EqualTo("{\"foo\":\"Mittwoch, 1. Januar 2020\"}"));
        }
        
        [Test]
        public void Deserialize_CultureAttribute()
        {
            var result = CreateSerializer().Deserialize<ClassWithFormattedField>(
                "{\"foo\":\"Mittwoch, 1. Januar 2020\"}");
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Foo, Is.EqualTo(new DateTime(2020, 1, 1)));
        }
        
        [Test]
        public void Serialize_Dictionary_KeyContext()
        {
            var d = new Dictionary<DateTime, String>()
            {
                [new DateTime(2020, 1, 1)] = "1",
            };

            var result = CreateSerializer().Serialize<Dictionary<DateTime, String>>(
                d, new JsonNodeSerializationContext() {
                KeyContext = new JsonKeySerializationContext() {
                    Format = "D",
                    FormatProvider = CultureInfo.GetCultureInfo("de-DE")
                }
            });

            Assert.That(result, Is.EqualTo("{\"Mittwoch, 1. Januar 2020\":\"1\"}"));
        }
        
        [Test]
        public void Deserialize_Dictionary_KeyContext()
        {
            var result = CreateSerializer().Deserialize<Dictionary<DateTime, String>>(
                "{\"Mittwoch, 1. Januar 2020\":\"1\"}", 
                new JsonNodeSerializationContext() {
                KeyContext = new JsonKeySerializationContext() {
                    Format = "D",
                    FormatProvider = CultureInfo.GetCultureInfo("de-DE")
                }
            });

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[new DateTime(2020, 1, 1)], Is.EqualTo("1"));
        }
        
        [Test]
        public void Serialize_Dictionary_ItemContext()
        {
            var d = new Dictionary<int, DateTime>()
            {
                [1] = new DateTime(2020, 1, 1),
            };

            var result = CreateSerializer().Serialize<Dictionary<int, DateTime>>(d, new JsonNodeSerializationContext() {
                KeyContext = new JsonKeySerializationContext() {
                    Converter = JsonNumberConverter.Default
                },
                ItemContext = new JsonItemSerializationContext() {
                    Format = "D",
                    FormatProvider = CultureInfo.GetCultureInfo("de-DE")
                }
            });

            Assert.That(result, Is.EqualTo("{\"1\":\"Mittwoch, 1. Januar 2020\"}"));
        }
        
        [Test]
        public void Deserialize_Dictionary_ItemContext()
        {
            var result = CreateSerializer().Deserialize<Dictionary<int, DateTime>>(
                "{\"1\":\"Mittwoch, 1. Januar 2020\"}", 
                new JsonNodeSerializationContext {
                    KeyContext = new JsonKeySerializationContext() {
                        Converter = JsonNumberConverter.Default
                    },
                    ItemContext = new JsonItemSerializationContext {
                        Format = "D",
                        FormatProvider = CultureInfo.GetCultureInfo("de-DE")
                    }
                });

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[1], Is.EqualTo(new DateTime(2020, 1, 1)));
        }
        
        [Test]
        public void Serialize_List_ItemContext()
        {
            var d = new List<DateTime>()
            {
               new DateTime(2020, 1, 1),
            };
            
            var result = CreateSerializer().Serialize<List<DateTime>>(d, new JsonNodeSerializationContext() {
                ItemContext = new JsonItemSerializationContext() {
                    Format = "D",
                    FormatProvider = CultureInfo.GetCultureInfo("de-DE")
                }
            });

            Assert.That(result, Is.EqualTo("[\"Mittwoch, 1. Januar 2020\"]"));
        }
        
        [Test]
        public void Deserialize_List_CustomItemContext()
        {
            var result = CreateSerializer().Deserialize<List<DateTime>>(
                "[\"Mittwoch, 1. Januar 2020\"]", 
                new JsonNodeSerializationContext {
                    ItemContext = new JsonItemSerializationContext {
                        Format = "D",
                        FormatProvider = CultureInfo.GetCultureInfo("de-DE")
                    }
                });

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0], Is.EqualTo(new DateTime(2020, 1, 1)));
        }
        
        [Test]
        public void Serialize_ClassWithFormattedMapKey()
        {
            var o = new ClassWithFormattedMapKey() {
                Foo = new Dictionary<DateTime, string>() {
                    [new DateTime(2020, 1, 1)] = "1",
                }
            };

            var result = CreateSerializer().Serialize<ClassWithFormattedMapKey>(o);

            Assert.That(result, Is.EqualTo("{\"foo\":{\"Mittwoch, 1. Januar 2020\":\"1\"}}"));
        }
        
        [Test]
        public void Deserialize_ClassWithFormattedMapKey()
        {
            var serializer = CreateSerializer();
            
            var result = serializer.Deserialize<ClassWithFormattedMapKey>(
                "{\"foo\":{\"Mittwoch, 1. Januar 2020\":\"1\"}}");
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Foo, Is.Not.Null);
            Assert.That(result.Foo.Count, Is.EqualTo(1));
            Assert.That(result.Foo[new DateTime(2020, 1, 1)], Is.EqualTo("1"));
        }
        
        [Test]
        public void Serialize_ClassWithFormattedMapItem()
        {
            var o = new ClassWithFormattedMapItem() {
                Foo = new Dictionary<string, DateTime>() {
                    ["1"] = new DateTime(2020, 1, 1),
                }
            };
            
            var result = CreateSerializer().Serialize<ClassWithFormattedMapItem>(o);

            Assert.That(result, Is.EqualTo("{\"foo\":{\"1\":\"Mittwoch, 1. Januar 2020\"}}"));
        }
        
        [Test]
        public void Deserialize_ClassWithFormattedMapItem()
        {
            var result = CreateSerializer().Deserialize<ClassWithFormattedMapItem>(
                "{\"foo\":{\"1\":\"Mittwoch, 1. Januar 2020\"}}");
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Foo, Is.Not.Null);
            Assert.That(result.Foo.Count, Is.EqualTo(1));
            Assert.That(result.Foo["1"], Is.EqualTo(new DateTime(2020, 1, 1)));
        }
    }
}