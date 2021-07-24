// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using ExactJson.Serialization;
using ExactJson.Serialization.Converters;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization.Converters
{
    public sealed class JsonGuidConverterTests
    {
        [TestCase(null, "18b8ef78-338d-4217-9403-d99a8fd00c74")]
        [TestCase("D", "18b8ef78-338d-4217-9403-d99a8fd00c74")]
        [TestCase("N", "18b8ef78338d42179403d99a8fd00c74")]
        public void GetString(string format, string guid)
        {
            var converter = new JsonGuidConverter();

            var result = converter.GetString(new Guid(guid), new JsonConverterContext() {
                Format = format
            });

            Assert.That(result, Is.EqualTo(guid));
        }

        [TestCase(null, "18b8ef78-338d-4217-9403-d99a8fd00c74")]
        [TestCase("D", "18b8ef78-338d-4217-9403-d99a8fd00c74")]
        [TestCase("N", "18b8ef78338d42179403d99a8fd00c74")]
        public void GetValue(string format, string guid)
        {
            var converter = new JsonGuidConverter();

            var result = converter.GetValue(guid, new JsonConverterContext() {
                Format = format
            });

            Assert.That(result, Is.EqualTo(new Guid(guid)));
        }

        [Test]
        public void GetString_NullValue_ThrowsArgumentNullException()
        {
            var converter = new JsonGuidConverter();

            Assert.Throws<ArgumentNullException>(() 
                => converter.GetString(null, new JsonConverterContext()));
        }
        
        [Test]
        public void GetValue_NullString_ThrowsArgumentNullException()
        {
            var converter = new JsonGuidConverter();

            Assert.Throws<ArgumentNullException>(() 
                => converter.GetValue(null, new JsonConverterContext()));
        }
        
        [Test]
        public void GetValue_InvalidString_ThrowsJsonInvalidValueException()
        {
            var converter = new JsonGuidConverter();

            Assert.Throws<JsonInvalidValueException>(() 
                => converter.GetValue("", new JsonConverterContext()));
        }
    }
}