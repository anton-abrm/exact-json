using System;
using System.Globalization;

using ExactJson.Serialization;
using ExactJson.Serialization.Converters;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization.Converters
{
    public sealed class JsonDateTimeOffsetConverterTests
    {
        [TestCase(null, null, "2024-12-04T14:06:08+03:00", 638689179680000000L, 3)]
        [TestCase("yyyy-MM-ddTHH:mm:ssK", null, "2024-12-04T14:06:08+03:00", 638689179680000000L, 3)]
        [TestCase("dddd, dd MMMM yyyy HH:mm:ssK", null, "Wednesday, 04 December 2024 14:06:08+03:00", 638689179680000000L, 3)]
        [TestCase("dddd, dd MMMM yyyy HH:mm:ssK", "de-DE", "Mittwoch, 04 Dezember 2024 14:06:08+03:00", 638689179680000000L, 3)]
        public void GetString(string format, string culture, string value, long ticks, double hours)
        {
            var converter = new JsonDateTimeOffsetConverter();
            var formatProvider = culture is not null 
                ? CultureInfo.GetCultureInfo(culture) 
                : null;
            
            var result = converter.GetString(new DateTimeOffset(ticks, TimeSpan.FromHours(hours)), new JsonConverterContext() {
                Format = format,
                FormatProvider = formatProvider
            });

            Assert.That(result, Is.EqualTo(value));
        }

        [TestCase(null, null, "2024-12-04T14:06:08+03:00", 638689179680000000L, 3)]
        [TestCase("yyyy-MM-ddTHH:mm:ssK", null, "2024-12-04T14:06:08+03:00", 638689179680000000L, 3)]
        [TestCase("dddd, dd MMMM yyyy HH:mm:ssK", null, "Wednesday, 04 December 2024 14:06:08+03:00", 638689179680000000L, 3)]
        [TestCase("dddd, dd MMMM yyyy HH:mm:ssK", "de-DE", "Mittwoch, 04 Dezember 2024 14:06:08+03:00", 638689179680000000L, 3)]
        public void GetValue(string format, string culture, string value, long ticks, double hours)
        {
            var converter = new JsonDateTimeOffsetConverter();
            var formatProvider = culture is not null 
                ? CultureInfo.GetCultureInfo(culture) 
                : null;
            
            var result = converter.GetValue(value, new JsonConverterContext() {
                Format = format,
                FormatProvider = formatProvider
            });

            Assert.That(result, Is.EqualTo(new DateTimeOffset(ticks, TimeSpan.FromHours(hours))));
        }

        [Test]
        public void GetString_NullValue_ThrowsArgumentNullException()
        {
            var converter = new JsonDateTimeOffsetConverter();

            Assert.Throws<ArgumentNullException>(() 
                => converter.GetString(null, new JsonConverterContext()));
        }
        
        [Test]
        public void GetValue_NullString_ThrowsArgumentNullException()
        {
            var converter = new JsonDateTimeOffsetConverter();

            Assert.Throws<ArgumentNullException>(() 
                => converter.GetValue(null, new JsonConverterContext()));
        }
        
        [Test]
        public void GetValue_InvalidString_ThrowsJsonInvalidValueException()
        {
            var converter = new JsonDateTimeOffsetConverter();

            Assert.Throws<JsonInvalidValueException>(() 
                => converter.GetValue("", new JsonConverterContext()));
        }
    }
}