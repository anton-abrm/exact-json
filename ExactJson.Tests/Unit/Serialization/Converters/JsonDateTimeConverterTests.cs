using System;
using System.Globalization;

using ExactJson.Serialization;
using ExactJson.Serialization.Converters;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization.Converters
{
    public sealed class JsonDateTimeConverterTests
    {
        [TestCase(null, null, "2024-12-04T14:06:08", 638689179680000000L)]
        [TestCase("yyyy-MM-ddTHH:mm:ss", null, "2024-12-04T14:06:08", 638689179680000000L)]
        [TestCase("F", null, "Wednesday, 04 December 2024 14:06:08", 638689179680000000L)]
        [TestCase("F", "de-DE", "Mittwoch, 4. Dezember 2024 14:06:08", 638689179680000000L)]
        public void GetString(string format, string culture, string value, long ticks)
        {
            var converter = new JsonDateTimeConverter();
            var formatProvider = culture != null 
                ? CultureInfo.GetCultureInfo(culture) 
                : null;
            
            var result = converter.GetString(new DateTime(ticks), new JsonConverterContext() {
                Format = format,
                FormatProvider = formatProvider
            });

            Assert.That(result, Is.EqualTo(value));
        }

        [TestCase(null, null, "2024-12-04T14:06:08", 638689179680000000L)]
        [TestCase("yyyy-MM-ddTHH:mm:ss", null, "2024-12-04T14:06:08", 638689179680000000L)]
        [TestCase("F", null, "Wednesday, 04 December 2024 14:06:08", 638689179680000000L)]
        [TestCase("F", "de-DE", "Mittwoch, 4. Dezember 2024 14:06:08", 638689179680000000L)]
        public void GetValue(string format, string culture, string value, long ticks)
        {
            var converter = new JsonDateTimeConverter();
            var formatProvider = culture != null 
                ? CultureInfo.GetCultureInfo(culture) 
                : null;
            
            var result = converter.GetValue(value, new JsonConverterContext() {
                Format = format,
                FormatProvider = formatProvider
            });

            Assert.That(result, Is.EqualTo(new DateTime(ticks)));
        }

        [Test]
        public void GetString_NullValue_ThrowsArgumentNullException()
        {
            var converter = new JsonDateTimeConverter();

            Assert.Throws<ArgumentNullException>(() 
                => converter.GetString(null, new JsonConverterContext()));
        }
        
        [Test]
        public void GetValue_NullString_ThrowsArgumentNullException()
        {
            var converter = new JsonDateTimeConverter();

            Assert.Throws<ArgumentNullException>(() 
                => converter.GetValue(null, new JsonConverterContext()));
        }
        
        [Test]
        public void GetValue_InvalidString_ThrowsJsonInvalidValueException()
        {
            var converter = new JsonDateTimeConverter();

            Assert.Throws<JsonInvalidValueException>(() 
                => converter.GetValue("", new JsonConverterContext()));
        }
    }
}