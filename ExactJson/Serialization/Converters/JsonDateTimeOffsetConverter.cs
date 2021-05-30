using System;
using System.Globalization;

namespace ExactJson.Serialization.Converters
{
    public sealed class JsonDateTimeOffsetConverter : JsonStringConverter
    {
        private const string DefaultFormat = "yyyy-MM-ddTHH:mm:sszzz";

        public static JsonDateTimeOffsetConverter Default { get; } = new JsonDateTimeOffsetConverter();

        public override string GetString(object value, JsonConverterContext context)
        {
            return ((DateTimeOffset) value).ToString(
                context.Format ?? DefaultFormat, 
                context.FormatProvider ?? CultureInfo.InvariantCulture);
        }

        public override object GetValue(string s, JsonConverterContext context)
        {
            try {
                return DateTimeOffset.ParseExact(s, 
                    context.Format ?? DefaultFormat, 
                    context.FormatProvider ?? CultureInfo.InvariantCulture);
            }
            catch (FormatException) {
                throw new JsonInvalidValueException();
            }
        }
    }
}