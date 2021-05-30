using System;

using ExactJson.Infra;

namespace ExactJson.Serialization.Converters
{
    public sealed class JsonByteArrayConverter : JsonStringConverter
    {
        public const string Base64Format = "Base64";
        public const string Base16Format = "Base16";

        public static JsonByteArrayConverter Default { get; } = new JsonByteArrayConverter();

        public override string GetString(object value, JsonConverterContext context)
        {
            if (value is null) {
                throw new ArgumentNullException(nameof(value));
            }

            string format = context.Format ?? Base64Format;

            switch (format) {
                case Base16Format:
                    return HexUtil.ToHex((byte[]) value);

                case Base64Format:
                    return Convert.ToBase64String((byte[]) value);
            }

            throw new InvalidOperationException($"Format {format} not supported.");
        }

        public override object GetValue(string s, JsonConverterContext context)
        {
            string format = context.Format ?? Base64Format;

            try {
                switch (format) {
                    
                    case Base16Format:
                        return HexUtil.FromHex(s);

                    case Base64Format:
                        return Convert.FromBase64String(s);

                    default:
                        throw new InvalidOperationException($"Format {format} not supported.");
                }
            }
            catch (FormatException) {
                throw new JsonInvalidValueException();
            }
        }
    }
}