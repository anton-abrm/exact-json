// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using ExactJson.Serialization;
using ExactJson.Serialization.Converters;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization.Converters
{
    public sealed class JsonNumberConverterTests
    {
        [TestCase("1", typeof(double), ".2", "1.00")]
        [TestCase("1", typeof(float), ".2", "1.00")]
        [TestCase("1", typeof(ulong), ".2", "1.00")]
        [TestCase("1", typeof(long), ".2", "1.00")]
        [TestCase("1", typeof(uint) , ".2", "1.00")]
        [TestCase("1", typeof(int) , ".2", "1.00")]
        [TestCase("1", typeof(ushort) , ".2", "1.00")]
        [TestCase("1", typeof(short) , ".2", "1.00")]
        [TestCase("1", typeof(byte) , ".2", "1.00")]
        [TestCase("1", typeof(sbyte) , ".2", "1.00")]
        [TestCase("1", typeof(decimal) , ".2", "1.00")]
        public void GetString(string value, Type type, string format, string expectedResult)
        {
            var converter = new JsonNumberConverter();

            var result = converter.GetString(Convert.ChangeType(value, type), new JsonConverterContext {
                Format = format,
            });

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCase("1", typeof(double))]
        [TestCase("1", typeof(float))]
        [TestCase("1", typeof(ulong))]
        [TestCase("1", typeof(long))]
        [TestCase("1", typeof(uint))]
        [TestCase("1", typeof(int))]
        [TestCase("1", typeof(ushort))]
        [TestCase("1", typeof(short))]
        [TestCase("1", typeof(byte))]
        [TestCase("1", typeof(sbyte))]
        [TestCase("1", typeof(decimal))]
        public void GetValue(string s, Type type)
        {
            var converter = new JsonNumberConverter();
           
            var result = converter.GetValue(s, new JsonConverterContext() {
                TargetType = type
            });

            Assert.That(result, Is.EqualTo(Convert.ChangeType(s, type)));
        }

        [Test]
        public void GetString_NotNumberValue_ThrowsArgumentException()
        {
            var converter = new JsonNumberConverter();

            Assert.Throws<ArgumentException>(() 
                => converter.GetString(new DateTime(), new JsonConverterContext()));
        }
        
        [Test]
        public void GetString_NullValue_ThrowsArgumentNullException()
        {
            var converter = new JsonNumberConverter();

            Assert.Throws<ArgumentNullException>(() 
                => converter.GetString(null, new JsonConverterContext()));
        }
        
        [Test]
        public void GetValue_InvalidTargetType_ThrowsArgumentException()
        {
            var converter = new JsonNumberConverter();

            Assert.Throws<ArgumentException>(() 
                => converter.GetValue("1", new JsonConverterContext() {
                    TargetType = typeof(DateTime)
                }));
        }
        
        [Test]
        public void GetValue_NullString_ThrowsArgumentNullException()
        {
            var converter = new JsonNumberConverter();

            Assert.Throws<ArgumentNullException>(() 
                => converter.GetValue(null, new JsonConverterContext()));
        }
        
        [Test]
        public void GetValue_InvalidNumber_ThrowsJsonJsonInvalidValueException()
        {
            var converter = new JsonNumberConverter();

            Assert.Throws<JsonInvalidValueException>(() 
                => converter.GetValue("", new JsonConverterContext() {
                    TargetType = typeof(byte)
                }));
        }
        
        [Test]
        public void GetValue_OutOfRange_ThrowsJsonValueOutOfRangeException()
        {
            var converter = new JsonNumberConverter();

            Assert.Throws<JsonValueOutOfRangeException>(() 
                => converter.GetValue("256", new JsonConverterContext() {
                    TargetType = typeof(byte)
                }));
        }
    }
}