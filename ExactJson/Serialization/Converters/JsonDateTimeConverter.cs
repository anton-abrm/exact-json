using System;
using System.Globalization;

namespace ExactJson.Serialization.Converters
{
    public sealed class JsonDateTimeConverter : JsonStringConverter
    {
        private const string DefaultFormat = "yyyy-MM-ddTHH:mm:ss";

        public static JsonDateTimeConverter Default { get; } = new JsonDateTimeConverter();

        public override string GetString(object value, JsonConverterContext context)
        {
            if (value is null) {
                throw new ArgumentNullException(nameof(value));
            }

            return ((DateTime) value).ToString(
                context.Format ?? DefaultFormat, 
                context.FormatProvider ?? CultureInfo.InvariantCulture);
        }

        public override object GetValue(string s, JsonConverterContext context)
        {
            try {
                return DateTime.ParseExact(s, 
                    context.Format ?? DefaultFormat, 
                    context.FormatProvider ?? CultureInfo.InvariantCulture);
            }
            catch (FormatException) {
                throw new JsonInvalidValueException();
            }
        }
    }
}