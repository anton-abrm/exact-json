using System.Globalization;

using ExactJson.Serialization;

using NUnit.Framework;

namespace ExactJson.Tests.Unit.Serialization
{
    public sealed class JsonConverterContextTests
    {
        [Test]
        public void AllProperties_Default()
        {
            var context = new JsonConverterContext();

            Assert.That(context.Format, Is.Null);
            Assert.That(context.FormatProvider, Is.Null);
            Assert.That(context.TargetType, Is.Null);
        }

        [Test]
        public void AllProperties_Set()
        {
            var context = new JsonConverterContext {
                Format = "",
                FormatProvider = CultureInfo.InvariantCulture,
                TargetType = typeof(object)
            };
            
            Assert.That(context.Format, Is.EqualTo(""));
            Assert.That(context.FormatProvider, Is.EqualTo(CultureInfo.InvariantCulture));
            Assert.That(context.TargetType, Is.EqualTo(typeof(object)));
        }
    }
}