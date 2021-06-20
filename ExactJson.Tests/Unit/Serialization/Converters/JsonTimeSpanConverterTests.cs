using System;
using System.Globalization;

using ExactJson.Serialization;
using ExactJson.Serialization.Converters;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization.Converters
{
    public sealed class JsonTimeSpanConverterTests
    {
        [TestCase(null, null, "16:14:30", 584700000000)]
        [TestCase("hh\\:mm\\:ss", null, "16:14:30", 584700000000)]
        [TestCase("G", null, "0:16:14:30.0000000", 584700000000)]
        [TestCase("G", "fr-FR", "0:16:14:30,0000000", 584700000000)]
        public void GetString(string format, string culture, string value, long ticks)
        {
            var converter = new JsonTimeSpanConverter();
            var formatProvider = culture is not null 
                ? CultureInfo.GetCultureInfo(culture) 
                : null;
            
            var result = converter.GetString(new TimeSpan(ticks), new JsonConverterContext {
                Format = format,
                FormatProvider = formatProvider
            });

            Assert.That(result, Is.EqualTo(value));
        }

        [TestCase(null, null, "16:14:30", 584700000000)]
        [TestCase("hh\\:mm\\:ss", null, "16:14:30", 584700000000)]
        [TestCase("G", null, "0:16:14:30.0000000", 584700000000)]
        [TestCase("G", "fr-FR", "0:16:14:30,0000000", 584700000000)]
        public void GetValue(string format, string culture, string value, long ticks)
        {
            var converter = new JsonTimeSpanConverter();
            var formatProvider = culture is not null 
                ? CultureInfo.GetCultureInfo(culture) 
                : null;
            
            var result = converter.GetValue(value, new JsonConverterContext {
                Format = format,
                FormatProvider = formatProvider
            });

            Assert.That(result, Is.EqualTo(new TimeSpan(ticks)));
        }

        [Test]
        public void GetString_NullValue_ThrowsArgumentNullException()
        {
            var converter = new JsonTimeSpanConverter();

            Assert.Throws<ArgumentNullException>(() 
                => converter.GetString(null, new JsonConverterContext()));
        }
        
        [Test]
        public void GetValue_NullString_ThrowsArgumentNullException()
        {
            var converter = new JsonTimeSpanConverter();

            Assert.Throws<ArgumentNullException>(() 
                => converter.GetValue(null, new JsonConverterContext()));
        }
        
        [Test]
        public void GetValue_InvalidString_ThrowsJsonInvalidValueException()
        {
            var converter = new JsonTimeSpanConverter();

            Assert.Throws<JsonInvalidValueException>(() 
                => converter.GetValue("", new JsonConverterContext()));
        }
    }
}