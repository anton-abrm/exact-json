using System;

namespace ExactJson.Serialization.Converters
{
    public sealed class JsonUriConverter : JsonStringConverter
    {
        public override string GetString(object value, JsonConverterContext context)
        {
            if (value is null) {
                throw new ArgumentNullException(nameof(value));
            }

            return ((Uri) value).ToString();
        }

        public override object GetValue(string s, JsonConverterContext context)
        {
            try {
                return new Uri(s);
            }
            catch (FormatException) {
                throw new JsonInvalidValueException();
            }
        }
    }
}