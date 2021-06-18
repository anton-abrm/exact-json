using System;

using ExactJson.Serialization;
using ExactJson.Serialization.Converters;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization.Converters
{
    public sealed class JsonByteArrayConverterTests
    {
        [TestCase("SGVsbG8h")]
        public void GetString(string base64)
        {
            var converter = new JsonByteArrayConverter();

            var result = converter.GetString(Convert.FromBase64String(base64), new JsonConverterContext());

            Assert.That(result, Is.EqualTo(base64));
        }

        [TestCase("SGVsbG8h")]
        public void GetValue(string base64)
        {
            var converter = new JsonByteArrayConverter();

            var result = converter.GetValue(base64, new JsonConverterContext());

            Assert.That(result, Is.EquivalentTo(Convert.FromBase64String(base64)));
        }

        [Test]
        public void GetString_NullValue_ThrowsArgumentNullException()
        {
            var converter = new JsonByteArrayConverter();

            Assert.Throws<ArgumentNullException>(() 
                => converter.GetString(null, new JsonConverterContext()));
        }
        
        [Test]
        public void GetValue_NullString_ThrowsArgumentNullException()
        {
            var converter = new JsonByteArrayConverter();

            Assert.Throws<ArgumentNullException>(() 
                => converter.GetValue(null, new JsonConverterContext()));
        }
        
        [Test]
        public void GetValue_InvalidString_ThrowsJsonInvalidValueException()
        {
            var converter = new JsonByteArrayConverter();

            Assert.Throws<JsonInvalidValueException>(() 
                => converter.GetValue("<>", new JsonConverterContext()));
        }
    }
}