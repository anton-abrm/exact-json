using System;

using ExactJson.Serialization;
using ExactJson.Serialization.Converters;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization.Converters
{
    public sealed class JsonVersionConverterTests
    {
        [TestCase("1.0.0.0")]
        public void GetString(string version)
        {
            var converter = new JsonVersionConverter();

            var result = converter.GetString(new Version(version), new JsonConverterContext());

            Assert.That(result, Is.EqualTo(version));
        }

        [TestCase("1.0.0.0")]
        public void GetValue(string version)
        {
            var converter = new JsonVersionConverter();

            var result = converter.GetValue(version, new JsonConverterContext());

            Assert.That(result, Is.EqualTo(new Version(version)));
        }

        [Test]
        public void GetString_NullValue_ThrowsArgumentNullException()
        {
            var converter = new JsonVersionConverter();

            Assert.Throws<ArgumentNullException>(() 
                => converter.GetString(null, new JsonConverterContext()));
        }
        
        [Test]
        public void GetValue_NullString_ThrowsArgumentNullException()
        {
            var converter = new JsonVersionConverter();

            Assert.Throws<ArgumentNullException>(() 
                => converter.GetValue(null, new JsonConverterContext()));
        }
        
        [Test]
        public void GetValue_InvalidString_ThrowsJsonInvalidValueException()
        {
            var converter = new JsonVersionConverter();

            Assert.Throws<JsonInvalidValueException>(() 
                => converter.GetValue("", new JsonConverterContext()));
        }
    }
}