// Copyright (c) Anton Abramenko <anton@abramenko.dev>
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using ExactJson.Serialization;
using ExactJson.Serialization.Converters;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization.Converters
{
    public sealed class JsonUriConverterTests
    {
        [TestCase("https://example.com/")]
        public void GetString(string version)
        {
            var converter = new JsonUriConverter();

            var result = converter.GetString(new Uri(version), new JsonConverterContext());

            Assert.That(result, Is.EqualTo(version));
        }

        [TestCase("https://example.com/")]
        public void GetValue(string version)
        {
            var converter = new JsonUriConverter();

            var result = converter.GetValue(version, new JsonConverterContext());

            Assert.That(result, Is.EqualTo(new Uri(version)));
        }

        [Test]
        public void GetString_NullValue_ThrowsArgumentNullException()
        {
            var converter = new JsonUriConverter();

            Assert.Throws<ArgumentNullException>(() 
                => converter.GetString(null, new JsonConverterContext()));
        }
        
        [Test]
        public void GetValue_NullString_ThrowsArgumentNullException()
        {
            var converter = new JsonUriConverter();

            Assert.Throws<ArgumentNullException>(() 
                => converter.GetValue(null, new JsonConverterContext()));
        }
        
        [Test]
        public void GetValue_InvalidString_ThrowsJsonInvalidValueException()
        {
            var converter = new JsonUriConverter();

            Assert.Throws<JsonInvalidValueException>(() 
                => converter.GetValue("", new JsonConverterContext()));
        }
    }
}