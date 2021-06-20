using System;

namespace ExactJson.Serialization.Converters
{
    public sealed class JsonGuidConverter : JsonConverter
    {
        private const string DefaultFormat = "D";

        public override string GetString(object value, JsonConverterContext context)
        {
            if (value is null) {
                throw new ArgumentNullException(nameof(value));
            }

            return ((Guid) value).ToString(context.Format ?? DefaultFormat);
        }

        public override object GetValue(string s, JsonConverterContext context)
        {
            try {
                return Guid.ParseExact(s, context.Format ?? DefaultFormat);
            }
            catch (FormatException) {
                throw new JsonInvalidValueException();
            }
        }
    }
}