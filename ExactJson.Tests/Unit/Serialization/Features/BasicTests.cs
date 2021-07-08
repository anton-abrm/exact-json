using System;
using System.Collections.Generic;

using ExactJson.Serialization;
using ExactJson.Serialization.Converters;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization.Features
{
    public class BasicTests
    {
        private enum EnumWithoutAttributes
        {
            Foo
        }
        
        private sealed class ClassWithNoGetter
        {
            [JsonNode]
            public string Foo 
            {
                set {}
            }
        }
        
        private abstract class BaseClass
        {
        }
        
        private sealed class DerivedClass : BaseClass
        {
        }
        
        private enum DemoEnum
        {
            [JsonEnumValue] A, 
            [JsonEnumValue] B, 
            [JsonEnumValue] C
        }
        
        private static readonly object[] Inputs = {
            
            new object[] { typeof(DemoEnum?), "null" },
            new object[] { typeof(DemoEnum?), "\"A\"" },
            new object[] { typeof(DemoEnum), "\"A\"" },
            new object[] { typeof(DemoEnum), "\"B\"" },
            new object[] { typeof(DemoEnum), "\"C\"" },
            
            new object[] { typeof(bool?), "null" },
            new object[] { typeof(bool?), "true" },
            new object[] { typeof(bool), "true" },
            new object[] { typeof(bool), "false" },

            new object[] { typeof(byte?), "null" },
            new object[] { typeof(byte?), "0" },
            new object[] { typeof(byte), "0" },
            new object[] { typeof(byte), "255" },

            new object[] { typeof(sbyte?), "null" },
            new object[] { typeof(sbyte?), "-128" },
            new object[] { typeof(sbyte), "-128" },
            new object[] { typeof(sbyte), "127" },

            new object[] { typeof(ushort?), "null" },
            new object[] { typeof(ushort?), "0" },
            new object[] { typeof(ushort), "0" },
            new object[] { typeof(ushort), "65535" },

            new object[] { typeof(short?), "null" },
            new object[] { typeof(short?), "-32768" },
            new object[] { typeof(short), "-32768" },
            new object[] { typeof(short), "32767" },

            new object[] { typeof(uint?), "null" },
            new object[] { typeof(uint?), "0" },
            new object[] { typeof(uint), "0" },
            new object[] { typeof(uint), "4294967295" },

            new object[] { typeof(int?), "null" },
            new object[] { typeof(int?), "-2147483648" },
            new object[] { typeof(int), "-2147483648" },
            new object[] { typeof(int), "2147483647" },

            new object[] { typeof(ulong?), "null" },
            new object[] { typeof(ulong?), "0" },
            new object[] { typeof(ulong), "0" },
            new object[] { typeof(ulong), "18446744073709551615" },

            new object[] { typeof(long?), "null" },
            new object[] { typeof(long?), "-9223372036854775808" },
            new object[] { typeof(long), "-9223372036854775808" },
            new object[] { typeof(long), "9223372036854775807" },

            new object[] { typeof(decimal?), "null" },
            new object[] { typeof(decimal?), "0" },
            new object[] { typeof(decimal), "-79228162514264337593543950335" },
            new object[] { typeof(decimal), "-7.9228162514264337593543950335" },
            new object[] { typeof(decimal), "-0.0000000000000000000000000001" },
            new object[] { typeof(decimal), "0" },
            new object[] { typeof(decimal), "0.0000000000000000000000000001" },
            new object[] { typeof(decimal), "7.9228162514264337593543950335" },
            new object[] { typeof(decimal), "79228162514264337593543950335" },

            new object[] { typeof(double?), "null" },
            new object[] { typeof(double?), "0.0" },
            new object[] { typeof(double), "0.0" },
            new object[] { typeof(double), "0.1" },
            new object[] { typeof(double), "1.0E-5" },
            new object[] { typeof(double), "0.100000000000001" },
            new object[] { typeof(double), "0.0001" },
            new object[] { typeof(double), "0.000123456789012345" },
            new object[] { typeof(double), "100000000000000.0" },
            new object[] { typeof(double), "100000000000001.0" },
            new object[] { typeof(double), "123456789012345.0" },
            new object[] { typeof(double), "1.0E+15" },
            new object[] { typeof(double), "1.23456789012345E+15" },
            new object[] { typeof(double), "0.6822871999174" },
            new object[] { typeof(double), "5.0E-324" },
            new object[] { typeof(double), "1.79769313486231E+308" },
            new object[] { typeof(double), "-1.79769313486231E+308" },
            new object[] { typeof(double), "1.79769313486231E+308" },
            new object[] { typeof(double), "-1.79769313486231E+308" },

            new object[] { typeof(float?), "null" },
            new object[] { typeof(float?), "0.0" },
            new object[] { typeof(float), "1.0E-45" },
            new object[] { typeof(float), "-3.4028235E+38" },
            new object[] { typeof(float), "3.4028235E+38" },

            new object[] { typeof(string), "null" },
            new object[] { typeof(string), "\"Hello\"" },

            new object[] { typeof(object), "null" },
            new object[] { typeof(object), "{}" },

            new object[] { typeof(List<object>), "null" },
            new object[] { typeof(List<object>), "[]" },
            new object[] { typeof(List<int>), "[1]" },
            new object[] { typeof(List<int?>), "[0]" },
            new object[] { typeof(List<int?>), "[null]" },

            new object[] { typeof(Dictionary<string, object>), "null" },
            new object[] { typeof(Dictionary<string, object>), "{}" },
            new object[] { typeof(Dictionary<string, int>), "{\"foo\":1}" },
            new object[] { typeof(Dictionary<string, int?>), "{\"foo\":1}" },
            new object[] { typeof(Dictionary<string, int?>), "{\"foo\":null}" },
        };

        [TestCaseSource(nameof(Inputs))]
        public void Serialize_InputValues(Type type, string original)
        {
            original = JsonNode.Parse(original).ToString();

            var serializer = new JsonSerializer {
                SerializeNullProperty = true
            };

            var ctx = new JsonNodeSerializationContext {
                IsOptional = true,
                ItemContext = new JsonItemSerializationContext {
                    IsOptional = true
                }
            };

            var obj = serializer.Deserialize(type, original, ctx);

            var actual = serializer.Serialize(type, obj, ctx);

            Assert.That(actual, Is.EqualTo(original));
        }

        [Test]
        public void Deserialize_EmptyObject_NonEmptyJson()
        {
            var json = "{\"a\":1, \"b\":2}";

            var serializer = new JsonSerializer();

            var result = serializer.Deserialize<object>(json);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void Deserialize_ReaderAtEndObject_ThrowsInvalidOperationException()
        {
            var r = new JsonStringReader("{}");

            Assert.That(r.Read());
            Assert.That(r.TokenType == JsonTokenType.StartObject);
            Assert.That(r.Read());
            Assert.That(r.TokenType == JsonTokenType.EndObject);

            var serializer = new JsonSerializer();

            Assert.Throws<InvalidOperationException>(() =>
                serializer.Deserialize<object>(r));
        }
        
        private static readonly object[] InvalidDeserializationTypeCases = {
            
            new object[] { typeof(bool), "\"foo\"" },
            new object[] { typeof(int), "\"foo\"" },
            new object[] { typeof(object), "\"foo\"" },
            new object[] { typeof(List<int>), "\"foo\"" },
            new object[] { typeof(Dictionary<int, int>), "\"foo\"" },
            
            new object[] { typeof(DemoEnum), "1" },
            new object[] { typeof(bool), "1" },
            new object[] { typeof(string), "1" },
            new object[] { typeof(object), "1" },
            new object[] { typeof(List<int>), "1" },
            new object[] { typeof(Dictionary<int, int>), "1" },

            new object[] { typeof(DemoEnum), "true" },
            new object[] { typeof(int), "true" },
            new object[] { typeof(string), "true" },
            new object[] { typeof(object), "true" },
            new object[] { typeof(List<int>), "true" },
            new object[] { typeof(Dictionary<int, int>), "true" },

            new object[] { typeof(DemoEnum), "{}" },
            new object[] { typeof(bool), "{}" },
            new object[] { typeof(string), "{}" },
            new object[] { typeof(int), "{}" },
            new object[] { typeof(List<int>), "{}" },

            new object[] { typeof(DemoEnum), "[]" },
            new object[] { typeof(bool), "[]" },
            new object[] { typeof(string), "[]" },
            new object[] { typeof(int), "[]" },
            new object[] { typeof(object), "[]" },
            new object[] { typeof(Dictionary<int, int>), "[]" },
        };
        
        private static readonly object[] InvalidSerializationTypeCases = {
            
            new object[] { typeof(bool), "foo" },
            new object[] { typeof(int), "foo" },
            new object[] { typeof(object), "foo" },
            new object[] { typeof(List<int>), "foo" },
            new object[] { typeof(Dictionary<int, int>), "foo" },
            
            new object[] { typeof(DemoEnum), 1 },
            new object[] { typeof(bool), 1 },
            new object[] { typeof(string), 1 },
            new object[] { typeof(object), 1 },
            new object[] { typeof(List<int>), 1},
            new object[] { typeof(Dictionary<int, int>), 1 },

            new object[] { typeof(DemoEnum), true },
            new object[] { typeof(int), true },
            new object[] { typeof(string),true },
            new object[] { typeof(object), true },
            new object[] { typeof(List<int>), true },
            new object[] { typeof(Dictionary<int, int>), true },

            new object[] { typeof(DemoEnum), new object() },
            new object[] { typeof(bool), new object() },
            new object[] { typeof(string), new object() },
            new object[] { typeof(int), new object() },
            new object[] { typeof(List<int>), new object() },

            new object[] { typeof(DemoEnum), new List<int>() },
            new object[] { typeof(bool), new List<int>() },
            new object[] { typeof(string), new List<int>() },
            new object[] { typeof(int), new List<int>() },
            new object[] { typeof(object), new List<int>() },
            new object[] { typeof(Dictionary<int, int>), new List<int>() },
        };
        
        [TestCaseSource(nameof(InvalidDeserializationTypeCases))]
        public void Deserialize_X_ThrowsJsonInvalidTypeException(Type targetType, string json)
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() =>
                serializer.Deserialize(targetType, json));
            
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidTypeException>());
        }
        
        [TestCaseSource(nameof(InvalidSerializationTypeCases))]
        public void Serialize_X_ThrowsJsonInvalidTypeException(Type targetType, object value)
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(
                () =>serializer.Serialize(targetType, value));
            
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidTypeException>());
        }

        [Test]
        public void Deserialize_OutOfRange_ThrowsJsonValueOutOfRangeException()
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(
                () => serializer.Deserialize<int>("123456789123456"));
            
            Assert.That(ex.InnerException, Is.InstanceOf<JsonValueOutOfRangeException>());
        }

        [Test]
        public void SerializeAndDeserialize_DictionaryWithConverter()
        {
            var d = new Dictionary<int, int>() {
                [1] = 2
            };

            var serializer = new JsonSerializer();

            var result = serializer.Serialize<Dictionary<int, int>>(d, new JsonNodeSerializationContext() {
                KeyContext = new JsonKeySerializationContext {
                    Converter = new JsonNumberConverter()
                },
                ItemContext = new JsonItemSerializationContext {
                    Converter = new JsonNumberConverter()
                }
            });

            Assert.That(result, Is.EqualTo("{\"1\":\"2\"}"));
        }
        
        [Test]
        public void Deserialize_DictionaryWithConverter()
        {
            var serializer = new JsonSerializer();

            var result = serializer.Deserialize<Dictionary<int, int>>("{\"1\":\"2\"}", new JsonNodeSerializationContext() {
                KeyContext = new JsonKeySerializationContext {
                    Converter = new JsonNumberConverter()
                },
                ItemContext = new JsonItemSerializationContext {
                    Converter = new JsonNumberConverter()
                }
            });

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[1], Is.EqualTo(2));
        }
        
        [Test]
        public void Serialize_DictionaryWithEnumAsKey()
        {
            var d = new Dictionary<DemoEnum, int>() {
                [DemoEnum.A] = 2
            };

            var serializer = new JsonSerializer();

            var result = serializer.Serialize<Dictionary<DemoEnum, int>>(d, new JsonNodeSerializationContext() {
                ItemContext = new JsonItemSerializationContext {
                    Converter = new JsonNumberConverter()
                }
            });

            Assert.That(result, Is.EqualTo("{\"A\":\"2\"}"));
        }
        
        [Test]
        public void Deserialize_DictionaryWithEnumAsKey()
        {
            var serializer = new JsonSerializer();

            var result = serializer.Deserialize<Dictionary<DemoEnum, int>>("{\"A\":\"2\"}", new JsonNodeSerializationContext() {
                ItemContext = new JsonItemSerializationContext {
                    Converter = new JsonNumberConverter()
                }
            });

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[DemoEnum.A], Is.EqualTo(2));
        }

        [Test]
        public void Serialize_ClassWithNoGetter_ThrowsJsonInvalidTypeException()
        {
            var value = new ClassWithNoGetter();

            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Serialize<ClassWithNoGetter>(value));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidTypeException>());
        }
        
        [Test]
        public void Deserialize_ClassWithNoGetter_ThrowsJsonInvalidTypeException()
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Deserialize<ClassWithNoGetter>("{\"Foo\":\"bar\"}"));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidTypeException>());
        }
        
        [Test]
        public void Serialize_EnumWithoutAttributes_ThrowsJsonInvalidValueException()
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Serialize<EnumWithoutAttributes>(EnumWithoutAttributes.Foo));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidValueException>());
        }
        
        [Test]
        public void Deserialize_EnumWithoutAttributes_ThrowsJsonInvalidValueException()
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Deserialize<EnumWithoutAttributes>("\"Bar\""));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidValueException>());
        }

        [Test]
        public void Serialize_ICollection_JsonInvalidTypeException()
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Serialize<ICollection<string>>(new List<string>()));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidTypeException>());
        }
        
        [Test]
        public void Deserialize_ICollection_JsonInvalidTypeException()
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Deserialize<ICollection<string>>("[]"));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidTypeException>());
        }
        
        [Test]
        public void Serialize_IDictionary_JsonInvalidTypeException()
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Serialize<IDictionary<string, string>>(new Dictionary<string, string>()));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidTypeException>());
        }
        
        [Test]
        public void Deserialize_IDictionary_JsonInvalidTypeException()
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Deserialize<IDictionary<string, string>>("{}"));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidTypeException>());
        }

        [Test]
        public void Serialize_DerivedClassWithoutAlias()
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Serialize<BaseClass>(new DerivedClass()));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidTypeException>());
        }
        
        [Test]
        public void Serialize_DictionaryWithInvalidEnumAsKey()
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Serialize<Dictionary<EnumWithoutAttributes, string>>(new Dictionary<EnumWithoutAttributes, string>() {
                    [EnumWithoutAttributes.Foo] = "Bar"
                }));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidValueException>());
        }
        
        [Test]
        public void Deserialize_DictionaryWithInvalidEnumAsKey()
        {
            var serializer = new JsonSerializer();

            var ex = Assert.Throws<JsonSerializationException>(() 
                => serializer.Deserialize<Dictionary<EnumWithoutAttributes, string>>("{\"Foo\":\"Bar\"}"));
            
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<JsonInvalidValueException>());
        }
    }
}