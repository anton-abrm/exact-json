using System.Collections.Generic;
using System.Collections.ObjectModel;

using ExactJson.Serialization;
using ExactJson.Serialization.Converters;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization.Features
{
    public sealed class KeyItemContextTests
    {
        private class CustomDictionaryBase : Dictionary<int, int> { }
        private class CustomDictionary : CustomCollectionBase { }
        
        private class CustomCollectionBase : Collection<int> { }
        private class CustomCollection : CustomCollectionBase { }

        [Test]
        public void Serialize_BoundDictionaryContext()
        {
            var serializer = new JsonSerializer();

            serializer.SetContext<Dictionary<int, int>>(new JsonNodeSerializationContext() {
                KeyContext = new JsonKeySerializationContext() {
                    Converter = new JsonNumberConverter()
                },
                ItemContext = new JsonItemSerializationContext() {
                    Format = ".1"
                }
            });
            
            serializer.SetContext<CustomDictionaryBase>(new JsonNodeSerializationContext() {
                KeyContext = null,
                ItemContext = null
            });
            
            serializer.SetContext<CustomDictionary>(new JsonNodeSerializationContext() {
                KeyContext = null,
                ItemContext = null
            });

            var result = serializer.Serialize<Dictionary<int, int>>(new Dictionary<int, int>() {
                [1] = 1
            });

            Assert.That(result, Is.EqualTo("{\"1\":1.0}"));
        }

        [Test]
        public void Deserialize_BoundDictionaryContext()
        {
            var serializer = new JsonSerializer();

            serializer.SetContext<Dictionary<int, int>>(new JsonNodeSerializationContext() {
                KeyContext = new JsonKeySerializationContext() {
                    Converter = new JsonNumberConverter()
                },
                ItemContext = new JsonItemSerializationContext() {
                    Format = ".1"
                }
            });
            
            serializer.SetContext<CustomDictionaryBase>(new JsonNodeSerializationContext() {
                KeyContext = null,
                ItemContext = null
            });
            
            serializer.SetContext<CustomDictionary>(new JsonNodeSerializationContext() {
                KeyContext = null,
                ItemContext = null
            });

            var result = serializer.Deserialize<Dictionary<int, int>>("{\"1\":1.0}");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[1], Is.EqualTo(1));
        }

        [Test]
        public void Serialize_BoundCollectionContext()
        {
            var serializer = new JsonSerializer();

            serializer.SetContext<Collection<int>>(new JsonNodeSerializationContext {
                ItemContext = new JsonItemSerializationContext() {
                    Format = ".2"
                }
            });
            
            serializer.SetContext<CustomCollectionBase>(new JsonNodeSerializationContext {
                ItemContext = null
            });
            
            serializer.SetContext<CustomCollection>(new JsonNodeSerializationContext {
                ItemContext = null
            });

            var result = serializer.Serialize<CustomCollection>(new CustomCollection { 1 });

            Assert.That(result, Is.EqualTo("[1.00]"));
        }
        
        [Test]
        public void Deserialize_BoundCollectionContext()
        {
            var serializer = new JsonSerializer();

            serializer.SetContext<Collection<int>>(new JsonNodeSerializationContext {
                ItemContext = new JsonItemSerializationContext() {
                    Format = ".2"
                }
            });
            
            serializer.SetContext<CustomCollectionBase>(new JsonNodeSerializationContext {
                ItemContext = null
            });
            
            serializer.SetContext<CustomCollection>(new JsonNodeSerializationContext {
                ItemContext = null
            });

            var result = serializer.Deserialize<CustomCollection>("[1.00]");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0], Is.EqualTo(1));
        }
    }
}