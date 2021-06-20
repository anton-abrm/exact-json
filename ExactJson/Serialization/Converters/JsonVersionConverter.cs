using System;

namespace ExactJson.Serialization.Converters
{
    public sealed class JsonVersionConverter : JsonConverter
    {
        public override string GetString(object value, JsonConverterContext context)
        {
            if (value is null) {
                throw new ArgumentNullException(nameof(value));
            }

            return ((Version) value).ToString();
        }

        public override object GetValue(string s, JsonConverterContext context)
        {
            if (s is null) {
                throw new ArgumentNullException(nameof(s));
            }

            if (Version.TryParse(s, out var version)) {
                return version;
            }
            
            throw new JsonInvalidValueException();
        }
    }
}