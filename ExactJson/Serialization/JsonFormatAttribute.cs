using System;

namespace ExactJson.Serialization
{
    public sealed class JsonFormatAttribute : JsonNodeModifierAttribute
    {
        public JsonFormatAttribute(string format)
        {
            Format = format ?? throw new ArgumentNullException(nameof(format));
        }

        public string Format { get; }
    }
}