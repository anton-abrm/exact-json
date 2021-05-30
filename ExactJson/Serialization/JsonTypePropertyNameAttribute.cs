using System;

namespace ExactJson.Serialization
{
    public sealed class JsonTypePropertyNameAttribute : JsonNodeModifierAttribute
    {
        public JsonTypePropertyNameAttribute(string propertyName)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        }

        public string PropertyName { get; set; }
    }
}