using System;
using System.Globalization;

namespace ExactJson.Serialization.Converters
{
    public sealed class JsonTimeSpanConverter : JsonConverter
    {
        private const string DefaultFormat = "hh\\:mm\\:ss";

        public override string GetString(object value, JsonConverterContext context)
        {
            if (value is null) {
                throw new ArgumentNullException(nameof(value));
            }

            return ((TimeSpan) value).ToString(
                context.Format ?? DefaultFormat,
                context.FormatProvider ?? CultureInfo.InvariantCulture);
        }

        public override object GetValue(string s, JsonConverterContext context)
        {
            try {
                return TimeSpan.ParseExact(s,
                    context.Format ?? DefaultFormat,
                    context.FormatProvider ?? CultureInfo.InvariantCulture);
            }
            catch (FormatException) {
                throw new JsonInvalidValueException();
            }
        }
    }
}